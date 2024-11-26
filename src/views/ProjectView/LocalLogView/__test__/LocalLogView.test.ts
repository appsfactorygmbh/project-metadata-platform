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

  const generateWrapper = () => {
    return mount(LocalLogView, {
      plugins: [
        createTestingPinia({
          stubActions: true,
          initialState: {
            localLogs: {
              localLog: logsData,
            },
          },
        }),
      ],
      global: {
        provide: {
          [localLogStoreSymbol as symbol]: useLocalLogStore(),
        },
        plugins: [router],
      },
    });
  };

  it('renders correctly', () => {
    const wrapper = generateWrapper();
    expect(wrapper.find('.localLog').exists()).toBe(true);
    expect(wrapper.find('.timeline').exists()).toBe(true);
    const menuItems = wrapper.findAll('.log');
    expect(menuItems.length).toBe(logsData.length);

    menuItems.forEach((itemWrapper, index) => {
      expect(itemWrapper.text()).toContain(logsData[index].logMessage);
    });
  });
});
