import { VueWrapper, mount } from '@vue/test-utils';
import { beforeEach, describe, expect, it } from 'vitest';
import FormModal from '../FormModal.vue';
import { type FormStore } from '@/components/Form/FormStore';

describe('FormModal.vue', () => {
  type FormModalInstance = {
    open: boolean;
    title: string;
    fromStore: FormStore;
    handleOk: () => Promise<void>;
    resetModal: () => void;
  };

  let wrapper: VueWrapper<FormModalInstance>;

  beforeEach(() => {
    // Mock formStore and title
    const mockFormStore: Partial<FormStore> = {
      submit: () => Promise.resolve(),
      resetFields: () => {},
    };
    const title = 'Test Modal';

    wrapper = mount(FormModal, {
      props: {
        formStore: mockFormStore as FormStore,
        title: title,
      },
    }) as unknown as VueWrapper<FormModalInstance>;
  });

  it('modal opens correctly', async () => {
    await wrapper.setProps({ open: true });
    await wrapper.vm.$nextTick();
    expect(wrapper.vm.open).toBe(true);
  });
});
