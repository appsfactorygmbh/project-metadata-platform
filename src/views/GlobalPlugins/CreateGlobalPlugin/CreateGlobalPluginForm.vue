<script setup lang="ts">
  import type { FormStore, FormSubmitType } from '@/components/Form';
  import { useGlobalPluginsStore } from '@/store';
  import { reactive } from 'vue';
  import GlobalPluginForm from '../GlobalPluginForm/GlobalPluginForm.vue';
  import type { GlobalPluginFormData } from '../GlobalPluginForm';
  import type { CreatePluginRequest } from '@/api/generated';
  import { message } from 'ant-design-vue';

  const { formStore } = defineProps<{
    formStore: FormStore;
  }>();

  const globalPluginStore = useGlobalPluginsStore();

  const onSubmit: FormSubmitType = (fields: CreatePluginRequest) => {
    globalPluginStore
      .create(fields)
      .then(() => message.success('Plugin created successfully'))
      .catch((error) => {
        message.error((error as Error).message ?? 'An error occurred');
        throw error;
      });
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
</template>
