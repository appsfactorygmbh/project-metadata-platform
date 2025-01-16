<script setup lang="ts">
  import { type FormStore, type FormSubmitType } from '@/components/Form';
  import { notification } from 'ant-design-vue';
  import { projectEditStoreSymbol } from '@/store/injectionSymbols';
  import { inject, onBeforeMount, reactive, ref, toRaw } from 'vue';
  import type { SelectProps } from 'ant-design-vue';
  import type {
    GlobalPluginModel,
    PluginEditModel,
    PluginModel,
  } from '@/models/Plugin';
  import type { LabeledValue, SelectValue } from 'ant-design-vue/lib/select';
  import type { RulesObject } from '@/components/Form/types';
  import type { AddPluginFormData } from './AddPluginFormData.ts';
  import { useGlobalPluginsStore } from '@/store/GlobalPluginStore.ts';
  import _ from 'lodash';

  const { formStore, initialValues } = defineProps<{
    formStore: FormStore;
    initialValues: AddPluginFormData;
  }>();

  const globalPluginStore = useGlobalPluginsStore();
  const projectEditStore = inject(projectEditStoreSymbol);
  const options = ref<SelectProps['options']>([]);

  onBeforeMount(async () => {
    await globalPluginStore?.fetchAll();
    options.value = toRaw(globalPluginStore?.getGlobalPlugins)
      ?.filter((plugin) => !plugin.isArchived)
      .map((plugin: GlobalPluginModel) => {
        return {
          value: plugin.name,
          label: plugin.name,
        };
      });
  });

  const [notificationApi, contextHolder] = notification.useNotification();

  const onSubmit: FormSubmitType = (fields) => {
    try {
      console.log(fields);
      const pluginNumber: number | undefined =
        globalPluginStore?.getGlobalPlugins.find(
          (plugin) => plugin.name === toRaw(fields).globalPlugin,
        )?.id;
      if (pluginNumber === undefined) {
        return;
      }

      const pluginDef: PluginModel = {
        id: pluginNumber,
        pluginName: toRaw(fields).globalPlugin,
        displayName: toRaw(fields).pluginName,
        url: toRaw(fields).pluginUrl,
      };
      addPlugin(pluginDef);
    } catch {
      notificationApi.error({
        message: 'An error occurred. The plugin could not be created',
      });
      console.log('fehler');
    }
  };

  const addPlugin = (pluginDef: PluginModel) => {
    const index = projectEditStore?.initialAdd(pluginDef);

    if (index !== undefined) {
      const newPlugin: PluginEditModel = {
        ...pluginDef,
        editKey: index,
        isDeleted: false,
      };
      projectEditStore?.addNewPlugin(newPlugin);
    }
  };

  const formItemLayoutWithOutLabel = {
    wrapperCol: {
      xs: { span: 24, offset: 0 },
      sm: { span: 20, offset: 4 },
    },
  };

  const dynamicValidateForm = reactive<AddPluginFormData>(initialValues);

  const rulesRef = reactive<RulesObject<AddPluginFormData>>({
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
    globalPlugin: [
      {
        required: true,
        message: 'Please select a global plugin.',
        trigger: 'change',
        type: 'string',
      },
    ],
    inputsDisabled: [
      {
        required: false,
      },
    ],
  });

  const handleChange = (value: SelectValue) => {
    console.log(`selected ${_.toString(value)}`);
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
      />
    </a-form-item>
    <a-form-item
      name="pluginName"
      class="column"
      :no-style="true"
      :whitespace="true"
      :rules="rulesRef.pluginName"
    >
      <a-input
        id="inputAddPluginPluginName"
        v-model:value="dynamicValidateForm.pluginName"
        class="inputField"
        placeholder="Plugin Name"
        :disabled="dynamicValidateForm.inputsDisabled"
        :rules="rulesRef.pluginName"
      />
    </a-form-item>
    <a-form-item
      name="pluginUrl"
      class="column"
      :no-style="true"
      :whitespace="true"
      :rules="rulesRef.pluginName"
    >
      <a-input
        id="inputAddPluginPluginUrl"
        v-model:value="dynamicValidateForm.pluginUrl"
        class="inputField"
        placeholder="Plugin URL"
        :disabled="dynamicValidateForm.inputsDisabled"
        :rules="rulesRef.pluginName"
      />
    </a-form-item>
  </a-form>
  <contextHolder />
</template>
