import { describe, expect, it } from 'vitest';
import { mount } from '@vue/test-utils';
import SplitView from '../SplitView.vue';
import { createPinia, setActivePinia } from 'pinia';
import router from '@/router';
import { Splitpanes } from 'splitpanes';

setActivePinia(createPinia());

beforeEach(() => {
  localStorage.clear();
});

describe('SplitView.vue', () => {
  it('renders correctly', () => {
    const wrapper = mount(SplitView, {
      global: {
        stubs: {
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

    expect(wrapper.findAll('.splitpanes__pane')[0].isVisible()).toBeTruthy();
    expect(wrapper.findAll('.splitpanes__pane')[1].isVisible()).toBeTruthy();
  });

  it('saves pane sizes to localStorage on resize', async () => {
    const wrapper = mount(SplitView, {
      global: {
        stubs: {
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

    const newSizes = [
      { min: 20, max: 100, size: 20 },
      { min: 32, max: 100, size: 80 },
    ];
    const pane = wrapper.findComponent(Splitpanes);
    pane.vm.$emit('resized', newSizes);

    const storedSizes = localStorage.getItem('paneSizes');
    expect(storedSizes).toBe(JSON.stringify(newSizes));
  });

  it('should save pane sizes to localStorage on resize', async () => {
    const wrapper = mount(SplitView, {
      global: {
        stubs: {
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

    const newSizes = [
      { min: 20, max: 100, size: 20 },
      { min: 32, max: 100, size: 80 },
    ];
    const pane = wrapper.findComponent(Splitpanes);
    pane.vm.$emit('resized', newSizes);

    const storedSizes = localStorage.getItem('paneSizes');
    expect(storedSizes).toBe(JSON.stringify(newSizes));
  });

  it('should load pane sizes from localStorage on mount', () => {
    const mockPaneSizes = [
      { min: 20, max: 100, size: 20 },
      { min: 32, max: 100, size: 80 },
    ];
    const getItemSpy = vi
      .spyOn(Storage.prototype, 'getItem')
      .mockReturnValue(JSON.stringify(mockPaneSizes));

    const wrapper = mount(SplitView, {
      global: {
        stubs: {
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

    const leftPaneWidth = (wrapper.vm as unknown as {leftPaneWidth: number}).leftPaneWidth ;
    const rightPaneWidth = (wrapper.vm as unknown as {rightPaneWidth: number}).rightPaneWidth ;

    expect(getItemSpy).toHaveBeenCalledWith('paneSizes');
    expect(leftPaneWidth).toBe(mockPaneSizes[0].size);
    expect(rightPaneWidth).toBe(mockPaneSizes[1].size);
  });
});
