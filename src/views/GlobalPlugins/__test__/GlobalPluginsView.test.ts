import { createPinia, setActivePinia } from 'pinia';
import GlobalPluginsView from '../GlobalPluginsView.vue';
import { describe, it, expect } from 'vitest';
import { flushPromises, mount } from '@vue/test-utils';
import { createTestingPinia } from '@pinia/testing';

const testData = [
  {
    id: 0,
    name: 'Plugin 1',
    archieved: true,
  },
  {
    id: 1,
    name: 'Plugin 2',
    archieved: false,
  },
];

describe('GlobalPluginsView.vue', () => {
  setActivePinia(createPinia());

  it('renders the fetched Plugins, but not the archieved ones', async () => {
    const wrapper = mount(GlobalPluginsView, {
      plugins: [
        createTestingPinia({
          stubActions: false,
          initialState: {
            plugin: {
              globalPlugins: testData,
            },
          },
        }),
      ],
      propsData: {
        isTest: true,
      },
    });

    await flushPromises();
    expect(wrapper.findAll('.ant-list-item')).toHaveLength(1);
    expect(wrapper.find('.ant-list-item').text()).toBe('Plugin 2');
    expect(wrapper.findAll('.ant-btn')).toHaveLength(2);
  });
});
