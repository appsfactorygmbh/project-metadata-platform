import { mount } from '@vue/test-utils';
import PluginView from '../PluginView.vue';

describe('PluginView', () => {
  it('renders plugins dynamically', async () => {
    const plugins = [
      { name: 'Test Plugin', url: 'https://example.com' },
      { name: 'Another Plugin', url: 'https://example.org' },
    ];
    const wrapper = mount(PluginView, {
      props: {
        plugins: plugins,
      },
    });
    console.log(wrapper.html());
    expect(wrapper.findAll('.plugins')).toHaveLength(5);
  });
});
