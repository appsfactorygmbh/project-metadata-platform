<script setup lang="ts">
  import { ref, toRaw, type PropType } from 'vue';
  import { type FormStore } from '@/components/Form/FormStore';

  const { formStore, title, initiallyOpen } = defineProps({
    formStore: {
      type: Object as PropType<FormStore>,
      required: true,
    },
    title: {
      type: String,
      required: true,
    },
    initiallyOpen: {
      type: Boolean,
      default: true,
    },
  });

  const open = ref<boolean>(initiallyOpen);

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
