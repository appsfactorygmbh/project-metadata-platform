import type { LogEntryModel } from '@/models/Log/LogEntryModel';
import { createTestingPinia } from '@pinia/testing';
import { setActivePinia } from 'pinia';
import { describe } from 'vitest';
import { VueWrapper, mount } from '@vue/test-utils';
import GlobalLogsView from '../GlobalLogsView.vue';
import { logsStoreSymbol } from '@/store/injectionSymbols';
import { useLogsStore } from '@/store';

describe('GlobalLogsView.vue', () => {
  setActivePinia(createTestingPinia({ stubActions: false }));

  //   it('renders correctly', () => {
  //     const testData: LogEntryModel[] = [
  //       {
  //         logMessage: 'This is a log',
  //         timeStamp: '1/12/20',
  //       },
  //       {
  //         logMessage: 'This is another log',
  //         timeStamp: '2/12/20',
  //       },
  //       {
  //         logMessage: 'This is yet another log',
  //         timeStamp: '3/12/20',
  //       },
  //     ];
  //     const wrapper: VueWrapper = mount(GlobalLogsView, {
  //       plugins: [
  //         createTestingPinia({
  //           stubActions: false,
  //           initialState: {
  //             logs: {
  //               globalLogsEntries: testData,
  //             },
  //           },
  //         }),
  //       ],
  //       global: {
  //         provide: {
  //           [logsStoreSymbol as symbol]: useLogsStore(),
  //         },
  //       },
  //     });
  //     expect(wrapper.find('.ant-timeline-item')).toE;
  //   });
  it('fetches logs on mount', async () => {
    const logsStore = useLogsStore();
    mount(GlobalLogsView, {
      global: {
        provide: {
          [logsStoreSymbol as symbol]: useLogsStore(),
        },
      },
    });
    expect(logsStore.fetchGlobalLogs).toHaveBeenCalledTimes(1);
  });
  it('refetches on search input', async () => {
    const logsStore = useLogsStore();
    const wrapper = mount(GlobalLogsView, {
      global: {
        provide: {
          [logsStoreSymbol as symbol]: useLogsStore(),
        },
      },
    });
    await wrapper.findComponent({ name: 'AInput' }).setValue('created User');
    setTimeout(() => {
      console.log(wrapper);
    }, 500);
    expect(logsStore.fetchGlobalLogs).toHaveBeenCalledWith({
      searchParam: 'created user',
    });
  });
});
