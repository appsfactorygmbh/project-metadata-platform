<script setup lang="ts">
  import { type FormSubmitType } from '@/components/Form';
  import { message } from 'ant-design-vue';
  import { useGlobalPluginsStore } from '@/store';
  import { reactive } from 'vue';
  //import type { CreatePluginModel } from '@/models/Plugin';
  import { type FormStore } from '@/components/Form';
  import GlobalPluginForm from '../GlobalPluginForm/GlobalPluginForm.vue';
  import type { GlobalPluginFormData } from '../GlobalPluginForm';

  const { formStore } = defineProps<{
    formStore: FormStore;
  }>();

  const globalPluginStore = useGlobalPluginsStore();

  const onSubmit: FormSubmitType = async (fields) => {
    try {
      await globalPluginStore.create(fields)
      message.success('Created global Plugin successfully', 4);
    } catch (error) {
      message.error((error as Error).message, 4)
    }
  };

  formStore.setOnSubmit(onSubmit);

  const initialValues = reactive<GlobalPluginFormData>({
    pluginName: '',
    baseUrl: '',
    keys: [],
  });
</script>

<template>
  <GlobalPluginForm :form-store="formStore" :initial-values="initialValues" />
  <contextHolder />
</template>
