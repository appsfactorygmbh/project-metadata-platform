<script setup lang="ts">
  import { FormModal } from '@/components/Modal';
  import { CreateGlobalPluginForm } from './';
  import { useFormStore } from '@/components/Form';

  const formStore = useFormStore('createPluginForm');
</script>

<template>
  <FormModal title="Create Plugin" :form-store="formStore">
    <a-button
      type="primary"
      @click="() => console.log('formStore', formStore.getModel)"
    >
      Debug
    </a-button>
    <CreateGlobalPluginForm :form-store="formStore" />
  </FormModal>
</template>
