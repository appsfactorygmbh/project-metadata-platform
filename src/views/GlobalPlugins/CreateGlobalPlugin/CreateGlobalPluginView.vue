<script setup lang="ts">
  import { FormModal } from '@/components/Modal';
  import { CreateGlobalPluginForm } from './';
  import { useFormStore } from '@/components/Form';

  const formStore = useFormStore('createPluginForm');
</script>

<template>
  <contextHolder></contextHolder>
  <FormModal title="Create Plugin" :form-store="formStore">
    <a-button
      type="primary"
      @click="() => console.log('formStore', formStore.getRef)"
    >
      Debug
    </a-button>
    <CreateGlobalPluginForm :form-store="formStore" />
  </FormModal>
</template>
