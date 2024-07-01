import { flushPromises, mount } from '@vue/test-utils';
import { SearchableTable } from '@/components/Table';
import { describe, it, expect } from 'vitest';
import { createTestingPinia } from '@pinia/testing';
import { useSearchStore } from '@/store';
import { createPinia, setActivePinia } from 'pinia';
import type { SearchableColumns } from '../SearchableTableTypes';

const testData = [
  {
    id: 1,
    projectName: 'C',
    clientName: 'A',
    businessUnit: 'A',
    teamNumber: 1,
  },
  {
    id: 2,
    projectName: 'A',
    clientName: 'B',
    businessUnit: 'B',
    teamNumber: 2,
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
    sortMethod: 'number',
    defaultSortOrder: 'ascend' as const,
    hidden: false,
  },
];

describe('SearchableTable.vue', () => {
  setActivePinia(createPinia());
  const searchStoreSymbol = Symbol('searchStoreSym');
  const searchStore = useSearchStore('test');

  const wrapper = mount(SearchableTable, {
    plugins: [
      createTestingPinia({
        stubActions: false,
      }),
    ],
    global: {
      provide: {
        [searchStoreSymbol as symbol]: searchStore,
      },
    },
    propsData: {
      searchStoreSymbol: searchStoreSymbol,
      paneHeight: 800,
      columns: testColumns,
      isLoading: false,
    },
  });

  it('renders correctly with 2 columns', () => {
    expect(wrapper.findAll('.ant-table-column-sorters')).toHaveLength(2);
    console.log(wrapper.findAll('.ant-table-column-sorters'));

    expect(wrapper.findAll('.ant-table-column-sorters')[1].text).toBe(
      'Project Name',
    );
    expect(wrapper.findAll('.ant-table-column-sorters')[2].text).toBe(
      'Team Number',
    );
  });

  it('gets the data from the store', async () => {
    searchStore.setBaseSet(testData);
    searchStore.setSearchQuery('');
    console.log(wrapper.html());

    expect(searchStore.getSearchResults).toEqual(testData);
  });

  createTestingPinia({});
  const searchStoreTest = useSearchStore('test');

  global.innerWidth = 1800;

  it('to wont use search store actions that it is not allowed to', async () => {
    await flushPromises();
    expect(searchStoreTest.setBaseSet).toHaveBeenCalledTimes(0);
    expect(searchStoreTest.setSearchQuery).toHaveBeenCalledTimes(0);
  });
});
