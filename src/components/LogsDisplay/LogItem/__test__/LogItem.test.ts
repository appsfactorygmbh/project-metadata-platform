import { describe, expect, it } from 'vitest';
import { mount } from '@vue/test-utils';
import LogItem from '../LogItem.vue';

const timeStamp = '2024-11-11T18:10:30+00:00';

describe('LogItem.vue', () => {
  it('renders last Item without line', () => {
    const wrapperIsNotLast = mount(LogItem, {
      props: {
        logMessage: 'This is a log message',
        timeStamp,
        isLast: false,
      },
    });
    expect(wrapperIsNotLast.findAll('.line')).toHaveLength(1);
    const wrapperIsLast = mount(LogItem, {
      props: {
        logMessage: 'This is a log message',
        timeStamp,
        isLast: true,
      },
    });
    expect(wrapperIsLast.findAll('.line')).toHaveLength(0);
  });
  it('renders the timestamp as local string', () => {
    const wrapper = mount(LogItem, {
      props: {
        logMessage: 'Timestamp test message',
        timeStamp,
        isLast: false,
      },
    });

    expect(wrapper.find('.timeStamp').text()).toBe('11.11.2024, 18:10:30');
  });

  it('always renders the circle', () => {
    const wrapperWithLine = mount(LogItem, {
      props: {
        logMessage: 'Circle test message with line',
        timeStamp,
        isLast: false,
      },
    });

    const wrapperWithoutLine = mount(LogItem, {
      props: {
        logMessage: 'Circle test message without line',
        timeStamp,
        isLast: true,
      },
    });

    expect(wrapperWithLine.find('.circle').exists()).toBe(true);
    expect(wrapperWithoutLine.find('.circle').exists()).toBe(true);
  });
});
