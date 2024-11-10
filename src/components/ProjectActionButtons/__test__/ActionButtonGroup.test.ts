import ActionButtonGroup from '../ActionButtonGroup.vue';
import { mount } from '@vue/test-utils';
import { describe, expect, it, vi } from 'vitest';

describe('ActionButtonGroup.vue', () => {
  it('renders both create and archive buttons correctly', () => {
    const wrapper = mount(ActionButtonGroup);
    const buttons = wrapper.findAll('.ant-float-btn');

    expect(buttons).toHaveLength(2);
    buttons.forEach((button) => expect(button.isVisible()).toBeTruthy());
  });

  it('calls the create project function on create button click', async () => {
    const mockShowModal = vi.fn();

    const wrapper = mount(ActionButtonGroup, {
      props: {
        onOpenCreateModal: mockShowModal, // Übergibt die Mock-Funktion
      },
    });

    const createButton = wrapper.findComponent({ name: 'CreateProjectButton' });
    await createButton.trigger('click');

    expect(mockShowModal).toHaveBeenCalled();
  });

  it('calls the archive project function on archive button click', async () => {
    const mockArchiveProject = vi.fn();

    const wrapper = mount(ActionButtonGroup, {
      props: {
        projectId: 1,
        onArchiveSuccess: mockArchiveProject, // Übergibt die Mock-Funktion
      },
    });

    const archiveButton = wrapper.findComponent({
      name: 'ArchiveProjectButton',
    });
    await archiveButton.trigger('click');

    expect(mockArchiveProject).toHaveBeenCalled();
  });
});
