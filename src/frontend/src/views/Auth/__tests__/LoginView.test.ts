import { describe, expect, it } from 'vitest';
import { mount, VueWrapper } from '@vue/test-utils';
import { LoginView, SSOAuthButton } from '..';
import { setActivePinia } from 'pinia';
import { createTestingPinia } from '@pinia/testing';
import router from '@/router';
import initAuth from '@/auth';
import { msalInstance } from '@/services/msalService';

const mockRoute = {
  path: '/mock-path',
  query: { redirect: '/redirect-path' },
  params: {},
  hash: '',
  fullPath: '/mock-path',
  matched: [],
  meta: {},
  redirectedFrom: undefined,
};

const mockRouter = {
  push: vi.fn(),
};

vi.mock('vue-router', async (importOriginal) => {
  const actual = await importOriginal<typeof import('vue-router')>();
  return {
    ...actual,
    useRouter: () => mockRouter,
    useRoute: () => mockRoute,
  };
});

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
  it("should have a azure login button", async () => {
    expect(wrapper.findComponent(SSOAuthButton).exists()).toBe(true);
  });
  it("should call login redirect when clicking login button", async()=>{
    const loginButton = wrapper.findComponent(SSOAuthButton);

        await loginButton.trigger('click');

        expect(msalInstance.loginRedirect).toHaveBeenCalledTimes(1);
  })
});
