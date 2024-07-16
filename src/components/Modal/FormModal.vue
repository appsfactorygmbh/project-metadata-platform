<script setup lang="ts">
  import { ref, toRaw } from 'vue';
  import { type FormStore } from '@/components/Form/FormStore';

  const { formStore, title, initiallyOpen } = defineProps<{
    formStore: FormStore;
    title: string;
    initiallyOpen?: boolean;
  }>();

  const open = ref<boolean>(initiallyOpen ?? true);

  const emit = defineEmits(['close']);

  // checks for correct input
  const handleOk = () => {
    formStore
      .submit()
      .then(() => {
        open.value = false;
        emit('close');
      })
      .catch((e) => {
        console.log(e);
        console.log(toRaw(formStore.validateInfos));
      });
  };

  const resetModal = () => {
    emit('close');
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
