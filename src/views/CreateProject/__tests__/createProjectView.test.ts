import { describe, it, expect, beforeEach, vi } from 'vitest';
import { mount, VueWrapper } from '@vue/test-utils';
import { createPinia } from 'pinia';
import createProjectView from '../createProjectView.vue';
import { projectsService } from '../../../services/ProjectsService';

type CreateProjectViewInstance = {
  projectName: string;
  businessUnit: string;
  teamNumber: number | string;
  department: string;
  clientName: string;
  projectNameStatus: string;
  businessUnitStatus: string;
  teamNumberStatus: string;
  departmentStatus: string;
  clientNameStatus: string;
  fetchError: boolean;
  open: boolean;
  handleOk: () => Promise<void>;
};

import App from '../../../App.vue';

createApp(App).use(createPinia());

const spy = vi.spyOn(projectsService, 'addProject');

describe('ProjectModal.vue', () => {
  let wrapper: VueWrapper<CreateProjectViewInstance>;

  beforeEach(() => {
    wrapper = mount(createProjectView) as VueWrapper<CreateProjectViewInstance>;
  });

  it('should open the modal when the float button is clicked', async () => {
    const button = wrapper.findComponent({ name: 'a-float-button' });
    await button.trigger('click');
    expect(wrapper.vm.open).toBe(true);
  });

  it('should validate fields correctly', async () => {
    wrapper.vm.projectName = '';
    wrapper.vm.businessUnit = '';
    wrapper.vm.teamNumber = '-2';
    wrapper.vm.department = '';
    wrapper.vm.clientName = '';

    await wrapper.vm.handleOk();

    expect(wrapper.vm.projectNameStatus).toBe('error');
    expect(wrapper.vm.businessUnitStatus).toBe('error');
    expect(wrapper.vm.teamNumberStatus).toBe('error');
    expect(wrapper.vm.departmentStatus).toBe('error');
    expect(wrapper.vm.clientNameStatus).toBe('error');
  });

  it('should call projectsService.addProject with the correct data', async () => {
    wrapper.vm.projectName = 'Project A';
    wrapper.vm.businessUnit = 'Business Unit A';
    wrapper.vm.teamNumber = '1';
    wrapper.vm.department = 'Department A';
    wrapper.vm.clientName = 'Client A';

    await wrapper.vm.handleOk();
    expect(spy).toHaveBeenCalledWith({
      projectName: 'Project A',
      businessUnit: 'Business Unit A',
      teamNumber: 1,
      department: 'Department A',
      clientName: 'Client A',
    });
  });

  it('should set fetchError to true if project creation fails', async () => {
    wrapper.vm.projectName = 'Project A';
    wrapper.vm.businessUnit = 'Business Unit A';
    wrapper.vm.teamNumber = '2';
    wrapper.vm.department = 'Department A';
    wrapper.vm.clientName = 'Client A';

    spy.mockResolvedValue(
      new Response(null, {
        status: 400,
      }),
    );

    await wrapper.vm.handleOk();

    expect(wrapper.vm.fetchError).toBe(true);
  });

  it('should close the modal and reset fetchError if project creation succeeds', async () => {
    wrapper.vm.projectName = 'Project A';
    wrapper.vm.businessUnit = 'Business Unit A';
    wrapper.vm.teamNumber = 1;
    wrapper.vm.department = 'Department A';
    wrapper.vm.clientName = 'Client A';

    spy.mockResolvedValue(
      new Response('100', {
        status: 201,
      }),
    );

    await wrapper.vm.handleOk();

    expect(wrapper.vm.fetchError).toBe(false);
    expect(wrapper.vm.open).toBe(false);
  });
});
