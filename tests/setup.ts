import { afterEach } from 'vitest';
import { cleanup } from '@testing-library/vue';
import '@testing-library/jest-dom/vitest';
import { vi } from 'vitest';

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

// runs a cleanup after each test case (e.g. clearing jsdom)
afterEach(() => {
  cleanup();
});

// enable window.matchMedia (maybe needs to be changed)
window.matchMedia =
  window.matchMedia ||
  function (): MediaQueryList {
    return {
      matches: false,
      addListener: function () {},
      removeListener: function () {},
    } as unknown as MediaQueryList;
  };
