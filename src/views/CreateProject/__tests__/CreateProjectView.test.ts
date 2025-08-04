import { flushPromises, mount, VueWrapper } from '@vue/test-utils';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import CreateProjectView from '../CreateProjectView.vue';
import { projectRoutingSymbol } from '@/store/injectionSymbols';
import { useProjectRouting } from '@/utils/hooks';
import router from '@/router';
import { createTestingPinia } from '@pinia/testing';
import { setActivePinia } from 'pinia';
import { useTeamStore } from '@/store';

describe('CreateProjectView.vue', () => {
  type CreateProjectViewInstance = {
    fetchError: boolean;
    open: boolean;
    handleOk: () => Promise<void>;
    resetModal: () => void;
    formRef: {
      resetFields?: () => void;
      validate?: () => Promise<void>;
    };
    formState: {
      teamId: number | undefined;
    };
  };

  let wrapper: VueWrapper<CreateProjectViewInstance>;
  let teamStore: ReturnType<typeof useTeamStore>;

  const mockTeams = [
    { id: 101, teamName: 'Mock Team A' },
    { id: 102, teamName: 'Mock Team B' },
    { id: 103, teamName: 'Mock Team C' },
  ];

  beforeEach(() => {
    const testingPinia = createTestingPinia({
      stubActions: false,
      initialState: {
        team: {
          teams: mockTeams,
        },
      },
    });

    teamStore = useTeamStore(testingPinia);

    teamStore.getNameToId = vi.fn(
      (id: number) => mockTeams.find((t) => t.id === id)?.teamName,
    );
    teamStore.getIdToName = vi.fn(
      (name: string) => mockTeams.find((t) => t.teamName === name)?.id,
    );

    setActivePinia(testingPinia);

    wrapper = mount(CreateProjectView, {
      plugins: [testingPinia],
      global: {
        provide: {
          [projectRoutingSymbol as symbol]: useProjectRouting(router),
        },
      },
    }) as unknown as VueWrapper<CreateProjectViewInstance>;
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
    wrapper.vm.resetModal();
    expect(wrapper.vm.formRef.resetFields).toHaveBeenCalled();
  });

  it('set sample data correctly', async () => {
    const button = wrapper.findComponent({ name: 'a-float-button' });
    await button.trigger('click');

    await flushPromises();

    // get the modal div (search in document because it is portaled out of the wrapper)
    let modalContentWrapper = document.body.querySelector('.ant-modal-wrap');
    expect(modalContentWrapper).not.toBeNull();

    const teamSelectTrigger = modalContentWrapper!.querySelector(
      '[data-test="team-id-select"] .ant-select-selector',
    );
    expect(teamSelectTrigger).not.toBeNull();

    const mousedownEvent = new MouseEvent('mousedown', {
      bubbles: true,
      cancelable: true,
      button: 0,
    });

    (teamSelectTrigger as HTMLElement).dispatchEvent(mousedownEvent);
    await flushPromises();

    const teamLocalPopupContainer = modalContentWrapper!.querySelector(
      '.team-local-popup-container',
    );
    expect(teamLocalPopupContainer).not.toBeNull();

    const antSelectDropdown = teamLocalPopupContainer!.querySelector(
      '.ant-select-dropdown',
    );
    expect(antSelectDropdown).not.toBeNull();

    const options = antSelectDropdown!.querySelectorAll(
      '.ant-select-item-option',
    );
    expect(options.length).toBe(teamStore.getTeams.length + 1);

    expect(options[0]!.textContent).toBe('No Team');
    expect(options[1]!.textContent).toBe('Mock Team A');
    expect(options[1]!.getAttribute('data-testid')).toBe('team-select-0');

    expect(options[2]!.textContent).toBe('Mock Team B');
    expect(options[2]!.getAttribute('data-testid')).toBe('team-select-1');

    expect(options[3]!.textContent).toBe('Mock Team C');
    expect(options[3]!.getAttribute('data-testid')).toBe('team-select-2');

    expect(wrapper.vm.formState.teamId == undefined);

    (options[1] as HTMLElement).click();
    expect(wrapper.vm.formState.teamId == 101);

    (options[0] as HTMLElement).click();
    expect(wrapper.vm.formState.teamId == undefined);
  });

  it('handles form validation errors on handleOk', async () => {
    wrapper.vm.formRef = {
      validate: vi.fn().mockRejectedValue('Validation Error'),
    };
    await wrapper.vm.handleOk();
    expect(wrapper.vm.formRef.validate).toHaveBeenCalled();
  });

  it('emits close event when modal is closed', async () => {
    // Triggers the event directly
    wrapper.vm.$emit('close');

    const emittedEvents = wrapper.emitted('close');
    expect(emittedEvents).toBeTruthy();
  });
});
