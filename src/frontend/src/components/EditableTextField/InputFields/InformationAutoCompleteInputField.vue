<script lang="ts" setup>
  import { type PropType } from 'vue';

  const props = defineProps({
    value: {
      type: String,
      default: '',
    },
    attributeName: {
      type: String,
      required: true,
    },

    placeholder: {
      type: String,
      required: true,
    },

    options: {
      type: Array as PropType<string[]>,
      required: true,
    },
  });

  const emit = defineEmits(['update:value']);

  const formattedOptions = computed<{ value: string }[]>(() => {
    const baseOptions = props.options.map((opt) => ({ value: String(opt) }));
    const currentValue = props.value ? String(props.value) : '';

    if (
      currentValue &&
      !props.options.some(
        (opt) => String(opt).toLowerCase() === currentValue.toLowerCase(),
      )
    ) {
      return [{ value: currentValue }, ...baseOptions];
    }

    return baseOptions;
  });

  const filterOption = (inputValue: string, option: { value: string }) => {
    return option.value.toUpperCase().includes(inputValue.toUpperCase());
  };
</script>

<template>
  <a-form-item :name="props.attributeName" class="formItem">
    <a-auto-complete
      :value="props.value"
      :options="formattedOptions"
      :filter-option="filterOption"
      :placeholder="props.placeholder"
      style="width: 100%"
      @update:value="(val) => emit('update:value', val)"
    />
  </a-form-item>
</template>

<style lang="css" scoped>
  .formItem {
    margin: 0;
    min-width: 210px;
    max-width: 100%;
  }
</style>
