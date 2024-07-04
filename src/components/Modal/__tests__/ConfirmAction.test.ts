import { mount, VueWrapper } from '@vue/test-utils';
import { describe, it, expect, beforeEach, afterEach } from 'vitest';
import ConfirmationDialog from '../ConfirmAction.vue';
import ModalComponent from '@/components/Modal/ConfirmAction.vue'; // Pfad zu deiner Komponente
import { nextTick } from 'vue';

describe('ConfirmationDialog.vue', () => {
  let wrapper: VueWrapper<InstanceType<typeof ConfirmationDialog>>;

  beforeEach(() => {
    wrapper = mount(ConfirmationDialog, {
      attachTo: document.body,
      props: {
        isOpen: true,
        title: 'Test Title',
        message: 'Test Message',
      },
    });

    return nextTick();
  });

  afterEach(() => {
    wrapper.unmount();
  });

  it('renders the dialog when isOpen is true', async () => {
    await nextTick();

    const modal = document.querySelector('.ant-modal');

    expect(modal).not.toBeNull();
    expect(modal?.classList.contains('ant-modal-hidden')).toBe(false);

    const modalTitle = document.querySelector('.ant-modal-title');
    expect(modalTitle?.textContent).toBe('Test Title');

    const modalBody = document.querySelector('.ant-modal-body');
    expect(modalBody?.textContent).toContain('Test Message');
  });

  it('should emit confirm and close the modal when confirm is clicked', async () => {
    const vm = wrapper.vm;
    vm.onConfirm();
    await nextTick();

    const emittedEvents = wrapper.emitted();

    expect(emittedEvents.confirm).toBeTruthy();
    expect(emittedEvents.confirm.length).toBe(1);
    expect(emittedEvents['update:isOpen']).toBeTruthy();
    expect(emittedEvents['update:isOpen'].length).toBe(1);
    expect(emittedEvents['update:isOpen'][0]).toEqual([false]);
  });

  it('should emit cancel and close the modal when cancel is clicked', async () => {
    const vm = wrapper.vm;
    vm.onCancel();
    await nextTick();

    const emittedEvents = wrapper.emitted();

    expect(emittedEvents.cancel).toBeTruthy();
    expect(emittedEvents.cancel.length).toBe(1);
    expect(emittedEvents['update:isOpen']).toBeTruthy();
    expect(emittedEvents['update:isOpen'].length).toBe(1);
    expect(emittedEvents['update:isOpen'][0]).toEqual([false]);
  });
});
describe('ModalComponent open', () => {
  let wrapper: VueWrapper<InstanceType<typeof ModalComponent>>;

  beforeEach(() => {
    wrapper = mount(ModalComponent, {
      props: {
        isOpen: false,
        title: 'Test Title',
        message: 'Test Message',
      },
      attachTo: document.body,
    });
  });

  afterEach(() => {
    wrapper.unmount();
  });

  it('should open the modal when isOpen prop is true', async () => {
    await wrapper.setProps({ isOpen: true });
    expect(wrapper.vm.localIsOpen).toBe(true);

    expect(document.querySelector('.ant-modal')).not.toBeNull();
  });

  it('renders the correct title when open', async () => {
    await wrapper.setProps({ isOpen: true });
    const titleElement = document.querySelector('.ant-modal-title');
    expect(titleElement?.textContent).toBe('Test Title');
  });

  it('renders the correct message when open', async () => {
    await wrapper.setProps({ isOpen: true });
    const messageElement = document.querySelector('.ant-modal-body');
    expect(messageElement?.textContent).toContain('Test Message');
  });
});
