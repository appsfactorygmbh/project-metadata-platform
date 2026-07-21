<script lang="ts" setup>
  import InputField from './InputField.vue';
  import type { Rule } from 'ant-design-vue/es/form';
  const props = defineProps({
    attributeName: {
      type: String,
      required: true,
    },
    value: {
      type: String,
      required: true,
    },
    placeholder: {
      type: String,
      default: '',
    },
    rules: {
      type: Array<Rule>,
      default: [],
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
    <InputField
      :value="props.value"
      :placeholder="props.placeholder"
      :default="props.value"
      @update:value="(val: string) => emit('update:value', val)"
    />
  </a-form-item>
</template>

<style lang="css" scoped>
  .formItem {
    margin: 0;
  }
</style>
