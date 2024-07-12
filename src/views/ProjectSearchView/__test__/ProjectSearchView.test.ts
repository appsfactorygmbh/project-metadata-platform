import { flushPromises, mount, VueWrapper } from '@vue/test-utils';
import ProjectSearchView from '../ProjectSearchView.vue';
import { describe, it, expect } from 'vitest';
import { createTestingPinia } from '@pinia/testing';
import { usePluginsStore, useProjectStore } from '@/store';
import { createPinia, setActivePinia } from 'pinia';
import _ from 'lodash';
import {
  pluginStoreSymbol,
  projectsStoreSymbol,
} from '@/store/injectionSymbols';
import type { ComponentPublicInstance } from 'vue';
import router from '@/router';
import type { ProjectModel } from '@/models/Project';

interface ProjectSearchViewInstance {
  paneWidth: number;
  paneHeight: number;
  handleRowClick: (project: ProjectModel) => void;
}

describe('ProjectSearchView.vue', () => {
  setActivePinia(createPinia());

  const generateWrapper = (pWidth: number) => {
    return mount(ProjectSearchView, {
      plugins: [
        createTestingPinia({
          stubActions: true,
        }),
      ],
      global: {
        provide: {
          [projectsStoreSymbol as symbol]: useProjectStore(),
          [pluginStoreSymbol as symbol]: usePluginsStore(),
        },
        plugins: [router],
      },
      propsData: {
        paneWidth: pWidth,
        paneHeight: 800,
      },
    });
  };

  const wrapper = generateWrapper(800) as VueWrapper<
    ComponentPublicInstance<ProjectSearchViewInstance>
  >;

  it('renders correctly with 4 columns', () => {
    expect(wrapper.findAll('.ant-table-column-sorters')).toHaveLength(4);
  });

  createTestingPinia({});
  const wrapper2 = generateWrapper(300);

  it('hides columns when the pane width is not large enough', async () => {
    await flushPromises();

    _.delay(
      () =>
        expect(wrapper2.findAll('.ant-table-column-sorters').length).toBe(2),
      1000,
    );
  });
});
