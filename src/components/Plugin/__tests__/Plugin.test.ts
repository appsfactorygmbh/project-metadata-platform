import { mount, VueWrapper } from '@vue/test-utils';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { cutAfterTLD, createFaviconURL } from '../editURL';
import PluginComponent from '../PluginComponent.vue';
import router from '../../../router';
import { createTestingPinia } from '@pinia/testing';

interface PluginComponentInstance {
  pluginName: string;
  url: string;
  displayName: string;
  id: number;
  isLoading: boolean;
  isEditing: boolean;
  hide?: boolean;
}

const generateWrapper = (
  name: string,
  url: string,
  displayName: string,
  isLoading: boolean,
  isEditing: boolean,
  id: number,
  editKey = -1,
): VueWrapper<ComponentPublicInstance<PluginComponentInstance>> => {
  return mount(PluginComponent, {
    plugins: [
      createTestingPinia({
        stubActions: false,
      }),
    ],
    props: {
      pluginName: name,
      url: url,
      displayName: displayName,
      isLoading: isLoading,
      isEditing: isEditing,
      id: id,
      editKey,
    },
    global: {
      plugins: [router],
    },
  }) as VueWrapper<ComponentPublicInstance<PluginComponentInstance>>;
};

describe('Plugin.vue', () => {
  it('renders correctly with required props', () => {
    const wrapper = generateWrapper(
      'Test Plugin',
      'https://example.com/examplePath',
      'test instance',
      false,
      false,
      100,
    );

    expect(wrapper.find('h3').text()).toBe('Test Plugin');
    expect(wrapper.find('p').text()).toBe('test instance');

    expect(wrapper.find('img').attributes('src')).toBe(
      'https://www.google.com/s2/favicons?domain=https://example.com&sz=128',
    );
  });

  it('renders loading state correctly when isLoading is true', () => {
    const wrapper = mount(PluginComponent, {
      props: {
        pluginName: 'Test Plugin',
        url: 'https://test.com',
        displayName: 'Test',
        isLoading: true,
        isEditing: false,
        id: 100,
        editKey: -2,
      },
    });
    const skeleton = wrapper.find('.ant-skeleton-content');
    expect(skeleton.exists()).toBe(true);
  });

  it('changes state when isLoading updates', async () => {
    const wrapper = mount(PluginComponent, {
      props: {
        pluginName: 'Test Plugin',
        url: 'https://test.com',
        displayName: 'Test',
        isLoading: true,
        isEditing: false,
        id: 100,
        editKey: -1,
      },
    });
    expect(wrapper.find('.ant-skeleton-content').exists()).toBe(true);
    await wrapper.setProps({
      pluginName: 'Test Plugin',
      url: 'https://test.com',
      displayName: 'Test',
      isLoading: false,
      isEditing: false,
      id: 100,
    });
    expect(wrapper.find('.ant-skeleton-content').exists()).toBe(false);
  });

  it('creates correct favicon URL', () => {
    const result = createFaviconURL('https://example.com');
    expect(result).toBe(
      'https://www.google.com/s2/favicons?domain=https://example.com&sz=128',
    );
  });

  it('cuts URL after TLD', () => {
    //Test with https and .com TLD
    const result1 = cutAfterTLD('https://example.com/path/to/resource');
    expect(result1).toBe('https://example.com');

    //Test without https
    const result3 = cutAfterTLD('www.example.com/path/to/resource');
    expect(result3).toBe('www.example.com');

    //Test without www
    const result4 = cutAfterTLD('example.com/path/to/resource');
    expect(result4).toBe('example.com');
  });

  // invalid URL will lead to fallback favicon
  it('returns invalid URL if URL is invalid', () => {
    const result = cutAfterTLD('www.example');
    expect(result).toBe('www.example');
  });

  // create Mock for clipboard API
  const clipboard = { writeText: vi.fn() };
  beforeEach(() => {
    Object.defineProperty(global.navigator, 'clipboard', {
      value: clipboard,
      writable: true,
    });
  });

  it('copies URL to clipboard', async () => {
    const wrapper = generateWrapper(
      'Test Plugin',
      'https://example.com/examplePath',
      'test instance',
      false,
      false,
      100,
    );

    const card = wrapper.findComponent({ name: 'ACard' });
    // check if card exists, if it does, trigger click event
    if (card.exists()) {
      await card.trigger('click');
      expect(clipboard.writeText).toHaveBeenCalledWith(
        'https://example.com/examplePath',
      );
    } else {
      throw new Error('ACard component not found');
    }
  });

  it('opens the link onclick', async () => {
    // Mock window.open
    const windowOpenMock = vi.fn();
    global.window.open = windowOpenMock;
    const wrapper = generateWrapper(
      'Test Plugin',
      'https://example.com/examplePath',
      'Test Plugin Instance 1',
      false,
      false,
      100,
    );
    await wrapper.findComponent({ name: 'ACard' }).trigger('click');
    expect(windowOpenMock).toBeCalledWith(
      'https://example.com/examplePath',
      '_blank',
    );
  });
  it('adds prefix to url before opening if its missing', async () => {
    // Mock window.open
    const windowOpenMock = vi.fn();
    global.window.open = windowOpenMock;
    const wrapper = generateWrapper(
      'Test Plugin',
      'example.com',
      'Test Plugin Instance 1',
      false,
      false,
      100,
    );
    await wrapper.findComponent({ name: 'ACard' }).trigger('click');
    expect(windowOpenMock).toBeCalledWith('https://example.com', '_blank');
  });
});
