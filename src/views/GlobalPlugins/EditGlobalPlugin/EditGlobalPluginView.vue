<script setup lang="ts">
  import { FormModal } from '@/components/Modal';
  import { EditGlobalPluginForm } from './';
  import { useFormStore } from '@/components/Form';

  const formStore = useFormStore('editPluginForm');

  const onClose = () => {
    // TODO: implement close function
  };
</script>

<template>
  <FormModal title="Edit Plugin" :form-store="formStore" @close="onClose">
    <EditGlobalPluginForm :form-store="formStore" />
  </FormModal>
</template>
