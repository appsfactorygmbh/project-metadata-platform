<script setup lang="ts">
  import { defineProps, reactive, ref, type UnwrapRef, watch } from 'vue';
  import type { FormType } from '@/components/Modal/FormTypes.ts';
  import type { CreateProjectModel } from '@/models/Project';
  import { MinusCircleOutlined, PlusOutlined } from '@ant-design/icons-vue';
  import type { FormInstance } from 'ant-design-vue';
  //import type { CreatePluginModel } from '@/models/Plugin';

  const { form } = defineProps<{
    form: FormType;
  }>();

  interface Key {
    value: string;
    key: number;
  }

  const formRef = ref<FormInstance>();

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

  const dynamicValidateForm = reactive<{ pluginName: string; keys: Key[] }>({
    pluginName: '',
    keys: [],
  });

  const removeKey = (item: Key) => {
    const index = dynamicValidateForm.keys.indexOf(item);
    if (index !== -1) {
      dynamicValidateForm.keys.splice(index, 1);
    }
  };

  const addKey = () => {
    dynamicValidateForm.keys.push({
      value: '',
      key: Date.now(),
    });
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
      v-for="(key, index) in dynamicValidateForm.keys"
      :key="key.key"
      v-bind="index === 0 ? formItemLayout : {}"
      :label="index === 0 ? 'Keys' : ''"
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
        v-if="dynamicValidateForm.keys.length > 1"
        class="dynamic-delete-button"
        @click="removeKey(key)"
      />
    </a-form-item>
    <a-form-item v-bind="formItemLayoutWithOutLabel">
      <a-button type="dashed" style="width: 60%" @click="addKey">
        <PlusOutlined />
        Add Key
      </a-button>
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
