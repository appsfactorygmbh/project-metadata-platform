<script setup lang="ts">
  import { MinusCircleOutlined, PlusOutlined } from '@ant-design/icons-vue';
  import { type FormSubmitType } from '@/components/Form';
  import { notification } from 'ant-design-vue';
  import { pluginStoreSymbol } from '@/store/injectionSymbols';
  import { inject, reactive, onMounted } from 'vue';
  //import type { CreatePluginModel } from '@/models/Plugin';
  import { type FormStore } from '@/components/Form';
  import type { RulesObject } from '@/components/Form/FormStore';
  import { useRoute } from 'vue-router';
  import { pluginService } from '@/services';

  type ProjectKey = {
    value: string;
    key: number;
  };

  type FormData = {
    pluginName: string;
    keys: ProjectKey[];
  };

  const { formStore } = defineProps<{
    formStore: FormStore;
  }>();

  const pluginStore = inject(pluginStoreSymbol);

  const [notificationApi, contextHolder] = notification.useNotification();

  const onSubmit: FormSubmitType = (fields) => {
    try {
      console.log(fields);
      pluginStore?.createPlugin(fields);
    } catch {
      notificationApi.error({
        message: 'An error occurred. The plugin could not be created',
      });
      console.log('fehler');
    }
  };

  const modelRef = reactive<FormData>({
    pluginName: '',
    keys: [],
  });

  onMounted(async () => {
    const route = useRoute();
    const pluginId = route.params.pluginId;
    if (typeof pluginId === 'string') {
      const numericPluginId = parseInt(pluginId, 10);
      if (!isNaN(numericPluginId)) {
        const globalPluginData =
          await pluginService.fetchGlobalPluginData(numericPluginId);
        modelRef.pluginName = globalPluginData.pluginName;
        const keysArray = Object.entries(globalPluginData.keys).map(
          ([, value], index) => ({
            key: index,
            value: value as string,
          }),
        );
        modelRef.keys = keysArray;
      }
    }
  });

  const rulesRef = reactive<RulesObject<FormData>>({
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

  formStore.setOnSubmit(onSubmit);
  formStore.setModel(modelRef);
  formStore.setRules(rulesRef);

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
  <a-form v-bind="formItemLayoutWithOutLabel" :model="modelRef">
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
