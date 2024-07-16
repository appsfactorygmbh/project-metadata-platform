import { flushPromises, mount } from '@vue/test-utils';
import ProjectSearchView from '../ProjectSearchView.vue';
import { describe, it, expect } from 'vitest';
import { createTestingPinia } from '@pinia/testing';
import { useProjectStore } from '@/store';
import { createPinia, setActivePinia } from 'pinia';
import _ from 'lodash';
import { projectsStoreSymbol } from '@/store/injectionSymbols';

describe('ProjectSearchView.vue', () => {
  setActivePinia(createPinia());

  const generateWrapper = (pWidth: number) => {
    return mount(ProjectSearchView, {
      plugins: [
        createTestingPinia({
          stubActions: false,
        }),
      ],
      global: {
        provide: {
          [projectsStoreSymbol as symbol]: useProjectStore(),
        },
      },
      propsData: {
        paneWidth: pWidth,
        paneHeight: 800,
      },
    });
  };

  const wrapper = generateWrapper(800);

  it('renders correctly with 4 columns', () => {
    expect(wrapper.findAll('.ant-table-column-sorters')).toHaveLength(4);
  });

  it('renders correctly with reset button', async () => {
    expect(wrapper.find('.reset').exists()).toBe(true);
  });

  createTestingPinia({});
  const wrapper2 = generateWrapper(300);

  it('hides columns when the pane width is not large enough', async () => {
    await flushPromises();

    _.delay(
      () =>
        expect(wrapper2.findAll('.ant-table-column-sorters').length).toBe(2),
      500,
    );
  });
});
