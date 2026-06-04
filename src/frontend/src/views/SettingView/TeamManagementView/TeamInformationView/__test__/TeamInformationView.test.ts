import { mount } from '@vue/test-utils';
import { describe, expect, it } from 'vitest';
import { createPinia, setActivePinia } from 'pinia';
import { createTestingPinia } from '@pinia/testing';
import { teamRoutingSymbol, teamStoreSymbol } from '@/store/injectionSymbols';
import { useTeamStore } from '@/store';
import router from '@/router';
import type { TeamModel } from '@/models/Team';

import { TeamInformationView } from '..';
import { useTeamRouting } from '@/utils/hooks/useTeamRouting';

const teamData1: TeamModel = {
  id: 100,
  teamName: 'Team1',
  businessUnit: { id: 1, businessUnitName: 'BU' },
};

const mockRoute = {
  path: '/mock-path',
  query: { teamId: '200' },
  params: {},
  hash: '',
  fullPath: '/mock-path',
  matched: [],
  meta: {},
  redirectedFrom: undefined,
};
const mockRouter = {
  push: vi.fn(),
};

vi.mock('vue-router', async (importOriginal) => {
  const actual = await importOriginal<typeof import('vue-router')>();
  return {
    ...actual,
    useRoute: () => mockRoute,
    useRouter: () => mockRouter,
  };
});

describe('TeamInformationView.vue', () => {
  setActivePinia(createPinia());
  const teamStore = useTeamStore();

  const generateWrapper = () => {
    return mount(TeamInformationView, {
      plugins: [
        createTestingPinia({
          stubActions: false,
        }),
      ],
      global: {
        provide: {
          [teamStoreSymbol as symbol]: teamStore,
          [teamRoutingSymbol as symbol]: useTeamRouting(),
        },
        plugins: [router],
      },
    });
  };

  it('renders correctly', () => {
    mockRoute.query.teamId = '100';
    teamStore.setTeam(teamData1);
    const wrapper = generateWrapper();
    expect(wrapper.find('.label').exists()).toBe(true);
    const text = wrapper.findAll('.text');
    expect(text[0].text()).toBe(teamData1.teamName);
    expect(text[1].text()).toBe(teamData1.businessUnit.businessUnitName);
  });
});
