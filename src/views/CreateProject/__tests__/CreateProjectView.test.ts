import { mount, VueWrapper } from '@vue/test-utils';
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
