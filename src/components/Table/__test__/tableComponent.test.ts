import { flushPromises, mount } from '@vue/test-utils';
import Table from '../tableComponent.vue';
import { describe, it, expect } from 'vitest';

const wrapper = mount(Table, {
  props: {
    paneWidth: 800,
    paneHeight: 800,
    isTest: true,
  },
});

describe('tableComponent.vue', () => {
  it('renders correctly with 4 columns', () => {
    expect(wrapper.findAll('.ant-table-column-sorters')).toHaveLength(4);
  });
  it('shows the data entries in alphabetical order', () => {
    expect(
      wrapper.findAll('.ant-table-row')[0].find('.ant-table-cell').text(),
    ).toBe('A');
    expect(
      wrapper.findAll('.ant-table-row')[1].find('.ant-table-cell').text(),
    ).toBe('B');
    expect(
      wrapper.findAll('.ant-table-row')[2].find('.ant-table-cell').text(),
    ).toBe('C');
  });
  it('hides columns when the pane width is not large enough', async () => {
    const wrapper2 = mount(Table, {
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
