<script setup lang="ts">
  import { type FormStore, type FormSubmitType } from '@/components/Form';
  import { App } from 'ant-design-vue';
  import { onBeforeMount, reactive, ref, toRaw } from 'vue';
  import type {
    DatePickerProps,
    FormInstance,
    SelectProps,
  } from 'ant-design-vue';
  import type { CreateBillingModel } from '@/models/Billing';
  import type { SelectValue } from 'ant-design-vue/lib/select';
  import { DatePicker } from 'ant-design-vue';
  import dayjs from 'dayjs';
  import localeData from 'dayjs/plugin/localeData';
  import weekday from 'dayjs/plugin/weekday';
  dayjs.extend(weekday);
  dayjs.extend(localeData);
  import type { RulesObject } from '@/components/Form/types';
  import type { AddBillingFormData } from './AddBillingFormData.ts';
  import { useGlobalBillingStore } from '@/store/GlobalBillingStore.ts';
  import { useBillingStore } from '@/store';
  import type { GlobalBillingModel } from '@/models/GlobalBilling';
  import { TimeFrame } from '@/api/generated/models/TimeFrame.ts';
  import { Currencies } from '@/api/generated/index.ts';
  import type { Rule } from 'ant-design-vue/es/form/interface';

  const { formStore, initialValues, projectId, pluginId } = defineProps<{
    projectId: number;
    pluginId: number;
    formStore: FormStore;
    initialValues: AddBillingFormData;
  }>();
  const { notification } = App.useApp();
  const globalBillingStore = useGlobalBillingStore();
  const billingStore = useBillingStore();
  const options = ref<SelectProps['options']>([]);
  const emit = defineEmits(['addedBilling']);
  const formRef = ref<FormInstance>();

  onBeforeMount(async () => {
    await globalBillingStore?.fetchAll();
    options.value = toRaw(globalBillingStore?.getGlobalBillingList).map(
      (billing: GlobalBillingModel) => {
        return {
          value: billing.id,
          label: billing.billingKind,
        };
      },
    );
  });

  const getCurrencyName = (currencyCode: string) => {
    try {
      return (
        new Intl.DisplayNames([], { type: 'currency' }).of(currencyCode) ||
        currencyCode
      );
    } catch {
      return currencyCode;
    }
  };

  const onSubmit: FormSubmitType = async (fields) => {
    try {
      const billingDef: CreateBillingModel = {
        billingId: toRaw(fields).billingId,
        contractIds: toRaw(fields).contractIds,
        currency: toRaw(fields).currency,
        budgetLimit: toRaw(fields).budgetLimit,
        hostingFee: toRaw(fields).hostingFee,
        targetMargin: toRaw(fields).targetMargin,
        timeFrame: toRaw(fields).timeFrame,
        date: toRaw(fields).date,
        notes: toRaw(fields).notes,
      };
      await billingStore.add(projectId, pluginId, billingDef);
      emit('addedBilling');
      notification.success({
        message: 'Success!',
        description: 'Billing Information added successfully.',
      });
    } catch (error) {
      notification.error({
        message: 'Error!',
        description: (error as Error).message ?? 'An error occurred.',
      });
      console.error('error while adding a new plugin billing', error);
    }
  };
  const handleChange = async (value: SelectValue) => {
    if (typeof value !== 'number') {
      dynamicValidateForm.inputsDisabled = true;
      return;
    }

    dynamicValidateForm.inputsDisabled = false;
    await globalBillingStore.fetch(value);
    const billing = globalBillingStore.getGlobalBilling;
    if (billing == undefined) {
      dynamicValidateForm.inputsDisabled = true;
      return;
    }
    dynamicValidateForm.currency = billing.currency ?? undefined;
    dynamicValidateForm.targetMargin = billing.targetMargin ?? undefined;
    dynamicValidateForm.timeFrame = billing.timeFrame ?? undefined;
  };
  const dynamicValidateForm = reactive<AddBillingFormData>(initialValues);
  const isDateNeeded = async (_rule: Rule, value: Date | undefined) => {
    if (dynamicValidateForm.timeFrame == TimeFrame.Date && !value) {
      return Promise.reject('Please add a valid date.');
    }
    return Promise.resolve();
  };

  const disabledDate: DatePickerProps['disabledDate'] = (current) => {
    if (!current) {
      return false;
    }

    return current.valueOf() < dayjs().startOf('day').valueOf();
  };

  const rulesRef = reactive<RulesObject<AddBillingFormData>>({
    billingId: [
      {
        required: true,
        message: 'Please select a global billing.',
        trigger: ['blur', 'change'],
        type: 'number',
      },
    ],

    budgetLimit: [
      {
        required: true,
        trigger: ['blur', 'change'],
        message: 'This field is required.',
      },
    ],
    hostingFee: [
      {
        required: true,
        trigger: ['blur', 'change'],
        message: 'This field is required.',
      },
    ],
    currency: [
      {
        required: true,
        trigger: ['blur', 'change'],
        message: 'This field is required.',
      },
    ],
    targetMargin: [
      {
        required: true,
        trigger: ['blur', 'change'],
        message: 'This field is required.',
      },
    ],
    timeFrame: [
      {
        required: true,
        trigger: ['blur', 'change'],
        message: 'This field is required.',
      },
    ],
    date: [
      {
        message: 'Please add a valid date.',
        validator: isDateNeeded,
        trigger: ['change', 'blur'],
      },
    ],
    inputsDisabled: [
      {
        required: false,
      },
    ],
  });

  formStore.setOnSubmit(onSubmit);
  formStore.setModel(dynamicValidateForm);
  formStore.setRules(rulesRef);

  defineExpose({
    formRef,
    validate: () => formRef.value?.validate(),
  });
</script>

<template>
  <a-form
    ref="formRef"
    :model="dynamicValidateForm"
    :rules="rulesRef"
    layout="vertical"
  >
    <a-form-item name="billingId" class="column">
      <a-select
        id="inputAddBillingBillingSelect"
        v-model:value="dynamicValidateForm.billingId"
        class="inputField"
        show-search
        option-filter-prop="label"
        placeholder="Select a global Billing Template"
        :options="options"
        @change="handleChange"
      />
    </a-form-item>
    <a-tooltip title="Press [Enter] to add a contract Id to the list.">
      <a-form-item
        has-feedback
        name="contractIds"
        class="selectcolumntop"
        :whitespace="false"
      >
        <a-select
          id="inputAddBillingContractIds"
          v-model:value="dynamicValidateForm.contractIds"
          mode="tags"
          placeholder="Contract Ids"
          :not-found-content="null"
          :open="false"
          :disabled="false"
        >
        </a-select>
      </a-form-item>
    </a-tooltip>
    <a-form-item
      has-feedback
      name="budgetLimit"
      class="column"
      :whitespace="true"
    >
      <a-input-number
        id="inputCreateBillingBudgetLimit"
        v-model:value="dynamicValidateForm.budgetLimit"
        style="width: 100%"
        placeholder="Budget Limit"
        :disabled="false"
        :controls="false"
        :max="999999999"
        :min="0"
        :parser="
          (val: string | undefined) => {
            if (!val) return '';
            return val.replace(/,/g, '.');
          }
        "
      />
    </a-form-item>
    <a-form-item
      has-feedback
      name="hostingFee"
      class="column"
      :whitespace="true"
    >
      <a-input-number
        id="inputCreateBillingHostingFee"
        v-model:value="dynamicValidateForm.hostingFee"
        style="width: 100%"
        placeholder="Hosting Fee"
        :disabled="false"
        :controls="false"
        :max="999999999"
        :min="0"
        :parser="
          (val: string | undefined) => {
            if (!val) return '';
            return val.replace(/,/g, '.');
          }
        "
      />
    </a-form-item>
    <a-form-item has-feedback name="currency" class="column" :whitespace="true">
      <a-select
        id="inputCreateGlobalBillingCurrency"
        v-model:value="dynamicValidateForm.currency"
        show-search
        class="inputField"
        placeholder="Currency"
        :disabled="dynamicValidateForm.inputsDisabled"
        option-filter-prop="label"
      >
        <a-select-option
          v-for="[key, value] in Object.entries(Currencies)"
          :key="value"
          :value="value"
          :label="getCurrencyName(key)"
        >
          {{ getCurrencyName(key) }}
        </a-select-option>
      </a-select>
    </a-form-item>
    <a-form-item
      :has-feedback="!dynamicValidateForm.inputsDisabled"
      name="targetMargin"
      class="column"
    >
      <a-input-number
        id="inputCreateBillingTargetMargin"
        v-model:value="dynamicValidateForm.targetMargin"
        style="width: 100%"
        placeholder="Target Margin"
        :disabled="dynamicValidateForm.inputsDisabled"
        :controls="false"
        :precision="0"
        :max="100"
        :min="0"
      />
    </a-form-item>
    <a-form-item
      has-feedback
      name="timeFrame"
      class="column"
      :whitespace="false"
    >
      <a-select
        id="inputCreateBillingTimeFrame"
        v-model:value="dynamicValidateForm.timeFrame"
        show-search
        class="inputField"
        placeholder="Time Frame"
        :disabled="dynamicValidateForm.inputsDisabled"
      >
        <a-select-option
          v-for="[key, value] in Object.entries(TimeFrame)"
          :key="value"
          :value="value"
        >
          {{ key }}
        </a-select-option>
      </a-select>
    </a-form-item>
    <a-form-item
      v-if="dynamicValidateForm.timeFrame == TimeFrame.Date"
      has-feedback
      name="date"
      class="column"
      :whitespace="false"
    >
      <DatePicker
        id="inputCreateBillingDate"
        class="inputField"
        style="width: 100%"
        :disabled="dynamicValidateForm.inputsDisabled"
        :allow-clear="false"
        :show-today="false"
        :disabled-date="disabledDate"
        :value="
          dynamicValidateForm.date ? dayjs(dynamicValidateForm.date) : undefined
        "
        :placeholder="dynamicValidateForm.date ? undefined : 'Select Date'"
        @update:value="
          (val) => {
            if (!val) dynamicValidateForm.date = undefined;
            else if (typeof val === 'string')
              dynamicValidateForm.date = new Date(val);
            else dynamicValidateForm.date = val.toDate();
          }
        "
      />
    </a-form-item>
    <a-form-item name="notes" feedback>
      <a-textarea
        v-model:value="dynamicValidateForm.notes"
        placeholder="Notes"
        :auto-size="{ minRows: 3, maxRows: 5 }"
        :maxlength="280"
        :show-count="true"
      />
    </a-form-item>
  </a-form>
</template>

<style>
  .column {
    margin-bottom: 3px;
  }
</style>
