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
    const button = wrapper.findComponent({name: 'a-float-button'});
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

  it('send put request with project data', async () => {
    // Create a spy for the addProject method
    const addProjectSpy = vi.fn();
    wrapper.vm.projectsStore = {
      addProject: addProjectSpy,
      setIsAdding: vi.fn(),
      getIsAdding: vi.fn(),
      getAddedSuccessfully: vi.fn(),
      fetchProjects: vi.fn(),
    };

    // Set formState values
    wrapper.vm.formState = {
      projectName: 'Test Project',
      businessUnit: 'Test Unit',
      teamNumber: 1,
      department: 'Test Department',
      clientName: 'Test Client',
    };

    // Mock formRef validate method to resolve immediately
    wrapper.vm.formRef = {
      validate: vi.fn().mockResolvedValue('Validation Success'),
    };

    // Call handleOk
    await wrapper.vm.handleOk();

    // Check if addProject has been called with correct data
    expect(addProjectSpy).toHaveBeenCalledWith({
      projectName: 'Test Project',
      businessUnit: 'Test Unit',
      teamNumber: 1,
      department: 'Test Department',
      clientName: 'Test Client',
    });
  });
})
