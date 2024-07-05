<script setup lang="ts">
  import { ref } from 'vue';
  import { type FormStore } from '@/components/Form/FormStore';

  const { formStore, title } = defineProps<{
    formStore: FormStore;
    title: string;
  }>();

  const open = ref<boolean>(true); //TODO: set default to false after implementing button
  const cancelFetch = ref<boolean>();

  const fetchError = ref<boolean>(false);

  // checks for correct input
  const handleOk = () => {
    cancelFetch.value = false;
    formStore
      .validate()
      .then(() => {
        console.log('formStore.getFieldsValue', formStore.getFieldsValue);
        formStore.submit();
      })
      .catch((error: unknown) => {
        console.log('error', error);
      });
  };

  const resetModal = () => {
    formStore.resetFields();
    fetchError.value = false;
  };
</script>

<template>
  <a-modal
    v-model:open="open"
    width="400px"
    :title="title"
    :ok-button-props="{ disabled: false }"
    @ok="handleOk"
    @cancel="resetModal"
  >
    <slot></slot>
  </a-modal>
</template>

<style scoped></style>
