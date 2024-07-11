import FloatingButton from '../FloatingButton.vue';
import { QuestionOutlined } from '@ant-design/icons-vue';
import { mount } from '@vue/test-utils';
import { describe, expect, it } from 'vitest';
import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';

const testButtons: FloatButtonModel[] = [
  {
    name: '1',
    onClick: () => {
      console.log('test1');
    },
    icon: QuestionOutlined,
    status: 'activated',
    tooltip: 'test',
  },
  {
    name: '2',
    onClick: () => {},
    icon: QuestionOutlined,
    status: 'deactivated',
    tooltip: 'test',
  },
  {
    name: '3',
    onClick: () => {
      console.log('test2');
    },
    icon: QuestionOutlined,
    status: 'disabled',
    tooltip: 'test',
  },
  {
    name: '4',
    onClick: () => {
      console.log('test3');
    },
    icon: QuestionOutlined,
    tooltip: 'test',
  },
];

describe('FloatingButton.vue', () => {
  const consoleMock = vi
    .spyOn(console, 'log')
    .mockImplementation(() => undefined);

  afterEach(() => {
    consoleMock.mockReset();
  });

  const generateWrapper = (id: number) => {
    return mount(FloatingButton, {
      propsData: {
        button: testButtons[id],
      },
    });
  };

  it('renders the activated button correctly', async () => {
    const wrapper = generateWrapper(0);

    expect(wrapper.find('.anticon').attributes('aria-label')).toBe('question');
    await wrapper.find('.ant-float-btn').trigger('click');
    expect(consoleMock).toHaveBeenCalledOnce();
    expect(consoleMock).toHaveBeenCalledWith('test1');
  });

  it('doesnt render a deactivated button', () => {
    const wrapper = generateWrapper(1);

    expect(wrapper.findAll('.ant-float-btn')).toHaveLength(0);
  });

  it('renders a disabled button without function', async () => {
    const wrapper = generateWrapper(2);

    expect(wrapper.find('.anticon').attributes('aria-label')).toBe('question');
    await wrapper.find('.ant-float-btn').trigger('click');
    expect(consoleMock).toHaveBeenCalledTimes(0);
  });

  it('renders a activated button, if no status is set', async () => {
    const wrapper = generateWrapper(3);

    expect(wrapper.find('.anticon').attributes('aria-label')).toBe('question');
    await wrapper.find('.ant-float-btn').trigger('click');
    expect(consoleMock).toHaveBeenCalledOnce();
    expect(consoleMock).toHaveBeenCalledWith('test3');
  });
});
