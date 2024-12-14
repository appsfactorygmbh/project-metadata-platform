import { VueWrapper, flushPromises, mount } from '@vue/test-utils';
import ProjectSearchView from '../ProjectSearchView.vue';
import { describe, expect, it } from 'vitest';
import { createTestingPinia } from '@pinia/testing';
import {
  useLocalLogStore,
  usePluginStore,
  useProjectStore,
  useSearchStore,
} from '@/store';
import { setActivePinia } from 'pinia';
import {
  localLogStoreSymbol,
  projectRoutingSymbol,
} from '@/store/injectionSymbols';
import type { ComponentPublicInstance } from 'vue';
import router from '@/router';
import type { ProjectModel } from '@/models/Project';
import { useProjectRouting } from '@/utils/hooks';

interface ProjectSearchViewInstance {
  paneWidth: number;
  paneHeight: number;
  handleRowClick: (project: ProjectModel) => void;
}

describe('ProjectSearchView.vue', () => {
  const generateWrapper = (pWidth: number) => {
    return mount(ProjectSearchView, {
      plugins: [
        createTestingPinia({
          stubActions: false,
        }),
      ],
      global: {
        provide: {
          [projectRoutingSymbol as symbol]: useProjectRouting(router),
          [localLogStoreSymbol as symbol]: useLocalLogStore(),
        },
        plugins: [router],
      },
      propsData: {
        paneWidth: pWidth,
        paneHeight: 800,
      },
    });
  };

  setActivePinia(createTestingPinia());

  beforeEach(() => {
    sessionStorage.clear();
  });

  it('renders correctly with 4 columns', () => {
    const wrapper = generateWrapper(800);
    expect(wrapper.findAll('.ant-table-column-sorters')).toHaveLength(4);
  });

  it('renders correctly with reset button', async () => {
    const wrapper = generateWrapper(800);
    expect(wrapper.find('[name="resetButton"]').exists()).toBe(true);
  });

  it('hides columns when the pane width is not large enough', async () => {
    createTestingPinia({});
    const wrapper = generateWrapper(300);
    await flushPromises();
    expect(wrapper.findAll('.ant-table-column-sorters')).toHaveLength(2);
  });

  it('adds a query when clicking on a project', async () => {
    await router.isReady();

    const wrapper = generateWrapper(700) as VueWrapper<
      ComponentPublicInstance<ProjectSearchViewInstance>
    >;
    const testProject: ProjectModel = {
      id: 200,
      projectName: 'test',
      clientName: 'test',
      businessUnit: 'test',
      teamNumber: 1,
      isArchived: false,
      slug: 'test-project',
    };

    wrapper.vm.handleRowClick(testProject);
    await flushPromises();

    expect(Number(router.currentRoute.value.query.projectId)).toBe(
      testProject.id,
    );
  });

  it('requests data with the project id given in the URL', async () => {
    await router.push({
      path: '/',
      query: { projectId: '300' },
    });
    await router.isReady();
    createTestingPinia({});

    generateWrapper(800);
    await flushPromises();
    await flushPromises();

    expect(useProjectStore().fetch).toHaveBeenCalledWith(300);
    expect(usePluginStore().fetch).toHaveBeenCalledWith(300);
  });

  it('sets the router query with the search query in the storage', async () => {
    await router.isReady();

    sessionStorage.setItem(
      'searchStorage',
      JSON.stringify({ searchQuery: 'test' }),
    );

    generateWrapper(800);
    await flushPromises();

    expect(useSearchStore('projects').setSearchQuery).toHaveBeenCalledWith(
      'test',
    );
  });

  it('sets the router query with the filters in the storage', async () => {
    await router.isReady();

    sessionStorage.setItem(
      'filterStorage',
      JSON.stringify({ projectName: 'test' }),
    );

    generateWrapper(800);
    await flushPromises();

    expect(router.currentRoute.value.query.projectName).toBe('test');
  });

  it('highlight button when search', async () => {
    await router.isReady();

    sessionStorage.setItem(
      'searchStorage',
      JSON.stringify({ searchQuery: 'test' }),
    );

    const wrapper = generateWrapper(700);
    await flushPromises();

    const button = wrapper.find('[name="resetButton"]').element as HTMLElement;
    expect(button.style.borderColor).toBe('#3e8ee2');
  });

  it('not highlight button when clear search', async () => {
    await router.isReady();

    sessionStorage.setItem(
      'searchStorage',
      JSON.stringify({ searchQuery: '' }),
    );

    const wrapper = generateWrapper(700);
    await flushPromises();

    const button = wrapper.find('[name="resetButton"]').element as HTMLElement;
    expect(button.style.borderColor).toBe('#d9d9d9');
  });

  it('highlight button when filter', async () => {
    await router.isReady();

    sessionStorage.setItem(
      'filterStorage',
      JSON.stringify({ projectName: 'test' }),
    );

    const wrapper = generateWrapper(700);
    await flushPromises();

    const button = wrapper.find('[name="resetButton"]').element as HTMLElement;
    expect(button.style.borderColor).toBe('#3e8ee2');
  });

  it('not highlight button when clear filter', async () => {
    await router.isReady();

    sessionStorage.setItem(
      'filterStorage',
      JSON.stringify({ projectName: '' }),
    );

    const wrapper = generateWrapper(700);
    await flushPromises();

    const button = wrapper.find('[name="resetButton"]').element as HTMLElement;
    expect(button.style.borderColor).toBe('#d9d9d9');
  });
});
