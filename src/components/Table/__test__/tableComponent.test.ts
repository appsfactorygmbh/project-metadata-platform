import { mount } from "@vue/test-utils";
import Table from "../tableComponent.vue";
import { describe, it, expect } from "vitest";

describe("tableComponent.vue", () => {
    it("renders correctly with 4 columns", async () => {
        const wrapper = mount(Table, {
            props: {
                paneWidth: 200,
                paneHeight: 800
            }
        });
        //console.log(wrapper.html());
        
        expect(wrapper.findAll(".ant-table-column-sorter")).toHaveLength(4);
    });
    it("shows the data entries in alphabetical order", () => {

    });
    it("hides columns when the pane width is not large", () => {

    })
})