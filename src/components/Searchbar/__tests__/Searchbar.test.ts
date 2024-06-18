import { mount } from "@vue/test-utils";
import { describe, it, expect } from 'vitest';
import SearchBar from "../SearchBar.vue";

describe('SearchBar.vue', () => {
    it('renders correctly', () => {
        const wrapper = mount(SearchBar);
        expect(wrapper.exists()).toBe(true);
    });
})
