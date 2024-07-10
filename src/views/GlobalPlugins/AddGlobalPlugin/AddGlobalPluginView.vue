<script setup lang="ts">
  import { FormModal } from '@/components/Modal';
  import { AddGlobalPluginForm } from './';
  import { useFormStore } from '@/components/Form';

  const formStore = useFormStore('addPluginForm');
</script>

<template>
  <FormModal title="Add Plugin" :form-store="formStore">
    <AddGlobalPluginForm :form-store="formStore" />
  </FormModal>
</template>
