import { afterEach } from 'vitest';
import { cleanup } from '@testing-library/vue';
import '@testing-library/jest-dom/vitest';
import { vi } from 'vitest';
import { config, mount } from '@vue/test-utils';

// See https://github.com/vitest-dev/vitest/issues/821
Object.defineProperty(window, 'matchMedia', {
  writable: true,
  value: vi.fn().mockImplementation((query) => ({
    matches: false,
    media: query,
    onchange: null,
    addListener: vi.fn(), // deprecated
    removeListener: vi.fn(), // deprecated
    addEventListener: vi.fn(),
    removeEventListener: vi.fn(),
    dispatchEvent: vi.fn(),
  })),
});

// enable window.matchMedia (maybe needs to be changed)
window.matchMedia =
  // window.matchMedia ??
  function (query: unknown): MediaQueryList {
    return {
      matches: false,
      media: query,
      onchange: null,
      addListener: function () {},
      removeListener: function () {},
      addEventListener: function () {},
      removeEventListener: function () {},
      dispatchEvent: function () {},
    } as unknown as MediaQueryList;
  };

// See https://github.com/NickColley/jest-axe/issues/147
const { getComputedStyle } = window;
window.getComputedStyle = (elt) => getComputedStyle(elt);

// runs a cleanup after each test case (e.g. clearing jsdom)
afterEach(() => {
  cleanup();
});

const findElementByText = (
  wrapper: VueWrapperInstance,
  searchedElement: Parameters<VueWrapperInstance['findAll']>[0],
  text: string,
) => {
  return wrapper
    .findAll(searchedElement)
    .filter((c) => {
      return c.text() === text;
    })
    .at(0);
};

const findComponentByText = (
  wrapper: VueWrapperInstance,
  searchedComponent: Parameters<VueWrapperInstance['findComponent']>[0],
  text: string,
) => {
  return wrapper
    .findAllComponents(searchedComponent)
    .filter((c) => {
      return c.text() === text;
    })
    .at(0);
};

// See https://github.com/vuejs/vue-test-utils/issues/960
type VueWrapperInstance = ReturnType<typeof mount>;
config.plugins.VueWrapper.install(() => {
  return {
    findComponentByText(
      searchedComponent: Parameters<VueWrapperInstance['findComponent']>[0],
      text: string,
    ) {
      return findComponentByText(
        this as VueWrapperInstance,
        searchedComponent,
        text,
      );
    },
    findElementByText(
      searchedElement: Parameters<VueWrapperInstance['findAll']>[0],
      text: string,
    ) {
      return findElementByText(
        this as VueWrapperInstance,
        searchedElement,
        text,
      );
    },
  };
});
