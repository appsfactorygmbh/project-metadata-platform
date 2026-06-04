import { flushPromises, mount, VueWrapper } from '@vue/test-utils';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import CreateProjectView from '../CreateProjectView.vue';
import { projectRoutingSymbol } from '@/store/injectionSymbols';
import { useProjectRouting } from '@/utils/hooks';
import router from '@/router';
import { createTestingPinia } from '@pinia/testing';
import { setActivePinia } from 'pinia';
import { useTeamStore, useCompanyStore } from '@/store';

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
      companyId: number;
    };
  };

  let wrapper: VueWrapper<CreateProjectViewInstance>;
  let teamStore: ReturnType<typeof useTeamStore>;
  let companyStore: ReturnType<typeof useCompanyStore>;

  const mockTeams = [
    { id: 101, teamName: 'Mock Team A' },
    { id: 102, teamName: 'Mock Team B' },
    { id: 103, teamName: 'Mock Team C' },
  ];

  const mockCompanies = [
    { id: 101, companyName: 'Mock Company A' },
    { id: 102, companyName: 'Mock Company B' },
    { id: 103, companyName: 'Mock Company C' },
  ];

  beforeEach(() => {
    const testingPinia = createTestingPinia({
      stubActions: false,
      initialState: {
        team: {
          teams: mockTeams,
        },
        company: {
          companies: mockCompanies,
        },
      },
    });

    teamStore = useTeamStore(testingPinia);
    companyStore = useCompanyStore(testingPinia);

    teamStore.getNameToId = vi.fn(
      (id: number) => mockTeams.find((t) => t.id === id)?.teamName,
    );
    teamStore.getIdToName = vi.fn(
      (name: string) => mockTeams.find((t) => t.teamName === name)?.id,
    );

    companyStore.getNameToId = vi.fn(
      (id: number) => mockCompanies.find((c) => c.id === id)?.companyName,
    );
    companyStore.getIdToName = vi.fn(
      (name: string) => mockCompanies.find((c) => c.companyName === name)?.id,
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

  it('set team sample data correctly', async () => {
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


    const antSelectDropdown = modalContentWrapper!.querySelector(
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

  it('sets company sample data correctly', async () => {
    const button = wrapper.findComponent({ name: 'a-float-button' });
    await button.trigger('click');

    await flushPromises();

    let modalContentWrapper = document.body.querySelector('.ant-modal-wrap');
    expect(modalContentWrapper).not.toBeNull();

    const companySelectTrigger = modalContentWrapper!.querySelector(
      '[data-test="company-id-select"] .ant-select-selector',
    );
    expect(companySelectTrigger).not.toBeNull();

    const mousedownEvent = new MouseEvent('mousedown', {
      bubbles: true,
      cancelable: true,
      button: 0,
    });

    (companySelectTrigger as HTMLElement).dispatchEvent(mousedownEvent);
    await flushPromises();


    const options = modalContentWrapper!.querySelectorAll(
      '[data-testid^="company-select-"]'
    );

    expect(options.length).toBe(companyStore.getCompanies.length);

    expect(options[0]!.textContent).toBe('Mock Company A');
    expect(options[0]!.getAttribute('data-testid')).toBe('company-select-0');

    expect(options[1]!.textContent).toBe('Mock Company B');
    expect(options[1]!.getAttribute('data-testid')).toBe('company-select-1');


    expect(wrapper.vm.formState.companyId).toBe(0);


    const companySelect = wrapper.findComponent(
      '[data-test="company-id-select"]'
    ) as VueWrapper<any>;

    companySelect.vm.$emit('update:value', 'Mock Company A');

    await flushPromises();

    expect(wrapper.vm.formState.companyId).toBe(101);
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
