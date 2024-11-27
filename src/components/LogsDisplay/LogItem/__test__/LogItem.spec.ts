import { describe, expect, it } from 'vitest';
import { mount } from '@vue/test-utils';
import LogItem from '../LogItem.vue';

describe('CreateUserView.vue', () => {
  it('renders last Item without line', () => {
    const wrapperIsNotLast = mount(LogItem, {
      props: {
        logMessage: 'This is a log message',
        timeStamp: new Date('2024-11-11T18:10:30+00:00').toLocaleString(),
        isLast: false,
      },
    });
    expect(wrapperIsNotLast.findAll('.line')).toHaveLength(1);
    const wrapperIsLast = mount(LogItem, {
      props: {
        logMessage: 'This is a log message',
        timeStamp: new Date('2024-11-11T18:10:30+00:00').toLocaleString(),
        isLast: true,
      },
    });
    expect(wrapperIsLast.findAll('.line')).toHaveLength(0);
  });
});
