import { afterEach } from 'vitest';
import { cleanup } from '@testing-library/vue';
import '@testing-library/jest-dom/vitest';

// runs a cleanup after each test case (e.g. clearing jsdom)
afterEach(() => {
  cleanup();
});

// enable window.matchMedia (maybe needs to be changed)
if (typeof window !== 'undefined') {
  window.matchMedia =
    window.matchMedia ||
    function (): MediaQueryList {
      return {
        matches: false,
        addListener: function () {},
        removeListener: function () {},
      } as unknown as MediaQueryList;
    };
}
