import { mount, VueWrapper } from '@vue/test-utils';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import CreateProjectView from '../createProjectView.vue';

describe('CreateProjectView.vue', () => {
  type CreateProjectViewInstance = {
    fetchError: boolean;
    open: boolean;
    handleOk: () => Promise<void>;
    resetModal: () => void;
  };

  let wrapper: VueWrapper<CreateProjectViewInstance>;

  beforeEach(() => {
    wrapper = mount(CreateProjectView) as VueWrapper<CreateProjectViewInstance>;
  });

  it('opens modal when plus button is clicked', async () => {
    const button = wrapper.findComponent({ name: 'a-float-button' });
    await button.trigger('click');
    expect(wrapper.vm.open).toBe(true);
  });

  it('resets form when resetModal is called', async () => {
    wrapper.vm.formRef = {
      resetFields: vi.fn(),
    };
    await wrapper.vm.resetModal();
    expect(wrapper.vm.formRef.resetFields).toHaveBeenCalled();
  });

  it('handles form validation errors on handleOk', async () => {
    wrapper.vm.formRef = {
      validate: vi.fn().mockRejectedValue('Validation Error'),
    };
    await wrapper.vm.handleOk();
    expect(wrapper.vm.formRef.validate).toHaveBeenCalled();
  });
});
