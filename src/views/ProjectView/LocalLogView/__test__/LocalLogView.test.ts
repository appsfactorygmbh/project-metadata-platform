import { mount } from '@vue/test-utils';
import LocalLogView from '../LocalLogView.vue';
import LogTimeline from '@/components/LogsDisplay/LogTimeline/LogTimeline.vue';
import { describe, expect, it } from 'vitest';
import { createTestingPinia } from '@pinia/testing';
import { useLocalLogStore } from '@/store';
import { createPinia, setActivePinia } from 'pinia';
import { localLogStoreSymbol } from '@/store/injectionSymbols';
import router from '@/router';
setActivePinia(createPinia());
const logsStore = useLocalLogStore();

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

describe('LocalLogView.vue', () => {
  it('not show when there is no log data', () => {
    logsStore.setLocalLogs([]);
    logsStore.setIsLoadingLocalLog(false);
    const wrapper = generateWrapper();
    expect(wrapper.find('.localLog').exists()).toBe(false);
    expect(wrapper.find('.cardContainer').exists()).toBe(false);
  });

  it('show when there is log data', () => {
    logsStore.setLocalLogs(logsData);
    logsStore.setIsLoadingLocalLog(false);
    const wrapper = generateWrapper();
    expect(wrapper.find('.localLog').exists()).toBe(true);
    expect(wrapper.find('.cardContainer').exists()).toBe(true);
  });

  it('load the correct amount of log data', () => {
    const wrapper = generateWrapper();
    const logTimelineComponent = wrapper.findComponent(LogTimeline);
    expect(logTimelineComponent.props('logEntries')).toHaveLength(
      logsData.length,
    );
  });
});
