<script lang="ts" setup>
  import type { PropType } from 'vue';

  type Status = '' | 'error' | 'warning' | undefined;

  const { inputValue, inputStatus, requiredValue } = defineProps({
    inputValue: {
      type: [Number, String],
      required: true,
    },
    inputStatus: {
      type: String as PropType<Status>,
      required: true,
    },
    requiredValue: {
      type: Boolean,
      required: true,
    },
  });

  const emit = defineEmits(['updated', 'error', 'success']);
  const inputField = ref(inputValue);

  const handleChange = () => {
    if (!inputField.value && requiredValue) {
      emit('error');
    } else {
      emit('success');
    }
  };
</script>

<template>
  <a-input
    v-model:value="inputField"
    class="inputField"
    :status="inputStatus"
    @input="$emit('updated', inputField)"
    @change="handleChange"
  />
</template>
