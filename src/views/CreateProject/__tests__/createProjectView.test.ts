import { describe, it, expect, beforeEach, vi } from 'vitest';
import { mount, VueWrapper } from '@vue/test-utils';
import createProjectView from '../createProjectView.vue';
import { projectsService } from '../../../services/ProjectService.ts';

const spy = vi.spyOn(projectsService, 'addProject');

describe('ProjectModal.vue', () => {
  let wrapper: VueWrapper<InstanceType<typeof createProjectView>>;

  beforeEach(() => {
    wrapper = mount(createProjectView);
  });

  it('should open the modal when the float button is clicked', async () => {
    const button = wrapper.findComponent({ name: 'a-float-button' });
    await button.trigger('click');
    expect((wrapper.vm as any).open).toBe(true);
  });

  it('should validate fields correctly', async () => {
    (wrapper.vm as any).projectName = '';
    (wrapper.vm as any).businessUnit = '';
    (wrapper.vm as any).teamNumber = '';
    (wrapper.vm as any).department = '';
    (wrapper.vm as any).clientName = '';

    await (wrapper.vm as any).handleOk();

    expect((wrapper.vm as any).projectNameStatus).toBe('error');
    expect((wrapper.vm as any).businessUnitStatus).toBe('error');
    expect((wrapper.vm as any).teamNumberStatus).toBe('error');
    expect((wrapper.vm as any).departmentStatus).toBe('error');
    expect((wrapper.vm as any).clientNameStatus).toBe('error');
  });

  it('should call projectsService.addProject with the correct data', async () => {
    (wrapper.vm as any).projectName = 'Project A';
    (wrapper.vm as any).businessUnit = 'Business Unit A';
    (wrapper.vm as any).teamNumber = 'Team 1';
    (wrapper.vm as any).department = 'Department A';
    (wrapper.vm as any).clientName = 'Client A';

    await (wrapper.vm as any).handleOk();
    expect(spy).toHaveBeenCalledWith({
      projectName: 'Project A',
      businessUnit: 'Business Unit A',
      teamNumber: 'Team 1',
      department: 'Department A',
      clientName: 'Client A',
    });
  });

  it('should set fetchError to true if project creation fails', async () => {
    (wrapper.vm as any).projectName = 'Project A';
    (wrapper.vm as any).businessUnit = 'Business Unit A';
    (wrapper.vm as any).teamNumber = 'Team 1';
    (wrapper.vm as any).department = 'Department A';
    (wrapper.vm as any).clientName = 'Client A';

    spy.mockResolvedValue(new Response(null, {
      status: 405,
    }));

    await (wrapper.vm as any).handleOk();

    expect((wrapper.vm as any).fetchError).toBe(true);
  });

  it('should close the modal and reset fetchError if project creation succeeds', async () => {
    (wrapper.vm as any).projectName = 'Project A';
    (wrapper.vm as any).businessUnit = 'Business Unit A';
    (wrapper.vm as any).teamNumber = 'Team 1';
    (wrapper.vm as any).department = 'Department A';
    (wrapper.vm as any).clientName = 'Client A';

    spy.mockResolvedValue(new Response("100", {
      status: 201,
    }));

    await (wrapper.vm as any).handleOk();

    expect((wrapper.vm as any).fetchError).toBe(false);
    expect((wrapper.vm as any).open).toBe(false);
  });
});
