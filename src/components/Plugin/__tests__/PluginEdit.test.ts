import { mount, VueWrapper } from '@vue/test-utils';
import { it, describe, expect, beforeEach } from 'vitest';
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
  hide?: boolean;
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
    const wrapper = generateWrapper(
      'Test Plugin',
      'https://example.com/examplePath',
      'test instance',
      false,
      true,
      100,
    );
    const projectEditStore = useProjectEditStore();

    const inputs = wrapper.findAll('input');
    const displayNameInput = inputs[0];
    const urlInput = inputs[1];

    await displayNameInput.setValue('new display name');
    expect(projectEditStore.updatePluginChanges).toHaveBeenCalledWith(
      '100https://example.com/examplePath',
      {
        displayName: 'new display name',
        url: 'https://example.com/examplePath',
        id: 100,
        pluginName: 'Test Plugin',
      },
    );
    await urlInput.setValue('https://example.com/newPath');
    expect(projectEditStore.updatePluginChanges).toHaveBeenCalledWith(
      '100https://example.com/examplePath',
      {
        displayName: 'new display name',
        url: 'https://example.com/newPath',
        id: 100,
        pluginName: 'Test Plugin',
      },
    );
  });

  it('hides the plugin correctly when DeleteOutlined icon is clicked', async () => {
    const wrapper = generateWrapper(
      'Test Plugin',
      'https://example.com/examplePath',
      'test instance',
      false,
      true,
      100,
    );

    const projectEditStore = useProjectEditStore();

    const deleteIcon = wrapper.findComponent({ name: 'DeleteOutlined' });
    await deleteIcon.trigger('click');

    expect(wrapper.vm.hide).toBe(true);
    expect(projectEditStore.deletePlugin).toHaveBeenCalledWith(
      100,
      'https://example.com/examplePath',
    );
  });
});

describe('projectEditStore', () => {
  it('initialAdd adds a plugin to the store', () => {
    const projectEditStore = useProjectEditStore();
    projectEditStore.initialAdd({
      displayName: 'test instance',
      url: 'https://example.com/examplePath',
      id: 100,
      pluginName: 'Test Plugin',
    });
    expect(projectEditStore.getPluginChanges.length).toEqual(1);
  });
  it('overrides the plugin in the store when updatePluginChanges is called', () => {
    const examplePlugin = {
      displayName: 'test instance',
      url: 'https://example.com/examplePath',
      id: 100,
      pluginName: 'Test Plugin',
    };

    const projectEditStore = useProjectEditStore();
    projectEditStore.initialAdd(examplePlugin);
    projectEditStore.updatePluginChanges(
      examplePlugin.id.toString() + examplePlugin.url,
      {
        displayName: 'new display name',
        url: 'https://example.com/examplePath',
        id: 100,
        pluginName: 'Test Plugin',
      },
    );
    expect(projectEditStore.getPluginChanges[0].displayName).toEqual(
      'new display name',
    );
  });
  it('checks if the input is correct and valid', () => {
    const examplePlugin1 = {
      displayName: 'test instance',
      url: 'https://example.com/examplePath',
      id: 100,
      pluginName: 'Test Plugin',
    };
    const examplePlugin2 = {
      displayName: 'other test instance',
      url: 'https://example.com/otherPath',
      id: 200,
      pluginName: 'Test Plugin',
    };
    const projectEditStore = useProjectEditStore();
    projectEditStore.initialAdd(examplePlugin1);
    projectEditStore.initialAdd(examplePlugin2);

    // throws error if urls are duplicated
    const result = projectEditStore.isCorrectUrlInput(
      {
        displayName: 'other test instance',
        url: 'https://example.com/otherPath',
        id: 100,
        pluginName: 'Test Plugin',
      }, "100https://example.com/otherPlugin"
    );
    expect(result).toBe(false);

    //doesn't throw error if it is the same plugin
    const result2 = projectEditStore.isCorrectUrlInput(
      {
        displayName: 'other test instance',
        url: 'https://example.com/otherPath',
        id: 200,
        pluginName: 'Test Plugin',
      }, "100https://example.com/otherPath"
    );
    expect(result2).toBe(true);
  });
});
