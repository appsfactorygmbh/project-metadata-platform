<script setup lang="ts">
  import { FormModal } from '@/components/Modal';
  import { CreateGlobalPluginForm } from './';
  import { useFormStore } from '@/components/Form';
  import { useRouter } from 'vue-router';

  const formStore = useFormStore('createPluginForm');

  const router = useRouter();

  const onClose = () => {
    router.push('/settings/global-plugins');
  };
</script>

<template>
  <FormModal title="Create Plugin" :form-store="formStore" @close="onClose">
    <CreateGlobalPluginForm :form-store="formStore" />
  </FormModal>
</template>
