import { flushPromises, mount } from '@vue/test-utils';
import { SearchableTable } from '@/components/Table';
import { describe, it, expect } from 'vitest';
import { Button, Input } from 'ant-design-vue';
import { createTestingPinia } from '@pinia/testing';
import { useProjectStore, useSearchStore } from '@/store';
import { createPinia, setActivePinia } from 'pinia';
import _ from 'lodash';
import { projectsStoreSymbol } from '@/store/injectionSymbols';
import router from '@/router';

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

describe('SearchableTable.vue', () => {
  setActivePinia(createPinia());
  const searchStoreSymbol = Symbol('searchStoreSym');
  const searchStore = useSearchStore('test');

  const wrapper = mount(SearchableTable, {
    plugins: [
      createTestingPinia({
        stubActions: false,
        initialState: {
          project: {
            project: testData,
          },
        },
      }),
    ],
    global: {
      provide: {
        [searchStoreSymbol as symbol]: searchStore,
        [projectsStoreSymbol as symbol]: useProjectStore(),
      },
      plugins: [router],
    },
    propsData: {
      searchStoreSymbol: searchStoreSymbol,
      paneWidth: 800,
      paneHeight: 800,
    },
  });

  it('renders correctly with 4 columns', () => {
    expect(wrapper.findAll('.ant-table-column-sorters')).toHaveLength(4);
  });

  it('gets the data from the store', async () => {
    searchStore.setBaseSet(testData);
    searchStore.setSearchQuery('');

    expect(searchStore.getSearchResults).toEqual(testData);
  });

  it('shows the data entries in alphabetical order', async () => {
    expect(
      wrapper.findAll('.ant-table-row')[0]?.find('.ant-table-cell').text(),
    ).toBe('A');
    expect(
      wrapper.findAll('.ant-table-row')[1]?.find('.ant-table-cell').text(),
    ).toBe('C');
  });

  it('filters the table when using the search function', async () => {
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

  createTestingPinia({});
  const searchStoreSymbolTest = Symbol('searchStoreSym');
  const searchStoreTest = useSearchStore('test');

  global.innerWidth = 960;

  const wrapper2 = mount(SearchableTable, {
    plugins: [
      createTestingPinia({
        stubActions: false,
        initialState: {
          project: {
            projects: testData,
          },
        },
      }),
    ],
    global: {
      provide: {
        [searchStoreSymbolTest as symbol]: searchStoreTest,
        [projectsStoreSymbol as symbol]: useProjectStore(),
      },
      plugins: [router],
    },
    propsData: {
      searchStoreSymbol: searchStoreSymbolTest,
      paneWidth: 300,
      paneHeight: 800,
    },
  });

  it('to wont use search store actions that it is not allowed to', async () => {
    await flushPromises();
    expect(searchStoreTest.setBaseSet).toHaveBeenCalledTimes(0);
    expect(searchStoreTest.setSearchQuery).toHaveBeenCalledTimes(0);
  });

  it('hides columns when the pane width is not large enough', async () => {
    await flushPromises();

    _.delay(
      () =>
        expect(wrapper2.findAll('.ant-table-column-sorters').length).toBe(2),
      500,
    );
  });
});
