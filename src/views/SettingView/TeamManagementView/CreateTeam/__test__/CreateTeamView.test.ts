import { describe, expect, it } from 'vitest';
import { VueWrapper, flushPromises, mount } from '@vue/test-utils';
import { setActivePinia } from 'pinia';
import { createTestingPinia } from '@pinia/testing';
import { teamRoutingSymbol, teamStoreSymbol } from '@/store/injectionSymbols';
import { FormItem } from 'ant-design-vue';
import CreateTeamView from '../CreateTeamView.vue';
import type { TeamModel } from '@/models/Team';
import { useTeamStore } from '@/store';
import { useFormStore } from '@/components/Form';

describe('CreateTeamView.vue', () => {
  setActivePinia(createTestingPinia({ stubActions: false }));

  let wrapper: VueWrapper;
  afterEach(() => {
    if (wrapper) {
      wrapper.unmount();
    }
  });

  const mockTeamRoutingService = {
    setTeamId: vi.fn((id: number) => Promise.resolve()),
  };

  it('renders correctly', () => {
    const inputFields = ['Team Name', 'Business Unit', 'PTL'];

    wrapper = mount(CreateTeamView, {
      global: {
        provide: {
          [teamStoreSymbol as symbol]: useTeamStore(),
          [teamRoutingSymbol as symbol]: mockTeamRoutingService,
        },
      },
    });

    const formItems = wrapper.findAllComponents(FormItem);
    expect(formItems).toHaveLength(3);

    for (let i = 0; i < formItems.length; i++) {
      expect(formItems[i].find('input').attributes('placeholder')).toBe(
        inputFields[i],
      );
    }
  });

  it('verifies a valid team name correctly', async () => {
    const testData: TeamModel[] = [
      {
        id: 1,
        teamName: 'Test Name',
        businessUnit: 'Bu Test',
      },
    ];

    wrapper = mount(CreateTeamView, {
      plugins: [
        createTestingPinia({
          stubActions: false,
          initialState: {
            team: {
              teams: testData,
            },
          },
        }),
      ],
      global: {
        provide: {
          [teamStoreSymbol as symbol]: useTeamStore(),
          [teamRoutingSymbol as symbol]: mockTeamRoutingService,
        },
      },
    });

    const emailField = wrapper.findAllComponents(FormItem)[0];

    await emailField.find('.ant-input').setValue('test');
    await flushPromises();

    expect(
      emailField.find('.ant-form-item-feedback-icon-success').exists(),
    ).toBe(true);
  });

  it('verifies an invalid team name correctly', async () => {
    const testData: TeamModel[] = [
      {
        id: 1,
        teamName: 'Test Name',
        businessUnit: 'Bu Test',
      },
    ];

    wrapper = mount(CreateTeamView, {
      plugins: [
        createTestingPinia({
          stubActions: false,
          initialState: {
            team: {
              teams: testData,
            },
          },
        }),
      ],
      global: {
        provide: {
          [teamStoreSymbol as symbol]: useTeamStore(),
          [teamRoutingSymbol as symbol]: mockTeamRoutingService,
        },
      },
    });

    const teamNameField = wrapper.findAllComponents(FormItem)[0];

    await teamNameField.find('.ant-input').setValue('Test Name');
    await flushPromises();

    expect(
      teamNameField.find('.ant-form-item-feedback-icon-error').exists(),
    ).toBe(true);
  });

  it('submits the form correctly', async () => {
    const testData = ['Test Team', 'Test BU', 'Test PTL'];

    const teamStore = useTeamStore();
    const formStore = useFormStore('CreateTeamForm');
    const createSpy = vi
      .spyOn(teamStore, 'create')
      .mockImplementation(() => Promise.resolve(1));

    wrapper = mount(CreateTeamView, {
      global: {
        provide: {
          [teamStoreSymbol as symbol]: teamStore,
          [teamRoutingSymbol as symbol]: mockTeamRoutingService,
        },
      },
    });

    const formInputs = wrapper.findAllComponents(FormItem);

    for (let i = 0; i < formInputs.length; i++) {
      await formInputs[i].find('.ant-input').setValue(testData[i]);
    }
    await flushPromises();

    await formStore.submit();
    await flushPromises();

    expect(createSpy).toHaveBeenCalled();

    expect(createSpy).toHaveBeenCalledWith({
      teamName: 'Test Team',
      businessUnit: 'Test BU',
      ptl: 'Test PTL',
    });
  });
});
