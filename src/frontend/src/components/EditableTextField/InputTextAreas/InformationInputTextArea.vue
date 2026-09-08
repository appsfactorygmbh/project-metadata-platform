<script lang="ts" setup>
  import type { Rule } from 'ant-design-vue/es/form';
  const props = defineProps({
    attributeName: {
      type: String,
      required: true,
    },
    value: {
      type: [String] as PropType<string | null | undefined>,
      required: true,
    },
    placeholder: {
      type: String,
      default: '',
    },
    maxLength: {
      type: Number,
      required: false,
      default: 280,
    },
    maxRows: {
      type: Number,
      required: false,
      default: 3,
    },
    rules: {
      type: Array as PropType<Rule[]>,
      default: () => [],
      required: false,
    },
  });

  const emit = defineEmits(['update:value']);
</script>

<template>
  <a-form-item
    :name="props.attributeName"
    class="formItem"
    :has-feedback="rules.length > 0"
    :rules="rules"
  >
    <TextArea
      :value="props.value"
      :placeholder="props.placeholder"
      :default="props.value ?? ''"
      :auto-size="{ minRows: 3, maxRows: props.maxRows }"
      :show-count="true"
      :maxlength="props.maxLength"
      @update:value="(val: string | null) => emit('update:value', val)"
    />
  </a-form-item>
</template>

<style lang="css" scoped>
  .formItem {
    margin: 0;
  }
</style>
