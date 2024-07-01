<template>
  <a-modal
    v-model:open="localIsOpen"
    :title="title"
    :style="{ top: '20px' }"
    ok-text="Yes"
    cancel-text="No"
    @ok="confirm"
    @cancel="cancel"
  >
    <p>{{ message }}</p>
  </a-modal>
</template>

<script setup>
  import { ref, watch } from 'vue';

  const props = defineProps({
    isOpen: {
      type: Boolean,
      required: true,
    },
    title: {
      type: String,
      required: true,
    },
    message: {
      type: String,
      required: true,
    },
  });

  const emit = defineEmits(['confirm', 'cancel', 'update:isOpen']);

  const localIsOpen = ref(props.isOpen);

  watch(
    () => props.isOpen,
    (newVal) => {
      localIsOpen.value = newVal;
    },
  );

  watch(localIsOpen, (newVal) => {
    emit('update:isOpen', newVal);
  });
  const closeModal = () => {
    localIsOpen.value = false;
  };

  const confirm = () => {
    emit('confirm');
    closeModal();
  };

  const cancel = () => {
    emit('cancel');
    closeModal();
  };
</script>

<style scoped></style>
