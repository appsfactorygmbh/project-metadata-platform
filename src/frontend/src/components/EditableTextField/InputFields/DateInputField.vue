<script lang="ts" setup>
  import { DatePicker } from 'ant-design-vue';
  import type { Rule } from 'ant-design-vue/es/form';
  import dayjs from 'dayjs';
  import type { DatePickerProps } from 'ant-design-vue';
  import weekday from 'dayjs/plugin/weekday';
  import localeData from 'dayjs/plugin/localeData';
  dayjs.extend(weekday);
  dayjs.extend(localeData);

  const props = defineProps({
    attributeName: {
      type: String,
      required: true,
    },
    value: {
      type: [Date, String, Object] as PropType<
        Date | string | null | undefined
      >,
      required: false,
      default: undefined,
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
    startDate: {
      type: [Date, String, Object] as PropType<
        Date | string | null | undefined
      >,
      required: false,
      default: undefined,
    },
  });

  const emit = defineEmits(['update:value']);
  const disabledDate: DatePickerProps['disabledDate'] = (current) => {
    if (!props.startDate || !current) {
      return false;
    }

    return current.valueOf() < dayjs(props.startDate).startOf('day').valueOf();
  };
</script>

<template>
  <a-form-item
    :name="props.attributeName"
    class="formItem"
    :has-feedback="rules.length > 0"
    :rules="rules"
  >
    <DatePicker
      :value="props.value ? dayjs(props.value) : undefined"
      :placeholder="props.placeholder"
      :allow-clear="false"
      :show-today="false"
      :disabled-date="disabledDate"
      v-bind="$attrs"
      @update:value="
        (val) => {
          if (!val) emit('update:value', undefined);
          else if (typeof val === 'string') emit('update:value', new Date(val));
          else emit('update:value', val.toDate());
        }
      "
    />
  </a-form-item>
</template>

<style lang="css" scoped>
  .formItem {
    margin: 0;
  }
</style>
