<script setup lang="ts">
  import { ref } from 'vue';
  import { type FormStore } from '@/components/Form/FormStore';

  const { formStore, title } = defineProps<{
    formStore: FormStore;
    title: string;
  }>();

  const open = ref<boolean>(true); //TODO: set default to false after implementing button

  // checks for correct input
  const handleOk = () => {
    formStore.submit().catch((e) => {
      console.log(e);
    });
  };

  const resetModal = () => {
    formStore.resetFields();
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
