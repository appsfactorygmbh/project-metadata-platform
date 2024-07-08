<script setup lang="ts">
  import { MinusCircleOutlined, PlusOutlined } from '@ant-design/icons-vue';
  import { reactive } from 'vue';
  //import type { CreatePluginModel } from '@/models/Plugin';
  import { type FormStore } from '@/components/Form';
  import type { RulesObject } from '@/components/Form/FormStore';
  import type { GlobalPluginFormData } from './';
  import type { ProjectKey } from '@/models/Plugin';

  const { formStore, initialValues } = defineProps<{
    formStore: FormStore;
    initialValues: GlobalPluginFormData;
  }>();

  const modelRef = reactive<GlobalPluginFormData>(initialValues);

  // TODO: add validation for keys
  const rulesRef = reactive<RulesObject<GlobalPluginFormData>>({
    pluginName: [
      {
        required: true,
        message: 'Please insert the plugin name.',
        trigger: 'change',
        type: 'string',
      },
    ],
    keys: [
      {
        required: false,
        message: 'Please insert the plugin key.',
        trigger: 'change',
      },
    ],
  });

  formStore.setModel(modelRef);
  formStore.setRules(rulesRef);

  const formRef = ref();

  watch(
    () => formRef.value,
    () => {
      console.log('formRef', formRef);
    },
    { immediate: true, deep: true },
  );

  const formItemLayout = {
    labelCol: {
      xs: { span: 24 },
      sm: { span: 4 },
    },
    wrapperCol: {
      xs: { span: 24 },
      sm: { span: 20 },
    },
  };

  const formItemLayoutWithOutLabel = {
    wrapperCol: {
      xs: { span: 24, offset: 0 },
      sm: { span: 20, offset: 4 },
    },
  };

  const removeProjectKey = (item: ProjectKey) => {
    const index = modelRef.keys.indexOf(item);
    if (index !== -1) {
      modelRef.keys.splice(index, 1);
    }
  };

  const addProjectKey = () => {
    modelRef.keys.push({
      value: '',
      key: Date.now(),
    });
  };
</script>

<template>
  <a-form v-bind="formItemLayoutWithOutLabel" ref="formRef" :model="modelRef">
    <a-form-item name="pluginName" :no-style="true" :whitespace="true">
      <a-input
        v-model:value="modelRef.pluginName"
        class="inputField"
        placeholder="Plugin Name"
      >
      </a-input>
    </a-form-item>
    <a-form-item
      v-for="(key, index) in modelRef.keys"
      :key="key.key"
      v-bind="index === 0 ? formItemLayout : {}"
      :label="index === 0 ? 'ProjectKeys' : ''"
      :name="['keys', index, 'value']"
      :rules="{
        required: true,
        message: 'Please insert the plugin key.',
        trigger: 'change',
      }"
    >
      <a-input
        v-model:value="key.value"
        placeholder="add the plugin key here"
        style="width: 60%; margin-right: 8px"
      />
      <MinusCircleOutlined
        v-if="modelRef.keys.length > 1"
        class="dynamic-delete-button"
        @click="removeProjectKey(key)"
      />
    </a-form-item>
    <a-form-item v-bind="formItemLayoutWithOutLabel">
      <a-button type="dashed" style="width: 60%" @click="addProjectKey">
        <PlusOutlined />
        Add ProjectKey
      </a-button>
    </a-form-item>
  </a-form>
  <contextHolder></contextHolder>
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
