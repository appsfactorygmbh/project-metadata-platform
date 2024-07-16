<script setup lang="ts">
  import { FormModal } from '@/components/Modal';
  import { AddPluginForm } from './index.ts';
  import { useFormStore } from '@/components/Form';
  import type { AddPluginFormData } from './AddPluginFormData.ts';

  const formStore = useFormStore('addPluginForm');
  const initialFormValues: AddPluginFormData = {
    globalPlugin: '',
    inputsDisabled: true,
    pluginName: '',
    pluginUrl: '',
  };
</script>

<template>
  <FormModal title="Add Plugin" :form-store="formStore">
    <AddPluginForm
      :form-store="formStore"
      :initial-values="initialFormValues"
    />
  </FormModal>
</template>
