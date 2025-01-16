<script setup lang="ts">
  import { MinusCircleOutlined, PlusOutlined } from '@ant-design/icons-vue';
  import { reactive } from 'vue';
  //import type { CreatePluginModel } from '@/models/Plugin';
  import { type FormStore } from '@/components/Form';
  import {
    type CustomRulesObject,
    type RulesObject,
  } from '@/components/Form/types';
  import type { GlobalPluginFormData } from './';
  import type { GlobalPluginKey } from '@/models/Plugin';
  import _ from 'lodash';

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
    keys: [
      {
        required: false,
        trigger: 'change',
        type: 'array',
      },
    ],
  });

  const customRules = reactive<CustomRulesObject<GlobalPluginFormData, 'keys'>>(
    {
      keys: [
        {
          ruleTarget: 'arrayItem',
          keyProp: 'key',
          validator: (_, value) => {
            if (value.value.length === 0) {
              return Promise.reject('Please insert the key or remove it.');
            }
            return Promise.resolve();
          },
          message: 'Please insert the key or remove it.',
        },
      ],
    },
  );

  formStore.setModel(modelRef);
  formStore.setRules(rulesRef);
  formStore.setCustomRules(customRules);

  const formItemLayout = {
    labelCol: {
      xs: { span: 24 },
      sm: { span: 7 },
    },
    wrapperCol: {
      xs: { span: 24 },
      sm: { span: 20 },
    },
  };

  const formItemLayoutWithOutLabel = {
    wrapperCol: {
      xs: { span: 24, offset: 0 },
      sm: { span: 20, offset: 0 },
    },
  };

  const removePluginKey = (item: GlobalPluginKey) => {
    const index = modelRef.keys.indexOf(item);
    if (index !== -1) {
      modelRef.keys.splice(index, 1);
    }
  };

  const addPluginKey = () => {
    const key = Date.now();
    modelRef.keys.push({
      archived: false,
      value: '',
      key: key,
    });
    formStore.setRules(rulesRef);
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
      :no-style="false"
      :whitespace="true"
      v-bind="formStore.validateInfos.pluginName"
    >
      <a-input
        v-model:value="modelRef.pluginName"
        class="inputField"
        placeholder="Plugin Name"
      />
    </a-form-item>
    <a-form-item
      v-for="(key, index) in modelRef.keys"
      :key="key.key"
      :label="index === 0 ? 'Project Keys' : ' '"
      :colon="false"
      :name="['keys', index, 'value']"
      v-bind="_.merge(formStore.validateInfos[key.key], formItemLayout)"
    >
      <a-input
        v-model:value="key.value"
        placeholder="add the plugin key here"
        style="width: 60%; margin-right: 8px"
      />
      <MinusCircleOutlined
        v-if="modelRef.keys.length > 0"
        data-test="dynamic-delete-button"
        class="dynamic-delete-button"
        @click="
          () => {
            removePluginKey(key);
          }
        "
      />
    </a-form-item>
    <a-row
      v-if="false"
      style="display: flex; justify-content: center"
    >
      <a-form-item v-bind="formItemLayoutWithOutLabel">
        <a-button
          type="dashed"
          @click="addPluginKey"
        >
          <PlusOutlined />
          Add PluginKey
        </a-button>
      </a-form-item>
    </a-row>
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
