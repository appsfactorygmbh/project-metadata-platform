import { mount } from '@vue/test-utils';
import { beforeEach, describe, expect, it } from 'vitest';
import { createTestingPinia } from '@pinia/testing';
import { usePluginStore, useProjectStore } from '../../../store';
import PluginComponent from '../PluginComponent.vue';
import { createPinia, setActivePinia } from 'pinia';
import { ResourceActions } from '@/models/utils/ResourceActions.ts';
import router from '@/router/router.ts';

beforeEach(() => {
  setActivePinia(createPinia());
});

describe('PluginComponent.vue', () => {
  const generateWrapper = (overrides = {}) => {
    return mount(PluginComponent, {
      props: {
        id: 100,
        pluginName: 'Test Plugin',
        url: 'https://example.com/examplePath',
        displayName: 'test instance',
        isLoading: false,
        pluginPermissions: [ResourceActions.Edit, ResourceActions.Delete],
        billingPermissions: [],
      },
      global: {
        plugins: [
          router,
          createTestingPinia({
            createSpy: vi.fn,
            initialState: {
              project: { currentProject: { id: 1 } },
            },
          }),
        ],
        stubs: {
          EditOutlined: true,
          DeleteOutlined: true,
          PlusOutlined: true,
          ConfirmAction: true,
          BillingComponent: true,
        },
      },
    });
  };
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('enters editing mode and displays inputs when edit badge is clicked', async () => {
    const wrapper = generateWrapper();

    expect(wrapper.findAll('input').length).toBe(0);

    const editBadge = wrapper.findComponent({ name: 'EditOutlined' });
    await editBadge.trigger('click');

    const inputs = wrapper.findAll('input');
    expect(inputs.length).toEqual(2);

    expect(wrapper.text()).toContain('Cancel');
    expect(wrapper.text()).toContain('Save');
  });

  it('updates the plugin data when Save is clicked', async () => {
    const wrapper = generateWrapper();
    const pluginStore = usePluginStore();
    const projectStore = useProjectStore();

    vi.spyOn(projectStore, 'getProject', 'get').mockReturnValue({
      id: 1,
    } as any);

    await wrapper.findComponent({ name: 'EditOutlined' }).trigger('click');

    const inputs = wrapper.findAllComponents({ name: 'AInput' });
    await inputs[0].vm.$emit('update:value', 'new display name');
    await inputs[1].vm.$emit('update:value', 'https://example.com/newPath');

    const buttons = wrapper.findAllComponents({ name: 'AButton' });
    const saveButton = buttons.find((b) => b.text().includes('Save'));
    await saveButton!.trigger('click');

    expect(pluginStore.update).toHaveBeenCalledWith(1, 100, {
      displayName: 'new display name',
      url: 'https://example.com/newPath',
    });

    expect(wrapper.findAllComponents({ name: 'AInput' }).length).toBe(0);
  });

  it('opens the confirmation modal when DeleteOutlined is clicked', async () => {
    const wrapper = generateWrapper();

    const confirmModal = wrapper.findComponent({ name: 'ConfirmAction' });
    expect(confirmModal.props('isOpen')).toBe(false);

    const deleteIcon = wrapper.findComponent({ name: 'DeleteOutlined' });
    await deleteIcon.trigger('click');

    expect(confirmModal.props('isOpen')).toBe(true);
  });
});
