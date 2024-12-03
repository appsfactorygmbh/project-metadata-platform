<script setup lang="ts">
  import { type FormSubmitType } from '@/components/Form';
  import { notification } from 'ant-design-vue';
  import { globalPluginStoreSymbol } from '@/store/injectionSymbols';
  import { inject, reactive } from 'vue';
  //import type { CreatePluginModel } from '@/models/Plugin';
  import { type FormStore } from '@/components/Form';
  import GlobalPluginForm from '../GlobalPluginForm/GlobalPluginForm.vue';
  import type { GlobalPluginFormData } from '../GlobalPluginForm';

  const { formStore } = defineProps<{
    formStore: FormStore;
  }>();

  const globalPluginStore = inject(globalPluginStoreSymbol);

  const [notificationApi, contextHolder] = notification.useNotification();

  const onSubmit: FormSubmitType = (fields) => {
    try {
      globalPluginStore?.createGlobalPlugin(fields);
    } catch {
      notificationApi.error({
        message: 'An error occurred. The plugin could not be created',
      });
      console.error('error');
    }
  };

  formStore.setOnSubmit(onSubmit);

  const initialValues = reactive<GlobalPluginFormData>({
    pluginName: '',
    keys: [],
  });
</script>

<template>
  <GlobalPluginForm :form-store="formStore" :initial-values="initialValues" />
  <contextHolder></contextHolder>
</template>
