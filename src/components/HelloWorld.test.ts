import { describe, expect, it } from 'vitest';
import { mount } from '@vue/test-utils';
import HelloWorld from './HelloWorld.vue';

describe('HelloWorld.vue', () => {
  it('increments count when button is clicked', async () => {
    const wrapper = mount(HelloWorld, {
      props: {
        msg: 'Test Message',
      },
    });

    expect(wrapper.find('button').text()).toBe('count is 0');

    await wrapper.find('button').trigger('click');

    expect(wrapper.find('button').text()).toBe('count is 1');
  });
});
