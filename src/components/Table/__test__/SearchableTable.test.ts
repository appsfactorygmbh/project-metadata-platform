import { flushPromises, mount } from '@vue/test-utils';
import { SearchableTable } from '@/components/Table';
import { describe, it, expect, vi } from 'vitest';
import { createPinia, setActivePinia } from 'pinia';

setActivePinia(createPinia());

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

describe('tableComponent.vue', () => {
  const mockResponse = {
    ok: true,
    statusText: 'Ok',
    json: async () => testData,
  } as Response;
  globalThis.fetch = vi.fn().mockResolvedValue(mockResponse);

  const wrapper = mount(SearchableTable, {
    props: {
      paneWidth: 800,
      paneHeight: 800,
      isTest: true,
    },
  });
  it('renders correctly with 4 columns', () => {
    expect(wrapper.findAll('.ant-table-column-sorters')).toHaveLength(4);
  });
  it('shows the data entries in alphabetical order', async () => {
    expect(
      wrapper.findAll('.ant-table-row')[0].find('.ant-table-cell').text(),
    ).toBe('A');
    expect(
      wrapper.findAll('.ant-table-row')[1].find('.ant-table-cell').text(),
    ).toBe('C');
  });
  it('hides columns when the pane width is not large enough', async () => {
    const wrapper2 = mount(SearchableTable, {
      props: {
        paneWidth: 300,
        paneHeight: 800,
        isTest: true,
      },
    });
    await flushPromises();

    expect(wrapper2.findAll('.ant-table-column-sorters').length).toBe(2);
  });
});
