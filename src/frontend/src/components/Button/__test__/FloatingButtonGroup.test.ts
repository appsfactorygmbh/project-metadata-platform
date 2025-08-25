import FloatingButtonGroup from '../FloatingButtonGroup.vue';
import { QuestionOutlined } from '@ant-design/icons-vue';
import { mount } from '@vue/test-utils';
import { describe, expect, it } from 'vitest';
import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';

const testButtons: FloatButtonModel[] = [
  {
    name: '1',
    onClick: () => {
      console.log('test');
    },
    icon: QuestionOutlined,
    size: 'middle',
    status: 'activated',
    tooltip: 'test',
  },
  {
    name: '2',
    onClick: () => {},
    icon: QuestionOutlined,
    size: 'middle',
    status: 'deactivated',
    tooltip: 'test',
  },
  {
    name: '3',
    onClick: () => {},
    icon: QuestionOutlined,
    size: 'middle',
    status: 'disabled',
    tooltip: 'test',
  },
];

describe('FloatingButtonGroup.vue', () => {
  const consoleMock = vi
    .spyOn(console, 'log')
    .mockImplementation(() => undefined);

  afterAll(() => {
    consoleMock.mockReset();
  });

  it('shows the buttons correctly', async () => {
    const wrapper = mount(FloatingButtonGroup, {
      propsData: {
        buttons: testButtons,
      },
    });

    expect(wrapper.findAll('.ant-float-btn')).toHaveLength(2);
    expect(wrapper.find('.anticon').attributes('aria-label')).toBe('question');
    await wrapper.find('.ant-float-btn').trigger('click');
    expect(consoleMock).toHaveBeenCalledWith('test');
  });
});
