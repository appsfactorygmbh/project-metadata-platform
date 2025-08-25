import 'vue-test-utils';
import { VueWrapper } from '@vue/test-utils';
import type { VueWrapperInstance } from '@vue/test-utils';

declare module '@vue/test-utils' {
  interface VueWrapper<T> {
    findElementByText(
      searchedElement: Parameters<VueWrapper['findAll']>[0],
      text: string,
    ): ReturnType<VueWrapper['find']>;
    findComponentByText(
      searchedComponent: Parameters<VueWrapper['findAllComponents']>[0],
      text: string,
    ): ReturnType<VueWrapper['find']>;
  }
}
