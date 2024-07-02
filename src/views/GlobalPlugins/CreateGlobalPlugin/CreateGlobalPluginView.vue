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
  const formRef = ref(form);

  const pluginStore = inject(pluginStoreSymbol);

  const [notificationApi, contextHolder] = notification.useNotification();

  const onSubmit: FormSubmitType = (fields) => {
    try {
      console.log(fields);
      pluginStore?.createPlugin(form.modelRef.value);
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
  <FormModal title="Create Plugin" :form-ref="formRef" :on-submit="onSubmit">
    <CreateGlobalPluginForm :form-ref="formRef" />
  </FormModal>
</template>
