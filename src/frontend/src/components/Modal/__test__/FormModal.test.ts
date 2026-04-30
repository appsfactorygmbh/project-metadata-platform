import { VueWrapper, mount } from '@vue/test-utils';
import { beforeEach, describe, expect, it } from 'vitest';
import FormModal from '../FormModal.vue';
import { type FormStore } from '@/components/Form/FormStore';

describe('FormModal.vue', () => {
  let wrapper: VueWrapper<InstanceType<typeof FormModal>>;

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
    });
  });

  it('modal opens correctly', async () => {
    await wrapper.setProps({ open: true });
    await wrapper.vm.$nextTick();
    expect(wrapper.vm.open).toBe(true);
  });
});
