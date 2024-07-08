<script setup lang="ts">
  import { FormModal } from '@/components/Modal';
  import { EditGlobalPluginForm } from './';
  import { useFormStore } from '@/components/Form';

  const formStore = useFormStore('editPluginForm');
</script>

<template>
  <FormModal title="Edit Plugin" :form-store="formStore">
    <EditGlobalPluginForm :form-store="formStore" />
  </FormModal>
</template>
