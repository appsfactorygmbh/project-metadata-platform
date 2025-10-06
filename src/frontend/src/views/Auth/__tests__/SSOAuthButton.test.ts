import { describe, expect, it } from 'vitest';
import { mount } from '@vue/test-utils';
import { SSOAuthButton} from '../';

describe('SSOAuthButton.vue', () => {
  it('renders correctly', () => {
    const wrapper = mount(SSOAuthButton);

    expect(wrapper.exists()).toBe(true);
  });
});
