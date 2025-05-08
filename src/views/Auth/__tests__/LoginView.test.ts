import { describe, expect, it } from 'vitest';
import { mount, VueWrapper } from '@vue/test-utils';
import { LoginView } from '..';
import { setActivePinia } from 'pinia';
import { createTestingPinia } from '@pinia/testing';
import router from '@/router';
import initAuth from '@/auth';

describe('LoginView.vue', () => {
  let wrapper: VueWrapper;
  beforeEach(() => {
    setActivePinia(
      createTestingPinia({
        stubActions: false,
      }),
    );
    wrapper = mount(LoginView, {
      plugins: [router, initAuth()],
      global: {
        stubs: {
          RouterLink: {
            template: '<span />',
          },
        },
      },
    });
  });

  it('renders correctly', () => {
    expect(wrapper.exists()).toBe(true);
  });

  it('should have a login form', () => {
    expect(wrapper.find('form').exists()).toBe(true);
    expect(wrapper.findComponent({ name: 'LoginForm' }).exists()).toBe(true);
  });
});
