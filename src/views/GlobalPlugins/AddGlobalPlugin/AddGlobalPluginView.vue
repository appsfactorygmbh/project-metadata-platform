<script setup lang="ts">
  import { FormModal } from '@/components/Modal';
  import { AddGlobalPluginForm } from './';
  import { useFormStore } from '@/components/Form';
  import type { AddGlobalPluginFormData } from '@/views/GlobalPlugins/AddGlobalPlugin/AddGlobalPluginFormData.ts';

  const formStore = useFormStore('addPluginForm');
  const initialFormValues: AddGlobalPluginFormData = {
    globalPlugin: '',
    inputsDisabled: true,
    pluginName: '',
    pluginUrl: '',
  };
</script>

<template>
  <FormModal title="Add Plugin" :form-store="formStore">
    <AddGlobalPluginForm :form-store="formStore" :initial-values="initialFormValues" />
  </FormModal>
</template>
