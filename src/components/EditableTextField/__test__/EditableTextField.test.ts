import { VueWrapper, mount } from '@vue/test-utils';
import { EditableTextField } from '@/components/EditableTextField';
import { describe, expect, it } from 'vitest';
import {} from '@pinia/testing';
import router from '@/router';
interface EditableTextFieldInstance {
  value: string;
  label: string;
  isLoading: boolean;
}

describe('EditableTextField', () => {
  const generateWrapper = () => {
    return mount(EditableTextField, {
      global: {
        provide: {},
        plugins: [router],
      },
      propsData: {
        value: 'Maxmuster1',
        label: 'Username',
        isLoading: false,
      },
    });
  };

  it('renders the label and value correctly in display mode', () => {
    const wrapper = generateWrapper() as VueWrapper<
      ComponentPublicInstance<EditableTextFieldInstance>
    >;

    expect(wrapper.find('.label').text()).toBe(`Username:`);
    expect(wrapper.find('.text').text()).toBe('Maxmuster1');
    expect(wrapper.find('.edit').text()).toBe('Edit');
  });
});
