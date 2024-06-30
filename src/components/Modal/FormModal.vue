<script setup lang="ts">
  import { ref } from 'vue';
  import type {
    FormSubmitType,
    FormType,
  } from '@/components/Modal/FormTypes.ts';

  const { form, title, onSubmit } = defineProps<{
    form: FormType;
    title: string;
    onSubmit: FormSubmitType;
  }>();

  const open = ref<boolean>(false);
  const formRef = ref();
  const cancelFetch = ref<boolean>();

  const fetchError = ref<boolean>(false);

  // checks for correct input
  const handleOk = () => {
    cancelFetch.value = false;
    form
      .validate()
      .then(() => {
        onSubmit(form.modelRef.value);
      })
      .catch((error: unknown) => {
        console.log('error', error);
      });
  };

  const resetModal = () => {
    formRef.value.resetFields();
    fetchError.value = false;
  };
</script>

<template>
  <a-modal
    v-model:open="open"
    width="400px"
    :title="title"
    :ok-button-props="{ disabled: isAdding }"
    @ok="handleOk"
    @cancel="resetModal"
  >
    <slot></slot>
  </a-modal>
</template>

<style scoped></style>
