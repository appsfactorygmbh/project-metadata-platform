<script lang="ts" setup>
  import { type PropType, computed } from 'vue';
  import type { Rule } from 'ant-design-vue/es/form';

  export interface SelectOption {
    id: number;
    name: string;
  }
  const props = defineProps({
    attributeName: {
      type: String,
      required: true,
    },
    placeholder: {
      type: String,
      default: '',
    },
    value: {
      type: [String, Number] as PropType<string | number | undefined>,
      required: true,
    },
    rules: {
      type: Array as PropType<Rule[]>,
      default: () => [],
    },
    options: {
      type: Array as PropType<SelectOption[]>,
      required: true,
    },
  });
  const emit = defineEmits(['update:value']);

  const currentAttributeRulesForTemplate = computed(() => props.rules);
</script>

<template>
  <a-form-item
    :name="props.attributeName"
    :rules="currentAttributeRulesForTemplate"
    class="formItem"
    :has-feedback="!!(props.rules && props.rules.length > 0)"
  >
    <a-select
      :value="props.value"
      show-search
      :placeholder="props.placeholder"
      :options="props.options"
      :field-names="{ label: 'name', value: 'id' }"
      option-filter-prop="name"
      :dropdown-match-select-width="false"
      style="width: 100%; min-width: 200px"
      @update:value="(val) => emit('update:value', val)"
    />
  </a-form-item>
</template>

<style lang="css" scoped>
  .formItem {
    margin: 0;
  }
</style>
