import { VueWrapper, mount } from '@vue/test-utils';
import { EditableTextField } from '@/components/EditableTextField';
import { describe, expect, it, vi } from 'vitest';
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
        plugins: [router],
      },
      props: {
        value: 'Maxmuster1',
        label: 'Username',
        isEditingKey: 'isEditingPassword',
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

  it('add isEditingPassword to URL when click edit', () => {
    const wrapper = generateWrapper() as VueWrapper<
      ComponentPublicInstance<EditableTextFieldInstance>
    >;
    const routerPushSpy = vi.spyOn(router, 'push');
    wrapper.find('button').trigger('click');

    const url = routerPushSpy.mock.calls[0][0];
    const queryObject = (url as { query: Record<string, string> }).query;
    expect(queryObject).toEqual({ isEditingPassword: 'true' });
  });
});
