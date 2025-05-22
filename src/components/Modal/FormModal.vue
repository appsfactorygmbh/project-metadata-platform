<script setup lang="ts">
  import { type PropType, ref } from 'vue';
  import { type FormStore } from '@/components/Form/FormStore';

  const { formStore, title, initiallyOpen, open } = defineProps({
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
    open: {
      type: Boolean,
      default: false,
    },
  });

  const isOpen = ref<boolean>(initiallyOpen);

  watch(
    () => open,
    (value) => {
      isOpen.value = value;
    },
  );

  const emit = defineEmits(['close', 'cancel']);

  // checks for correct input
  const handleOk = () => {
    formStore
      .submit()
      .then(() => {
        isOpen.value = false;
        emit('close');
      })
      .catch((e) => {
        console.error('error submitting input' + e);
      });
  };

  const resetModal = () => {
    emit('cancel');
    formStore.resetFields();
    isOpen.value = false;
    emit('close');
  };
</script>

<template>
  <a-modal
    v-model:open="isOpen"
    width="400px"
    :title="title"
    :ok-button-props="{ disabled: false }"
    @ok="handleOk"
    @cancel="resetModal"
  >
    <slot />
  </a-modal>
</template>
