import { describe, expect, it } from 'vitest';
import { flushPromises, mount } from '@vue/test-utils';
import { setActivePinia } from 'pinia';
import ProjectInformation from '../ProjectInformation.vue';
import { createTestingPinia, type TestingPinia } from '@pinia/testing';
import {
  localLogStoreSymbol,
  projectRoutingSymbol,
} from '@/store/injectionSymbols';
import { useProjectStore } from '@/store';
import router from '@/router';
import type { DetailedProjectModel } from '@/models/Project';
import {
  DeleteOutlined,
  EditOutlined,
  UndoOutlined,
} from '@ant-design/icons-vue';
import { ResourceActions } from '@/models/utils/ResourceActions.ts';

vi.mock('@/utils/hooks', async () => {
  const actual = await vi.importActual('@/utils/hooks');
  return {
    ...actual,
    useDeselect: () => ({
      isDeselected: ref(false),
    }),
  };
});

const testData: DetailedProjectModel = {
  id: 1,
  slug: 'test_project',
  isArchived: false,
  projectName: 'Heute Show',
  clientName: 'ZDF',
  team: {
    id: 1,
    businessUnit: { id: 1, businessUnitName: 'BU Health' },
    teamName: '42',
    ptl: 'Max Mustermann',
  },
  company: { id: 1, companyName: 'Appsfactory' },
  companyState: 'EXTERNAL',
  ismsLevel: 'NORMAL',
  isEoC: false,
  notes: 'TestNotes',
  permissions: [ResourceActions.Edit, ResourceActions.Delete],
};

describe('ProjectInformation.vue', () => {
  let testingPinia: TestingPinia;

  beforeEach(() => {
    vi.clearAllMocks();

    testingPinia = createTestingPinia({
      createSpy: vi.fn,
      initialState: {
        project: { project: JSON.parse(JSON.stringify(testData)) },
        company: { companies: [] },
        team: { teams: [] },
      },
    });
    setActivePinia(testingPinia);
  });

  const generateWrapper = () =>
    mount(ProjectInformation, {
      global: {
        plugins: [router, testingPinia],
        provide: {
          projectEdits: ref({}),
          [localLogStoreSymbol as symbol]: {},
          [projectRoutingSymbol as symbol]: {
            setProjectId: vi.fn(),
          },
        },
        stubs: {
          ConfirmAction: true,
        },
      },
    });

  it('renders the project information correctly', async () => {
    const wrapper = generateWrapper();
    await flushPromises();

    expect(wrapper.find('.projectName').text()).toEqual('Heute Show');

    const infoCards = wrapper.findAll('.infoCard');

    expect(infoCards[0].text()).toContain('Project\xa0Slug');
    expect(infoCards[0].text()).toContain('test_project');

    expect(infoCards[1].text()).toContain('Client\xa0Name');
    expect(infoCards[1].text()).toContain('ZDF');

    expect(infoCards[2].text()).toContain('Company');
    expect(infoCards[2].text()).toContain('Appsfactory');

    expect(infoCards[3].text()).toContain('Company\xa0State');
    expect(infoCards[4].text()).toContain('ISMS\xa0Level');
    expect(infoCards[6].text()).toContain('Team\xa0Name');
    expect(infoCards[6].text()).toContain('42');

    expect(infoCards[7].text()).toContain('Business\xa0Unit');
    expect(infoCards[7].text()).toContain('BU Health');

    expect(infoCards[8].text()).toContain('PTL');
    expect(infoCards[8].text()).toContain('Max Mustermann');

    expect(wrapper.findAll('.notesCard')[0].text()).toBe('TestNotes');
  });

  it('opens the confirmation modal when DeleteOutlined button is clicked', async () => {
    const projectStore = useProjectStore();

    vi.spyOn(projectStore, 'getProject', 'get').mockReturnValue({
      ...testData,
      isArchived: true,
    } as any);

    const wrapper = generateWrapper();
    await flushPromises();

    const deleteButton = wrapper.findComponent(DeleteOutlined);
    expect(deleteButton.exists()).toBeTruthy();

    await deleteButton.trigger('click');
    await flushPromises();

    const confirmModal = wrapper.findComponent({ name: 'ConfirmAction' });
    expect(confirmModal.exists()).toBeTruthy();
    expect(confirmModal.props('isOpen')).toBe(true);
  });

  it('does not render edit button but shows reactivate and delete buttons when archived', async () => {
    const projectStore = useProjectStore();

    vi.spyOn(projectStore, 'getProject', 'get').mockReturnValue({
      ...testData,
      isArchived: true,
    } as any);

    const wrapper = generateWrapper();
    await flushPromises();

    expect(wrapper.findComponent(EditOutlined).exists()).toBeFalsy();
    expect(wrapper.findComponent(UndoOutlined).exists()).toBeTruthy();
    expect(wrapper.findComponent(DeleteOutlined).exists()).toBeTruthy();
  });
});
