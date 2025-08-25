<script setup lang="ts">
  import { FormModal } from '@/components/Modal';
  import { EditGlobalPluginForm } from './';
  import { useFormStore } from '@/components/Form';
  import { useRouter } from 'vue-router';

  const formStore = useFormStore('editPluginForm');

  const router = useRouter();

  const onClose = () => {
    router.push('/settings/global-plugins');
  };
</script>

<template>
  <FormModal title="Edit Plugin" :form-store="formStore" @close="onClose">
    <EditGlobalPluginForm :form-store="formStore" />
  </FormModal>
</template>
