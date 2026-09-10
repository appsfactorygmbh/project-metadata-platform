<script setup lang="ts">
  import { FontColorsOutlined, LinkOutlined } from '@ant-design/icons-vue';
  import { reactive } from 'vue';
  import { type FormStore } from '@/components/Form';
  import { type RulesObject } from '@/components/Form/types';
  import type { GlobalPluginFormData } from './';

  const { formStore, initialValues } = defineProps<{
    formStore: FormStore;
    initialValues: GlobalPluginFormData;
  }>();

  const modelRef = reactive<GlobalPluginFormData>(initialValues);

  const rulesRef = reactive<
    RulesObject<GlobalPluginFormData | Record<string, unknown>>
  >({
    pluginName: [
      {
        required: true,
        message: 'Please insert the plugin name.',
        trigger: 'change',
        type: 'string',
      },
    ],
    baseUrl: [
      {
        required: true,
        message: 'Please insert the base url.',
        trigger: 'change',
        type: 'string',
      },
    ],
  });

  formStore.setModel(modelRef);
  formStore.setRules(rulesRef);

  const formItemLayoutWithOutLabel = {
    wrapperCol: {
      xs: { span: 24, offset: 0 },
      sm: { span: 20, offset: 0 },
    },
  };
</script>

<template>
  <a-form
    v-bind="formItemLayoutWithOutLabel"
    :model="modelRef"
    layout="horizontal"
  >
    <a-form-item
      name="pluginName"
      :no-style="true"
      :whitespace="true"
      v-bind="formStore.validateInfos.pluginName"
    >
      <a-input
        v-model:value="modelRef.pluginName"
        class="inputField"
        placeholder="Plugin Name"
      >
        <template #prefix>
          <FontColorsOutlined />
        </template>
      </a-input>
    </a-form-item>
    <a-form-item
      name="baseUrl"
      :no-style="true"
      :whitespace="true"
      v-bind="formStore.validateInfos.baseUrl"
    >
      <a-input
        v-model:value="modelRef.baseUrl"
        class="inputField"
        placeholder="Base Url"
      >
        <template #prefix>
          <LinkOutlined />
        </template>
      </a-input>
    </a-form-item>
  </a-form>
</template>

<style>
  .dynamic-delete-button {
    cursor: pointer;
    position: relative;
    top: 4px;
    font-size: 24px;
    color: #999;
    transition: all 0.3s;
  }
  .dynamic-delete-button:hover {
    color: #777;
  }
  .dynamic-delete-button[disabled] {
    cursor: not-allowed;
    opacity: 0.5;
  }
  .inputField {
    width: 100%;
    margin: 10px 0 10px 0;
  }
</style>
