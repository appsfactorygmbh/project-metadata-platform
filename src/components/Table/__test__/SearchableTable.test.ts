import { enableAutoUnmount, flushPromises, mount } from '@vue/test-utils';
import { SearchableTable } from '@/components/Table';
import { describe, expect, it } from 'vitest';
import { createTestingPinia } from '@pinia/testing';
import { useSearchStore } from '@/store';
import { createPinia, setActivePinia } from 'pinia';
import type { SearchableColumns } from '../SearchableTableTypes';
import { Button, Input } from 'ant-design-vue';
import router from '@/router';
import type { ProjectModel } from '@/models/Project';

const testData: ProjectModel[] = [
  {
    id: 1,
    projectName: 'C',
    clientName: 'A',
    businessUnit: 'A',
    teamNumber: 1,
    isArchived: false,
    slug: 'c',
    company: 'test',
    ismsLevel: 'NORMAL',
  },
  {
    id: 2,
    projectName: 'A',
    clientName: 'B',
    businessUnit: 'B',
    teamNumber: 2,
    isArchived: false,
    slug: 'a',
    company: 'test',
    ismsLevel: 'NORMAL',
  },
];

const testColumns: SearchableColumns = [
  {
    title: 'Project Name',
    dataIndex: 'projectName',
    key: 'projectName',
    searchable: true,
    ellipsis: true,
    align: 'center' as const,
    sortMethod: 'string',
    defaultSortOrder: 'ascend' as const,
  },
  {
    title: 'Team Number',
    dataIndex: 'teamNumber',
    key: 'teamNumber',
    ellipsis: true,
    align: 'center' as const,
  },
];

describe('SearchableTable.vue', () => {
  enableAutoUnmount(afterEach);
  setActivePinia(createPinia());
  const searchStoreSymbol = Symbol('searchStoreSym');
  const searchStore = useSearchStore('test');

  beforeEach(() => {
    sessionStorage.clear();
  });

  const generateWrapper = () => {
    return mount(SearchableTable, {
      plugins: [
        createTestingPinia({
          stubActions: false,
        }),
      ],
      global: {
        provide: {
          [searchStoreSymbol as symbol]: searchStore,
        },
        plugins: [router],
      },
      propsData: {
        searchStoreSymbol: searchStoreSymbol,
        paneHeight: 800,
        columns: testColumns,
        isLoading: false,
      },
    });
  };

  const loadData = async () => {
    searchStore.setBaseSet(testData);
    searchStore.setSearchQuery('');
    await flushPromises();
  };

  it('renders correctly with 2 columns', () => {
    const wrapper = generateWrapper();

    expect(wrapper.findAll('.ant-table-column-title')).toHaveLength(2);
  });

  it('show 1 sorter and 1 search filter', () => {
    const wrapper = generateWrapper();

    expect(wrapper.findAll('.ant-table-column-sorter')).toHaveLength(1);
    expect(wrapper.findAll('.ant-table-filter-column')).toHaveLength(1);
  });

  it('gets the data from the store', async () => {
    await loadData();

    expect(searchStore.getSearchResults).toEqual(testData);
  });

  it('shows the data entries in alphabetical order', async () => {
    const wrapper = generateWrapper();
    await loadData();

    expect(
      wrapper.findAll('.ant-table-row')[0]?.find('.ant-table-cell').text(),
    ).toBe('A');
    expect(
      wrapper.findAll('.ant-table-row')[1]?.find('.ant-table-cell').text(),
    ).toBe('C');
  });

  it('changes the order when clicking the sorter field', async () => {
    const wrapper = generateWrapper();
    await loadData();
    await wrapper.find('.ant-table-column-sorters').trigger('click');

    expect(
      wrapper.findAll('.ant-table-row')[0]?.find('.ant-table-cell').text(),
    ).toBe('C');
    expect(
      wrapper.findAll('.ant-table-row')[1]?.find('.ant-table-cell').text(),
    ).toBe('A');
  });

  it('filters the table when using the search function', async () => {
    const wrapper = generateWrapper();
    await loadData();

    expect(wrapper.findAll('.ant-table-row')).toHaveLength(2);
    await wrapper.find('.ant-table-filter-trigger').trigger('click');

    const searchInput = wrapper.getComponent(Input);
    const searchButton = wrapper.getComponent(Button);

    await searchInput.get('.ant-input').setValue('A');
    await searchButton.trigger('click');

    expect(wrapper.findAll('.ant-table-row')).toHaveLength(1);
    expect(wrapper.find('.ant-table-row').find('.ant-table-cell').text()).toBe(
      'A',
    );
  });

  it('stores the filters in the filterStorage', async () => {
    const wrapper = generateWrapper();
    await loadData();

    expect(wrapper.findAll('.ant-table-row')).toHaveLength(2);
    await wrapper.find('.ant-table-filter-trigger').trigger('click');

    const searchInput = wrapper.getComponent(Input);
    const searchButton = wrapper.getComponent(Button);

    await searchInput.get('.ant-input').setValue('A');
    await searchButton.trigger('click');

    const filterStorage = JSON.parse(sessionStorage.getItem('filterStorage')!);

    expect(filterStorage).toEqual({ projectName: 'A' });
  });

  it('reset the table when using the searchStore reset ', async () => {
    const wrapper = generateWrapper();
    await loadData();

    expect(wrapper.findAll('.ant-table-row')).toHaveLength(2);
    await wrapper.find('.ant-table-filter-trigger').trigger('click');

    const searchInput = wrapper.getComponent(Input);
    const searchButton = wrapper.getComponent(Button);

    await searchInput.get('.ant-input').setValue('A');
    await searchButton.trigger('click');

    expect(wrapper.findAll('.ant-table-row')).toHaveLength(1);
    await wrapper.find('.ant-table-filter-trigger').trigger('click');

    searchStore.reset();
    await flushPromises();
    expect(wrapper.findAll('.ant-table-row')).toHaveLength(2);
    expect(
      wrapper.findAll('.ant-table-row')[0]?.find('.ant-table-cell').text(),
    ).toBe('A');
    expect(
      wrapper.findAll('.ant-table-row')[1]?.find('.ant-table-cell').text(),
    ).toBe('C');
  });

  it('adds a query when searching in a column and deleting it when pressing reset', async () => {
    const wrapper = generateWrapper();

    await wrapper.find('.ant-table-filter-trigger').trigger('click');
    const searchInput = wrapper.getComponent(Input);
    const searchButton = wrapper.getComponent(Button);
    const resetButton = wrapper.findAllComponents(Button)[1];

    await searchInput.get('.ant-input').setValue('Test1');
    await searchButton.trigger('click');

    await flushPromises();
    expect(router.currentRoute.value.query.projectName).toBe('Test1');

    await resetButton.trigger('click');
    await flushPromises();
    expect(router.currentRoute.value.query.projectName).toBe(undefined);
  });

  it('filters columns, when providing inital query', async () => {
    await router.push({
      path: '/',
      query: { projectName: 'C' },
    });
    await router.isReady();

    const wrapper = generateWrapper();

    await wrapper.find('.ant-table-filter-trigger').trigger('click');
    const searchInput = wrapper.getComponent(Input);

    expect(searchInput.element.value).toBe('C');
    expect(wrapper.findAll('.ant-table-row')).toHaveLength(1);
    expect(wrapper.find('.ant-table-row').find('.ant-table-cell').text()).toBe(
      'C',
    );
  });

  createTestingPinia({});
  const searchStoreTest = useSearchStore('test');

  it('to wont use search store actions that it is not allowed to', async () => {
    await flushPromises();
    expect(searchStoreTest.setBaseSet).toHaveBeenCalledTimes(0);
    expect(searchStoreTest.setSearchQuery).toHaveBeenCalledTimes(0);
  });
});
