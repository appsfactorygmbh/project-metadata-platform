<script lang="ts" setup>
  import type { PropType } from 'vue';

  type Status = '' | 'error' | 'warning' | undefined;

  const { inputValue, inputStatus, requiredValue } = defineProps({
    requiredValue: {
      type: Boolean,
      required: true,
    },
    inputValue: {
      type: [Number, String],
      required: true,
    },
    inputStatus: {
      type: String as PropType<Status>,
      required: true,
    },
  });

  const emit = defineEmits(['updated', 'error', 'success']);
  const textArea = ref(inputValue);

  const handleChange = () => {
    if (!textArea.value && requiredValue) {
      emit('error');
    } else {
      emit('success');
    }
  };
</script>

<template>
  <a-textarea
    v-model:value="textArea"
    class="textArea"
    :status="inputStatus"
    :auto-size="{ minRows: 3, maxRows: 10 }"
    :maxlength="500"
    :show-count="true"
    @input="$emit('updated', textArea)"
    @change="handleChange"
  />
</template>
