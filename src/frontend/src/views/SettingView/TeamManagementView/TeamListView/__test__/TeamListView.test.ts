import { flushPromises, mount } from '@vue/test-utils';
import { describe, expect, it, vi } from 'vitest';
import { createTestingPinia } from '@pinia/testing';
import { teamRoutingSymbol, teamStoreSymbol } from '@/store/injectionSymbols';
import { useTeamStore } from '@/store';
import { PlusOutlined } from '@ant-design/icons-vue';
import { TeamListView } from '..';

vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: vi.fn(),
  }),
}));

const teamData1 = {
  id: '100',
  teamName: 'Team1',
  businessUnit: 'Finance',
};
const teamData2 = {
  id: '200',
  teamName: 'Team2',
  businessUnit: 'Health',
};

describe('TeamListView.vue', () => {
  const generateWrapper = () => {
    createTestingPinia({
      stubActions: true,
      initialState: {
        team: { teams: [teamData1, teamData2], isLoading: false },
      },
    });

    const teamStore = useTeamStore();

    const mockTeamRouting = {
      routerTeamId: ref(''),
      setTeamId: vi.fn(),
    };

    return mount(TeamListView, {
      global: {
        components: {
          PlusOutlined,
        },
        stubs: {
          RouterView: true,
        },
        provide: {
          [teamStoreSymbol as symbol]: teamStore,
          [teamRoutingSymbol as symbol]: mockTeamRouting,
        },
      },
    });
  };

  it('renders correctly', async () => {
    const wrapper = generateWrapper();

    await flushPromises();

    expect(wrapper.find('.layout').exists()).toBe(true);

    const icon = wrapper.findComponent(PlusOutlined);
    expect(icon.exists()).toBe(true);

    expect(wrapper.text()).toContain('Create Team');

    expect(wrapper.text()).toContain('Team1');
    expect(wrapper.text()).toContain('Team2');
  });

  it('calls fetchAll on mount', () => {
    generateWrapper();
    const userStore = useTeamStore();
    expect(userStore.fetchAll).toHaveBeenCalled();
  });
});
