import { mount } from '@vue/test-utils';
import Plugin from '../src/components/Plugin.vue';
import { describe, it, expect} from 'vitest';

describe('Plugin.vue', () => {
  it('renders correctly with required props', () => {
    const wrapper = mount(Plugin, {
      props: {
        pluginName: 'Test Plugin',
        url: 'https://example.com'
      }
    });

    expect(wrapper.find('h3').text()).toBe('Test Plugin');
    expect(wrapper.find('a').attributes('href')).toBe('https://example.com');
    expect(wrapper.find('a').text()).toBe('https://example.com');
  });

  it('creates correct favicon URL on created hook', () => {
    const wrapper = mount(Plugin, {
      props: {
        pluginName: 'Test Plugin',
        url: 'https://example.com'
      }
    });

    expect(wrapper.vm.faviconUrl).toBe('https://www.google.com/s2/favicons?domain=https://example.com&sz=128');
  });

  it('cuts URL after TLD', () => {
    const wrapper = mount(Plugin, {
      props: {
        pluginName: 'Test Plugin',
        url: 'https://example.com/path/to/resource'
      }
    });

    const result = wrapper.vm.cutAfterTLD('https://example.com/path/to/resource');
    expect(result).toBe('https://example.com');
  });
});
