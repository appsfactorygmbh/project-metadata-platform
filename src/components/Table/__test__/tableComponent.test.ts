import { mount } from "@vue/test-utils";
import Table from "../tableComponent.vue";
import { describe, it, expect } from "vitest";

describe("tableComponent.vue", () => {
    it("hides columns when the pane width is not large enough", () => {
        const wrapper = mount(Table, {
            props: {
                paneWidth: 800,
                paneHeight: 800,
                isTest: true
            },
        });

        expect(wrapper.find(".ant-table-column-sorters").isVisible()).toBe(true)

    });
    it("shows the data entries in alphabetical order", () => {
        const wrapper = mount(Table, {
            props: {
                paneWidth: 800,
                paneHeight: 800,
                isTest: true
            },
        });

        expect(wrapper.findAll(".ant-table-row")[0].find(".ant-table-cell").text()).toBe("A")
        expect(wrapper.findAll(".ant-table-row")[1].find(".ant-table-cell").text()).toBe("B")
        expect(wrapper.findAll(".ant-table-row")[2].find(".ant-table-cell").text()).toBe("C")

    });
    it("renders correctly with 4 columns", () => {
        const wrapper = mount(Table, {
            props: {
                paneWidth: 200,
                paneHeight: 800,
                isTest: true
            },
        });
        console.log(wrapper.html())

        expect(wrapper.findAll(".ant-table-column-sorters")).toHaveLength(4);
    });
})