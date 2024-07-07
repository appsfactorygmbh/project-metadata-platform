import { describe, it, expect } from 'vitest';
import { mount, VueWrapper } from '@vue/test-utils';
import PluginComponent from '@/components/Plugin/PluginComponent.vue';
import type { PluginModel } from '@/models/Plugin';
import type { ComponentPublicInstance } from 'vue';

interface PluginComponentInstance {
  pluginName: string;
  url: string;
  displayName: string;
  id: number;
  isLoading: boolean;
  isEditing: boolean;
  modelValue: PluginModel[];
}

const initialPlugin = {
  id: 100,
  pluginName: 'Test Plugin',
  url: 'https://test.com',
  displayName: 'Test',
  isLoading: false,
  isEditing: true,
};
const initialPlugin2 = {
  id: 200,
  pluginName: 'Test Plugin',
  url: 'https://test.com',
  displayName: 'Test',
  isLoading: false,
  isEditing: true,
};

const generateWrapper = (
  name: string,
  url: string,
  displayName: string,
  isLoading: boolean,
  isEditing: boolean,
  id: number,
  modelValue: PluginModel[],
): VueWrapper<ComponentPublicInstance<PluginComponentInstance>> => {
  return mount(PluginComponent, {
    props: {
      pluginName: name,
      url: url,
      displayName: displayName,
      isLoading: isLoading,
      isEditing: isEditing,
      id: id,
      modelValue: modelValue,
    },
  }) as VueWrapper<ComponentPublicInstance<PluginComponentInstance>>;
};

describe('PluginComponent', () => {
  it('renders the edit Component correctly', async () => {
    const wrapper = generateWrapper(
      'Test Plugin',
      'https://example.com/examplePath',
      'Test Plugin Instance 1',
      false,
      true,
      100,
      [initialPlugin, initialPlugin2],
    );

    const card = wrapper.findComponent({ name: 'a-card' });
    expect(card.exists()).toBe(true);

    // Check the plugin name
    const pluginName = wrapper.find('h3');
    expect(pluginName.text()).toBe('Test Plugin');

    const inputs = wrapper.findAll('.inputField');
    expect(inputs.length).toBe(2);

    const deleteIcon = wrapper.findComponent({ name: 'DeleteOutlined' });
    expect(deleteIcon.exists()).toBe(true);
  });

  it('deletes the plugin from the model when delete icon is clicked', async () => {
    const wrapper = generateWrapper(
      'Test Plugin',
      'https://example.com/examplePath',
      'Test Plugin Instance 1',
      false,
      true,
      100,
      [initialPlugin, initialPlugin2],
    );

    // deletes initialPlugin (first Plugin)
    const deleteIcon = wrapper.findComponent({ name: 'DeleteOutlined' });
    await deleteIcon.trigger('click');

    const emitted = wrapper.emitted('update:modelValue');
    expect(emitted).toBeTruthy();
    if (emitted) {
      const lastEmittedValue = emitted[emitted.length - 1][0] as PluginModel[];
      expect(
        lastEmittedValue.find((plugin: PluginModel) => plugin.id === 100),
      ).toBeFalsy();
    }
  });
});
