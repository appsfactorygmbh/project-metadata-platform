import { mount, VueWrapper } from '@vue/test-utils';
import { beforeEach, describe, expect, it } from 'vitest';
import { createTestingPinia } from '@pinia/testing';
import { projectEditStoreSymbol } from '../../../store/injectionSymbols';
import { useProjectEditStore } from '../../../store';
import PluginComponent from '../PluginComponent.vue';
import { createPinia, setActivePinia } from 'pinia';
import type { PluginEditModel, PluginModel } from '@/models/Plugin';

interface PluginComponentInstance {
  pluginName: string;
  url: string;
  displayName: string;
  id: number;
  isLoading: boolean;
  isEditing: boolean;
  hide: boolean;
  editKey: number;
  isDeleted: boolean;
}

const generateWrapper = (
  name: string,
  url: string,
  displayName: string,
  isLoading: boolean,
  isEditing: boolean,
  id: number,
): VueWrapper<ComponentPublicInstance<PluginComponentInstance>> => {
  return mount(PluginComponent, {
    props: {
      pluginName: name,
      url: url,
      displayName: displayName,
      isLoading: isLoading,
      isEditing: isEditing,
      id: id,
      editKey: -1,
    },
    plugins: [
      createTestingPinia({
        stubActions: false,
      }),
    ],
    global: {
      provide: {
        [projectEditStoreSymbol as symbol]: useProjectEditStore(),
      },
    },
  }) as VueWrapper<ComponentPublicInstance<PluginComponentInstance>>;
};

beforeEach(() => {
  setActivePinia(createPinia());
});

describe('PluginComponent.vue', () => {
  it('renders correctly when in editing mode', () => {
    const wrapper = generateWrapper(
      'Test Plugin',
      'https://example.com/examplePath',
      'test instance',
      false,
      true,
      100,
    );

    const inputs = wrapper.findAllComponents({ name: 'AInput' });
    expect(inputs.length).toEqual(2);
    const deleteIcon = wrapper.findComponent({ name: 'DeleteOutlined' });
    expect(deleteIcon.exists()).toBeTruthy();
  });

  it('updates the project data on input', async () => {
    const testPlugin = {
      displayName: 'test instance',
      url: 'https://example.com/examplePath',
      id: 100,
      pluginName: 'Test Plugin',
    };
    const projectEditStore = useProjectEditStore();
    projectEditStore.resetChanges();
    const index = projectEditStore.initialAdd(testPlugin);

    const wrapper = mount(PluginComponent, {
      props: {
        pluginName: testPlugin.pluginName,
        url: testPlugin.url,
        displayName: testPlugin.displayName,
        isLoading: false,
        isEditing: true,
        id: testPlugin.id,
        editKey: index,
        isDeleted: false,
      },
      global: {
        provide: {
          [projectEditStoreSymbol as symbol]: useProjectEditStore(),
        },
      },
    });

    projectEditStore.updatePluginChanges = vi.fn();

    const inputs = wrapper.findAll('input');
    const displayNameInput = inputs[0];
    const urlInput = inputs[1];

    await displayNameInput.setValue('new display name');
    expect(projectEditStore.updatePluginChanges).toHaveBeenCalledWith(index, {
      displayName: 'new display name',
      url: 'https://example.com/examplePath',
      id: 100,
      pluginName: 'Test Plugin',
      editKey: index,
      isDeleted: false,
    });
    await urlInput.setValue('https://example.com/newPath');
    expect(projectEditStore.updatePluginChanges).toHaveBeenCalledWith(index, {
      displayName: 'new display name',
      url: 'https://example.com/newPath',
      id: 100,
      pluginName: 'Test Plugin',
      editKey: index,
      isDeleted: false,
    });
  });

  it('hides the plugin correctly when DeleteOutlined icon is clicked', async () => {
    const testPlugin = {
      displayName: 'test instance',
      url: 'https://example.com/examplePath',
      id: 100,
      pluginName: 'Test Plugin',
    };

    const projectEditStore = useProjectEditStore();
    const index = projectEditStore.initialAdd(testPlugin);

    // Mock projectEditStore.deletePlugin
    const mockDeletePlugin = vi.fn();
    projectEditStore.deletePlugin = mockDeletePlugin;

    const wrapper: VueWrapper<
      ComponentPublicInstance<PluginComponentInstance>
    > = mount(PluginComponent, {
      props: {
        pluginName: 'Test Plugin',
        url: 'https://example.com',
        displayName: 'Test Instance',
        id: 1,
        isLoading: false,
        isEditing: true,
        hide: false,
        editKey: 0,
        isDeleted: false,
      },
      global: {
        provide: {
          [projectEditStoreSymbol as symbol]: useProjectEditStore(),
        },
      },
    }) as VueWrapper<ComponentPublicInstance<PluginComponentInstance>>;

    const deleteIcon = wrapper.findComponent({ name: 'DeleteOutlined' });
    await deleteIcon.trigger('click');

    expect(wrapper.vm.hide).toBe(true);
    expect(mockDeletePlugin).toHaveBeenCalledWith(index);
  });
});

describe('ProjectEditStore', () => {
  let store: ReturnType<typeof useProjectEditStore>;

  beforeEach(() => {
    setActivePinia(createPinia());
    store = useProjectEditStore();
  });

  it('initializes with default state', () => {
    expect(store.pluginChanges.size).toBe(0);
    expect(store.projectInformationChanges).toEqual([]);
    expect(store.canBeCreated).toBe(true);
    expect(store.pluginsWithUrlConflicts).toEqual([]);
    expect(store.duplicatedUrls.size).toBe(0);
    expect(store.emptyFields.size).toBe(0);
  });

  it('adds and removes empty fields correctly', () => {
    store.addEmptyField(1);
    expect(store.emptyFields.size).toBe(1);
    expect(store.emptyFields.has(1)).toBe(true);

    store.removeEmptyField(1);
    expect(store.emptyFields.size).toBe(0);
    expect(store.emptyFields.has(1)).toBe(false);
  });

  it('resets changes correctly', () => {
    store.addEmptyField(1);
    store.resetChanges();
    expect(store.pluginChanges.size).toBe(0);
    expect(store.projectInformationChanges).toEqual([]);
    expect(store.canBeCreated).toBe(true);
    expect(store.pluginsWithUrlConflicts).toEqual([]);
    expect(store.duplicatedUrls.size).toBe(0);
    expect(store.emptyFields.size).toBe(0);
  });

  it('checks for URL conflicts correctly', () => {
    const plugin: PluginModel = {
      id: 1,
      url: 'http://example.com',
      pluginName: 'Test Plugin',
      displayName: 'Test Plugin',
    };
    store.initialAdd(plugin);
    store.initialAdd(plugin);
    store.checkForConflicts();

    expect(store.duplicatedUrls.size).toBe(1);
    expect(store.duplicatedUrls.get('http://example.com')?.length).toBe(2);
    expect(store.getPluginsWithUrlConflicts.length).toBe(2);
  });

  it('adds and updates plugins correctly', () => {
    const plugin: PluginModel = {
      id: 1,
      url: 'http://example.com',
      displayName: 'Test Plugin',
      pluginName: 'Test Plugin',
    };
    const pluginEdit: PluginEditModel = {
      ...plugin,
      editKey: 0,
      isDeleted: false,
    };

    const index = store.initialAdd(plugin);
    expect(store.pluginChanges.size).toBe(1);
    expect(store.pluginChanges.get(index)).toEqual(pluginEdit);

    const updatedPluginEdit: PluginEditModel = {
      ...pluginEdit,
      displayName: 'Updated Plugin',
      pluginName: 'Test Plugin',
    };
    store.updatePluginChanges(index, updatedPluginEdit);
    expect(store.pluginChanges.get(index)).toEqual(updatedPluginEdit);
  });

  it('deletes plugins correctly', () => {
    const plugin: PluginModel = {
      id: 1,
      url: 'http://example.com',
      displayName: 'Test Plugin',
      pluginName: 'Test Plugin',
    };
    const index = store.initialAdd(plugin);

    store.deletePlugin(index);
    expect(store.pluginChanges.get(index)?.isDeleted).toBe(true);
  });

  it('computes getters correctly', () => {
    expect(store.getProjectInformationChanges).toEqual([]);

    const plugin: PluginModel = {
      id: 1,
      url: 'http://example.com',
      displayName: 'Test Plugin',
      pluginName: 'Test Plugin',
    };
    const index = store.initialAdd(plugin);

    expect(store.getPluginChanges.length).toBe(1);

    store.deletePlugin(index);
    expect(store.getPluginChanges.length).toBe(0);
  });

  it('computes canBeAdded correctly', () => {
    expect(store.getCanBeAdded).toBe(true);

    store.addEmptyField(1);
    expect(store.getCanBeAdded).toBe(false);

    store.removeEmptyField(1);
    expect(store.getCanBeAdded).toBe(true);

    const plugin: PluginModel = {
      id: 1,
      url: 'http://example.com',
      displayName: 'Test Plugin',
      pluginName: 'Test Plugin',
    };
    store.initialAdd(plugin);
    store.initialAdd(plugin); // Adding the same plugin to simulate a conflict
    store.checkForConflicts();

    expect(store.getCanBeAdded).toBe(false);
  });
});
