<script setup lang="ts">
  import { type FormSubmitType } from '@/components/Form';
  import { notification } from 'ant-design-vue';
  import { pluginStoreSymbol } from '@/store/injectionSymbols';
  import { type FormStore } from '@/components/Form';
  import {
    defineProps,
    onBeforeMount,
    ref,
    toRaw,
    inject,
    reactive,
  } from 'vue';
  import type { SelectProps } from 'ant-design-vue';
  import type { GlobalPluginModel } from '@/models/Plugin';
  import type { LabeledValue, SelectValue } from 'ant-design-vue/lib/select';
  import type { RulesObject } from '@/components/Form/FormStore.ts';
  import type { AddPluginFormData } from './AddPluginFormData.ts';

  const { formStore, initialValues } = defineProps<{
    formStore: FormStore;
    initialValues: AddPluginFormData;
  }>();

  const pluginStore = inject(pluginStoreSymbol);
  const options = ref<SelectProps['options']>([]);

  onBeforeMount(async () => {
    pluginStore?.setLoading(true);
    await pluginStore?.fetchGlobalPlugins();
    options.value = toRaw(pluginStore?.getGlobalPlugins)?.map(
      (plugin: GlobalPluginModel) => {
        return {
          value: plugin.name,
          label: plugin.name,
        };
      },
    );
  });

  const [notificationApi, contextHolder] = notification.useNotification();

  const onSubmit: FormSubmitType = (fields) => {
    try {
      console.log(fields);
    } catch {
      notificationApi.error({
        message: 'An error occurred. The plugin could not be created',
      });
      console.log('fehler');
    }
  };

  const formItemLayoutWithOutLabel = {
    wrapperCol: {
      xs: { span: 24, offset: 0 },
      sm: { span: 20, offset: 4 },
    },
  };

  const dynamicValidateForm = reactive<AddPluginFormData>(initialValues);

  const rulesRef = reactive<
    RulesObject<{ pluginName: string; pluginUrl: string }>
  >({
    pluginName: [
      {
        required: true,
        message: 'Please insert the plugin name.',
        trigger: 'change',
        type: 'string',
      },
    ],
    pluginUrl: [
      {
        required: true,
        message: 'Please insert the plugin URL.',
        trigger: 'change',
        type: 'string',
      },
    ],
  });

  const handleChange = (value: SelectValue) => {
    console.log(`selected ${value}`);
    dynamicValidateForm.inputsDisabled = false;
  };

  const filterOption = (input: string, option: LabeledValue) => {
    return (
      option.value
        .valueOf()
        .toString()
        .toLowerCase()
        .indexOf(input.toLowerCase()) >= 0
    );
  };

  formStore.setOnSubmit(onSubmit);
  formStore.setModel(dynamicValidateForm);
  formStore.setRules(rulesRef);

  const formRef = ref();
</script>

<template>
  <a-form
    ref="formRef"
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
        id="inputAddPluginPluginName"
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
        id="inputAddPluginPluginUrl"
        v-model:value="dynamicValidateForm.pluginUrl"
        class="inputField"
        placeholder="Plugin URL"
        :rules="[{ required: true, whitespace: true }]"
        :disabled="dynamicValidateForm.inputsDisabled"
      >
      </a-input>
    </a-form-item>
  </a-form>
  <contextHolder></contextHolder>
</template>
