import { mount } from '@vue/test-utils';
import { describe, it, expect } from 'vitest';
import ConfirmationDialog from '@/components/ConfirmAction/ConfirmationDialog.vue';

describe('ConfirmationDialog.vue', () => {
  it('renders the dialog when isOpen is true', async () => {
    const wrapper = mount(ConfirmationDialog, {
      attachTo: document.body, // Add this to specify the teleport destination
      props: {
        isOpen: true,
        title: 'Test Title',
        message: 'Test Message',
      },
    });

    await wrapper.vm.$nextTick();

    // Add debugging output to ensure the correct elements are found
    console.log(wrapper.html());

    const modal = document.querySelector('.ant-modal'); // Use document.querySelector for teleport
    console.log(modal?.innerHTML); // add Debugging-Output

    expect(modal).not.toBeNull();
    expect(modal?.classList.contains('ant-modal-hidden')).toBe(false);

    const modalTitle = document.querySelector('.ant-modal-title');
    expect(modalTitle?.textContent).toBe('Test Title');

    const modalBody = document.querySelector('.ant-modal-body');
    expect(modalBody?.textContent).toContain('Test Message');
  });
});
