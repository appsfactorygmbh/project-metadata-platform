<script setup lang="ts">
  import type { FormStore, FormSubmitType } from '@/components/Form';
  import { useGlobalPluginStore } from '@/store';
  import { reactive } from 'vue';
  import GlobalPluginForm from '../GlobalPluginForm/GlobalPluginForm.vue';
  import type { GlobalPluginFormData } from '../GlobalPluginForm';
  import type { CreatePluginRequest } from '@/api/generated';
  import { App } from 'ant-design-vue';

  const { formStore } = defineProps<{
    formStore: FormStore;
  }>();
  const { notification } = App.useApp();
  const globalPluginStore = useGlobalPluginStore();

  const onSubmit: FormSubmitType = (fields: CreatePluginRequest) => {
    globalPluginStore
      .create(fields)
      .then(() =>
        notification.success({
          message: 'Success!',
          description: 'Plugin created successfully.',
        }),
      )
      .catch((error) => {
        notification.error({
          message: 'Error!',
          description: (error as Error).message ?? 'An error occurred.',
        });
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
