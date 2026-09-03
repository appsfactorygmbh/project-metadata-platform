<script lang="ts" setup>
  import type { Rule } from 'ant-design-vue/es/form';
  const props = defineProps({
    attributeName: {
      type: String,
      required: true,
    },
    value: {
      type: [String, Number] as PropType<string | number | null | undefined>,
      required: true,
    },
    placeholder: {
      type: String,
      default: '',
    },
    rules: {
      type: Array as PropType<Rule[]>,
      default: () => [],
      required: false,
    },
    precision: {
      type: Number,
      default: undefined,
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
    <NumericInputField
      :value="props.value"
      :placeholder="props.placeholder"
      :default="props.value?.toString() ?? ''"
      :precision="props.precision"
      @update:value="(val: string | number | null) => emit('update:value', val)"
    />
  </a-form-item>
</template>

<style lang="css" scoped>
  .formItem {
    margin: 0;
  }
</style>
