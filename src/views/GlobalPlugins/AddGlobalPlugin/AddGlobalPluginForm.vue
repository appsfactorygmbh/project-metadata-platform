<script setup lang="ts">
  import { defineProps, onBeforeMount, reactive, ref, toRaw } from 'vue';
  import type { FormType } from '@/components/Modal/FormTypes.ts';
  import type { FormInstance, SelectProps } from 'ant-design-vue';
  import { pluginStoreSymbol } from '@/store/injectionSymbols';
  import { inject } from 'vue';
  import type { GlobalPluginModel } from '@/models/Plugin';
  import type { LabeledValue, SelectValue } from 'ant-design-vue/lib/select';

  const pluginStore = inject(pluginStoreSymbol);
  const options = ref<SelectProps['options']>([]);

  onBeforeMount(async () => {
    pluginStore!.setLoading(true);
    await pluginStore!.fetchGlobalPlugins();
    options.value = toRaw(pluginStore?.getGlobalPlugins)!.map((plugin: GlobalPluginModel) => {
      return {
        value: plugin.name,
        label: plugin.name
      }
    });
  });

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

  const dynamicValidateForm = reactive<{ pluginName: string; pluginUrl: string, globalPlugin: string, inputsDisabled: boolean }>({
    pluginName: '',
    pluginUrl: '',
    globalPlugin: '',
    inputsDisabled: true
  });

  const handleChange = (value: SelectValue) => {
    console.log(`selected ${value}`);
    dynamicValidateForm.inputsDisabled = false;
  };

  const filterOption = (input: string, option: LabeledValue) => {
    return option.value.valueOf().toString().toLowerCase().indexOf(input.toLowerCase()) >= 0;
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
        :disabled="dynamicValidateForm.inputsDisabled"
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
        :disabled="dynamicValidateForm.inputsDisabled"
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
