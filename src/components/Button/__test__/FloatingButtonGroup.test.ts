import FloatingButtonGroup from '../FloatingButtonGroup.vue';
import { QuestionOutlined } from '@ant-design/icons-vue';
import { mount } from '@vue/test-utils';
import { describe, expect, it } from 'vitest';
import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';

const testButtons: FloatButtonModel[] = [
  {
    name: '1',
    onClick: () => {},
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
];

describe('FloatingButtonGroup.vue', () => {
  it('shows the buttons correctly', () => {
    const wrapper = mount(FloatingButtonGroup, {
      propsData: {
        buttons: testButtons,
      },
    });

    expect(wrapper.findAll('.ant-float-btn')).toHaveLength(1);
    expect(wrapper.find('.anticon').attributes('aria-label')).toBe('question');
  });
});
