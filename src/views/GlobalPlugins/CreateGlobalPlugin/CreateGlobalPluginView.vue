<script setup lang="ts">
  import {
    FormModal,
    type FormSubmitType,
    type FormType,
  } from '@/components/Modal';
  import { CreateGlobalPluginForm } from './';
  import { Form, notification } from 'ant-design-vue';
  import { pluginStoreSymbol } from '@/store/injectionSymbols';
  import { inject } from 'vue';

  const modelRef = ref();
  const form: FormType = Form.useForm(modelRef);

  const pluginStore = inject(pluginStoreSymbol);

  const [notificationApi, contextHolder] = notification.useNotification();

  const onSubmit: FormSubmitType = (fields) => {
    try {
      pluginStore?.createPlugin(fields);
    } catch {
      notificationApi.error({
        message: 'An error occurred. The plugin could not be created',
      });
      console.log('fehler');
    }
  };
</script>

<template>
  <contextHolder></contextHolder>
  <FormModal title="Create Plugin" :form="form" :on-submit="onSubmit">
    <CreateGlobalPluginForm :form="form" />
  </FormModal>
</template>
s
