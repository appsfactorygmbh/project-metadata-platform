<script lang="ts" setup>
  import { type PropType } from 'vue';

  const props = defineProps({
    value: {
      type: Array as PropType<string[]>,
      default: () => [],
    },
    attributeName: {
      type: String,
      required: true,
    },
    options: {
      type: Array as PropType<string[]>,
      required: true,
    },
    placeholder: {
      type: String,
      default: '',
    },
    mode: {
      type: String as PropType<'multiple' | 'tags'>,
      default: 'multiple',
    },
  });

  const emit = defineEmits(['update:value']);
</script>

<template>
  <a-form-item :name="props.attributeName" class="formItem">
    <a-select
      :value="props.value"
      :mode="props.mode"
      :placeholder="props.placeholder"
      :not-found-content="
        props.options.length === 0 && props.mode == 'tags' ? null : undefined
      "
      @update:value="(val) => emit('update:value', val)"
    >
      <a-select-option v-for="option in options" :key="option" :value="option">
        {{ option }}
      </a-select-option>
    </a-select>
  </a-form-item>
</template>

<style lang="css" scoped>
  .formItem {
    margin: 0;
    min-width: 210px;
    max-width: 100%;
  }
</style>
