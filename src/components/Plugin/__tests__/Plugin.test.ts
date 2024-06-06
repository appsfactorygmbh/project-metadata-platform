import { mount } from '@vue/test-utils';
import Plugin from '../Plugin.vue';
import { describe, it, expect } from 'vitest';
import { cutAfterTLD } from '../editURL';

const generateWrapper = (name: string, url:string) => {
  return mount(Plugin, {
      props: {
      pluginName: name,
      url: url,
      },
  });

}

describe('Plugin.vue', () => {
  it('renders correctly with required props', () => {
    const wrapper= generateWrapper('Test Plugin', 'https://example.com');

    expect(wrapper.find('h3').text()).toBe('Test Plugin');
    expect(wrapper.find('a').attributes('href')).toBe('https://example.com');
    expect(wrapper.find('a').text()).toBe('https://example.com');
  });

  it('creates correct favicon URL on created hook', () => {
    const wrapper= generateWrapper('Test Plugin', 'https://example.com');

    expect(wrapper.vm.faviconUrl).toBe(
      'https://www.google.com/s2/favicons?domain=https://example.com&sz=128',
    );
  });

  it('cuts URL after TLD', () => {
    
    const result1 = cutAfterTLD(
      'https://example.com/path/to/resource',
    );
    expect(result1).toBe('https://example.com');

    const result2 = cutAfterTLD(
        'https://example.de/path/to/resource',
    );
    expect(result2).toBe('https://example.de');

    const result3 = cutAfterTLD(
        'www.example.com/path/to/resource',
    );
    expect(result3).toBe('www.example.com');

  });
});
