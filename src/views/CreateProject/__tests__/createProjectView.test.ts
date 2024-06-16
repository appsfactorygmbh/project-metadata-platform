import { mount } from '@vue/test-utils';
import { describe, it, expect } from 'vitest';
import createProjectView from '../createProjectView.vue';
import { PlusOutlined, ShoppingOutlined, TeamOutlined, BankOutlined, UserOutlined } from '@ant-design/icons-vue';
import { nextTick } from 'vue';

const generateWrapper = () => {
    return mount(createProjectView, {
        global: {
            components: {
                PlusOutlined, ShoppingOutlined, TeamOutlined, BankOutlined, UserOutlined
            }
        }
    });
};

describe('createProjectView.vue', () => {
    it('renders the modal with input fields and icons', async () => {
        const wrapper = generateWrapper();

        // Check if the modal is not open initially
        expect(wrapper.findComponent({name: "AModal"}).exists()).toBe(false);

        // Simulate clicking the float button to open the modal
        await wrapper.findComponent({name: "AFloatButton"}).trigger('click');
        await nextTick();

        // Check if the modal is open
        expect(wrapper.findComponent({name: "AModal"}).exists()).toBe(true);

        // Check if input fields and icons are rendered
        expect(wrapper.findComponent(ShoppingOutlined).exists()).toBe(true);
        expect(wrapper.findComponent(TeamOutlined).exists()).toBe(true);
        expect(wrapper.findComponent(BankOutlined).exists()).toBe(true);
        expect(wrapper.findComponent(UserOutlined).exists()).toBe(true);
    });

    it('validates input fields and sets status correctly', async () => {
        const wrapper = generateWrapper();

        // Simulate clicking the float button to open the modal
        await wrapper.findComponent({name: "AFloatButton"}).trigger('click');
        await nextTick();

        // Simulate clicking the OK button
        await wrapper.findComponent({name: "AButton"}).trigger('click');
        await nextTick();


        // Check if the status of each input field is set to error
        expect(wrapper.find('#businessUnitField').attributes('status')).toBe('error');
        expect(wrapper.find('#teamNumberField').attributes('status')).toBe('error');
        expect(wrapper.find('#departmentField').attributes('status')).toBe('error');
        expect(wrapper.find('#clientNameField').attributes('status')).toBe('error');

        // Fill in the input fields
        await wrapper.find('#businessUnitField input').setValue('Business Unit');
        await wrapper.find('#teamNumberField input').setValue('Team Number');
        await wrapper.find('#departmentField input').setValue('Department');
        await wrapper.find('#clientNameField input').setValue('Client Name');
        await nextTick();

        // Simulate clicking the OK button again
        await wrapper.findComponent({name: "AButton"}).trigger('click');
        await nextTick();

        // Check if the status of each input field is reset
        expect(wrapper.find('#businessUnitField').attributes('status')).toBe('');
        expect(wrapper.find('#teamNumberField').attributes('status')).toBe('');
        expect(wrapper.find('#departmentField').attributes('status')).toBe('');
        expect(wrapper.find('#clientNameField').attributes('status')).toBe('');

        // Check if the modal is closed
        expect(wrapper.findComponent({name: "AModal"}).exists()).toBe(false);
    });

    it('closes the modal if all fields are filled', async () => {
        const wrapper = generateWrapper();

        // Simulate clicking the float button to open the modal
        await wrapper.findComponent({name: "AFloatButton"}).trigger('click');
        await nextTick();

        // Fill in the input fields
        await wrapper.findComponent('#businessUnit').setValue('Business Unit');
        await wrapper.find('#teamNumberField input').setValue('Team Number');
        await wrapper.find('#departmentField input').setValue('Department');
        await wrapper.find('#clientNameField input').setValue('Client Name');
        await nextTick();

        // Simulate clicking the OK button
        await wrapper.findComponent({name: "AButton"}).trigger('click');
        await nextTick();

        // Check if the modal is closed
        expect(wrapper.findComponent({name: "AModal"}).exists()).toBe(false);
    });
});
