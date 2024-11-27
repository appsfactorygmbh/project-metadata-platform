import { mount } from '@vue/test-utils';
import LocalLogView from '../LocalLogView.vue';
import { describe, expect, it } from 'vitest';
import { createTestingPinia } from '@pinia/testing';
import { useLocalLogStore } from '@/store';
import { createPinia, setActivePinia } from 'pinia';
import { localLogStoreSymbol } from '@/store/injectionSymbols';
import router from '@/router';

const logsData = [
  {
    timestamp: '2024-11-11T18:10:30+00:00',
    logMessage: 'Mustermann edited project name from "DB Apps" to "DB App"',
  },
  {
    timestamp: '2024-12-15T18:10:30+00:00',
    logMessage: 'Mustermann edited team number from 2 to 3',
  },
];

describe('UserListView.vue', () => {
  setActivePinia(createPinia());
  const logsStore = useLocalLogStore();

  const generateWrapper = () => {
    return mount(LocalLogView, {
      plugins: [
        createTestingPinia({
          stubActions: false,
        }),
      ],
      global: {
        provide: {
          [localLogStoreSymbol as symbol]: logsStore,
        },
        plugins: [router],
      },
    });
  };

  it('show when there is log data', () => {
    logsStore.setLocalLogs(logsData);
    logsStore.setIsLoadingLocalLog(false);
    const wrapper = generateWrapper();
    expect(wrapper.find('.localLog').exists()).toBe(true);
    expect(wrapper.find('.cardContainer').exists()).toBe(true);

  });

  it('not show when there is no log data', () => {
    logsStore.setLocalLogs([]);
    logsStore.setIsLoadingLocalLog(false);
    const wrapper = generateWrapper();
    expect(wrapper.find('.localLog').exists()).toBe(false);
    expect(wrapper.find('.cardContainer').exists()).toBe(false);

  });
});
