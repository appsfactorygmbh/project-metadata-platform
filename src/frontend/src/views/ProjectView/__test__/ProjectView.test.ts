import { flushPromises, mount } from '@vue/test-utils';
import { describe, expect, it } from 'vitest';
import { ProjectView } from '..';
import router from '@/router';
import {
  localLogStoreSymbol,
  projectRoutingSymbol,
} from '@/store/injectionSymbols';
import { useProjectStore } from '@/store';
import { ResourceActions } from '@/models/utils';
import { type TestingPinia, createTestingPinia } from '@pinia/testing';

vi.mock('vue-auth3', () => ({
  useAuth: () => ({
    ready: vi.fn().mockResolvedValue(undefined),
    check: vi.fn().mockReturnValue(true),
  }),
}));

const mockIsEditing = ref(false);
const mockStopEditing = vi.fn();
vi.mock('@/utils/hooks/useEditing', () => ({
  useEditing: () => ({
    isEditing: mockIsEditing,
    stopEditing: mockStopEditing,
    startEditing: vi.fn(),
  }),
}));

describe('ProjectView.vue', () => {
  let pinia: TestingPinia;

  beforeEach(() => {
    vi.clearAllMocks();
    mockIsEditing.value = false;

    pinia = createTestingPinia({
      createSpy: vi.fn,
      initialState: {
        project: {
          project: {
            id: 300,
            projectName: 'Test Project',
            clientName: 'Test Client',
            company: { id: 2 },
            team: { id: 1 },
            isArchived: false,
            permissions: [ResourceActions.Edit],
          },
          projects: [{ id: 300, isArchived: false }],
        },
      },
    });
  });

  const generateWrapper = () => {
    return mount(ProjectView, {
      global: {
        plugins: [router, pinia],
        provide: {
          [localLogStoreSymbol as symbol]: { fetch: vi.fn() },
          [projectRoutingSymbol as symbol]: {
            routerProjectId: computed(() => 300),
            setProjectId: vi.fn(),
          },
        },
        stubs: {
          ProjectInformation: true,
          ProjectPlugins: true,
          LocalLogView: true,
          AddPluginView: true,
          ProjectEditButtons: true,
          ConfirmAction: true,
        },
      },
    });
  };

  it('renders the main project layout when an active project exists', async () => {
    const wrapper = generateWrapper();

    expect(wrapper.find('.empty-state-container').exists()).toBe(false);

    expect(wrapper.findComponent({ name: 'ProjectInformation' }).exists()).toBe(
      true,
    );
    expect(wrapper.findComponent({ name: 'ProjectPlugins' }).exists()).toBe(
      true,
    );
  });

  it('shows ProjectEditButtons and preps projectEdits payload when editing starts', async () => {
    const wrapper = generateWrapper();

    expect(wrapper.findComponent({ name: 'ProjectEditButtons' }).exists()).toBe(
      false,
    );
    mockIsEditing.value = true;
    await flushPromises();

    expect(wrapper.findComponent({ name: 'ProjectEditButtons' }).exists()).toBe(
      true,
    );
  });

  it('calls update on the project store when save is triggered', async () => {
    const wrapper = generateWrapper();
    const projectStore = useProjectStore();

    mockIsEditing.value = true;
    await flushPromises();

    const editButtons = wrapper.findComponent({ name: 'ProjectEditButtons' });
    await editButtons.vm.$emit('save');

    expect(projectStore.update).toHaveBeenCalledWith(
      300,
      expect.objectContaining({ projectName: 'Test Project' }),
    );
  });
});
