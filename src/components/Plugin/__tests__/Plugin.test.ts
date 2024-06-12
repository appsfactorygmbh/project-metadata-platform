import { mount } from '@vue/test-utils';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { cutAfterTLD, createFaviconURL } from '../editURL';
import PluginComponent from '../PluginComponent.vue';

const generateWrapper = (name: string, url: string) => {
  return mount(PluginComponent, {
    props: {
      pluginName: name,
      url: url,
    },
  });
};

describe('Plugin.vue', () => {
  it('renders correctly with required props', () => {
    const wrapper = generateWrapper(
      'Test Plugin',
      'https://example.com/examplePath',
    );

    expect(wrapper.find('h3').text()).toBe('Test Plugin');
    expect(wrapper.find('a').attributes('href')).toBe(
      'https://example.com/examplePath',
    );
    expect(wrapper.find('a').text()).toBe('https://example.com/examplePath');

    expect(wrapper.find('img').attributes('src')).toBe(
      'https://www.google.com/s2/favicons?domain=https://example.com&sz=128',
    );
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
});
