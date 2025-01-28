import { mount } from '@vue/test-utils';
import { describe, expect, it } from 'vitest';
import dayjs, { Dayjs } from 'dayjs';
import LogTimeFilter from '@/components/LogsDisplay/LogTimeFilter/LogTimeFilter.vue';

const logsData = [
  { logMessage: 'Log 1', timestamp: '2025-01-10T11:00:00+00:00' },
  { logMessage: 'Log 2', timestamp: '2025-01-11T12:00:00+00:00' },
  { logMessage: 'Log 3', timestamp: '2025-01-12T13:00:00+00:00' },
];

interface LogTimeFilterInterface {
  handleChange: (value: [Dayjs, Dayjs] | undefined) => void;
}

describe('LogTimeFilter', () => {
  it('filters logs based on the selected date range', async () => {
    const wrapper = mount(LogTimeFilter, {
      props: {
        logEntries: logsData,
      },
    });
    const vm = wrapper.vm as unknown as LogTimeFilterInterface;

    const range: [Dayjs, Dayjs] = [dayjs('2025-01-10'), dayjs('2025-01-11')];
    vm.handleChange(range);

    const filteredLogs = logsData.filter((entry) => {
      const logDate = dayjs(entry.timestamp).startOf('day');
      return logDate.isBetween(
        range[0].startOf('day'),
        range[1].endOf('day'),
        null,
        '[]',
      );
    });

    expect(wrapper.emitted('update:logs')?.[0]).toStrictEqual([filteredLogs]);
  });

  it('return all logs when not select date range', async () => {
    const wrapper = mount(LogTimeFilter, {
      props: {
        logEntries: logsData,
      },
    });
    const vm = wrapper.vm as unknown as LogTimeFilterInterface;
    const range = undefined;
    vm.handleChange(range);

    expect(wrapper.emitted('update:logs')?.[0]).toStrictEqual([logsData]);
  });
});
