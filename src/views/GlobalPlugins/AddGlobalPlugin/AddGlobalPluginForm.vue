<script setup lang="ts">
  import { defineProps, reactive, ref } from 'vue';
  import type { FormType } from '@/components/Modal/FormTypes.ts';
  import type { FormInstance, SelectProps } from 'ant-design-vue';

  const { form } = defineProps<{
    form: FormType;
  }>();


  const formRef = ref<FormInstance>();

  const formItemLayoutWithOutLabel = {
    wrapperCol: {
      xs: { span: 24, offset: 0 },
      sm: { span: 20, offset: 4 },
    },
  };

  const dynamicValidateForm = reactive<{ pluginName: string; pluginUrl: string, globalPlugin: string }>({
    pluginName: '',
    pluginUrl: '',
    globalPlugin: ''
  });

  const options = ref<SelectProps['options']>([
    { value: 'jack', label: 'Jack' },
    { value: 'lucy', label: 'Lucy' },
    { value: 'tom', label: 'Tom' },
  ]);

  const handleChange = (value: string) => {
    console.log(`selected ${value}`);
  };
  const handleBlur = () => {
    console.log('blur');
  };
  const handleFocus = () => {
    console.log('focus');
  };
  const filterOption = (input: string, option: any) => {
    return option.value.toLowerCase().indexOf(input.toLowerCase()) >= 0;
  };
</script>

<template>
  <a-form
    ref="formRef"
    :form="form"
    :model="dynamicValidateForm"
    v-bind="formItemLayoutWithOutLabel"
  >
    <a-form-item
      name="globalPlugin"
      :rules="[{ required: true, whitespace: true }]"
      class="column"
      :no-style="true"
      :whitespace="true"
    >
      <a-select
        v-model:value="dynamicValidateForm.globalPlugin"
        class="inputField"
        show-search
        placeholder="Select a global Plugin"
        :options="options"
        :filter-option="filterOption"
        @focus="handleFocus"
        @blur="handleBlur"
        @change="handleChange"
      ></a-select>
    </a-form-item>
    <a-form-item
      name="pluginName"
      :rules="[{ required: true, whitespace: true }]"
      class="column"
      :no-style="true"
      :whitespace="true"
    >
      <a-input
        v-model:value="dynamicValidateForm.pluginName"
        class="inputField"
        placeholder="Plugin Name"
        :rules="[{ required: true, whitespace: true }]"
      >
      </a-input>
    </a-form-item>
    <a-form-item
      name="pluginUrl"
      :rules="[{ required: true, whitespace: true }]"
      class="column"
      :no-style="true"
      :whitespace="true"
    >
      <a-input
        v-model:value="dynamicValidateForm.pluginUrl"
        class="inputField"
        placeholder="Plugin URL"
        :rules="[{ required: true, whitespace: true }]"
      >
      </a-input>
    </a-form-item>
  </a-form>
</template>

<style>
  .inputField {
    width: 100%;
    margin: 10px 0 10px 0;
  }
</style>
