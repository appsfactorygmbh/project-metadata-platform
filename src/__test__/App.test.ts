import { describe, it, expect } from "vitest";
import { mount } from "@vue/test-utils";
import App from "../App.vue";

describe("App.vue", () => {
    it("renders correctly with a proportion of 1:4", () => {
        const wrapper = mount(App);

        expect(wrapper.find("#pane1").attributes("size")).toBe("25");
        expect(wrapper.find("#pane2").attributes("size")).toBe("75");
    });

    it("has a min-size for every pane", () => {
        const wrapper = mount(App);

        expect(wrapper.find("#pane1").attributes("min-size")).toBe("20");
        expect(wrapper.find("#pane2").attributes("min-size")).toBe("1");
    });
});