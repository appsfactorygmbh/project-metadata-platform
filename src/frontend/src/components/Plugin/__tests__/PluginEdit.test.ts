import { mount, VueWrapper } from '@vue/test-utils';
import { beforeEach, describe, expect, it } from 'vitest';
import { createTestingPinia } from '@pinia/testing';
import { projectEditStoreSymbol } from '../../../store/injectionSymbols';
import { useProjectEditStore } from '../../../store';
import PluginComponent from '../PluginComponent.vue';
import { createPinia, setActivePinia } from 'pinia';

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
  showFavicon: boolean;
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
      id: id,
      isLoading: isLoading,
      isEditing: isEditing,
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
    projectEditStore.resetPluginChanges();
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
