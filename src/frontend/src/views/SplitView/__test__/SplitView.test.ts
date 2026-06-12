import { describe, expect, it } from 'vitest';
import { mount } from '@vue/test-utils';
import SplitView from '../SplitView.vue';
import router from '@/router';
import { Splitpanes, type SplitpanesResizedPayload } from 'splitpanes';

const generateWrapper = () => {
  return mount(SplitView, {
    global: {
      stubs: {
        CreateProjectView: {
          template: '<span />',
        },
        ProjectSearchView: {
          template: '<span />',
        },
        ProjectView: {
          template: '<span />',
        },
      },
      plugins: [router],
    },
  });
};

describe('SplitView.vue', () => {
  beforeEach(() => {
    beforeEach(() => {
      window.localStorage.clear();
      vi.restoreAllMocks(); // Ensure prototype spies are cleared between tests
    });
  });
  it('renders correctly', () => {
    const wrapper = generateWrapper();

    expect(wrapper.findAll('.splitpanes__pane')[0].isVisible()).toBeTruthy();
    expect(wrapper.findAll('.splitpanes__pane')[1].isVisible()).toBeTruthy();
  });

  it('should save pane sizes to localStorage on resize', async () => {
    const wrapper = generateWrapper();

    const newSizes: SplitpanesResizedPayload = {
      panes: [
        { min: 20, max: 100, size: 20 },
        { min: 32, max: 100, size: 80 },
      ],
    };
    const pane = wrapper.findComponent(Splitpanes);
    pane.vm.$emit('resized', newSizes);

    const storedSizes = window.localStorage.getItem('paneSizes');
    expect(storedSizes).toBe(JSON.stringify(newSizes.panes));
  });

  it('should load pane sizes from localStorage on mount', () => {
    const mockPaneSizes: SplitpanesResizedPayload = {
      panes: [
        { min: 20, max: 100, size: 20 },
        { min: 32, max: 100, size: 80 },
      ],
    };
    const getItemSpy = vi
      .spyOn(window.Storage.prototype, 'getItem')
      .mockReturnValue(JSON.stringify(mockPaneSizes.panes));

    const wrapper = generateWrapper();

    const leftPaneWidth = (wrapper.vm as unknown as { leftPaneWidth: number })
      .leftPaneWidth;
    const rightPaneWidth = (wrapper.vm as unknown as { rightPaneWidth: number })
      .rightPaneWidth;

    expect(getItemSpy).toHaveBeenCalledWith('paneSizes');
    expect(leftPaneWidth).toBe(mockPaneSizes.panes[0].size);
    expect(rightPaneWidth).toBe(mockPaneSizes.panes[1].size);

    getItemSpy.mockRestore();
  });
});
