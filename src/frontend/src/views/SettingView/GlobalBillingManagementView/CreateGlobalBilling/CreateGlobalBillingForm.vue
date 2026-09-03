<script setup lang="ts">
  import { type FormStore, type FormSubmitType } from '@/components/Form';
  import { App } from 'ant-design-vue';
  import { reactive, toRaw } from 'vue';
  import type { RulesObject } from '@/components/Form/types';
  import type { CreateGlobalBillingFormData } from './CreateGlobalBillingFormData.ts';
  import { CreateIsUniqueGlobalBillingKind } from '@/utils/form/userValidation.ts';
  import type { CreateGlobalBillingModel } from '@/models/GlobalBilling/CreateGlobalBillingModel.ts';
  import type { GlobalBillingStore } from '@/store/GlobalBillingStore.ts';
  import { TimeFrame } from '@/api/generated/index.ts';

  const { formStore, initialValues, globalBillingStore } = defineProps<{
    formStore: FormStore;
    initialValues: CreateGlobalBillingFormData;
    globalBillingStore: GlobalBillingStore;
  }>();
  const { notification } = App.useApp();
  const emit = defineEmits<(e: 'newId', id: number) => void>();
  const onSubmit: FormSubmitType = async (fields) => {
    try {
      const globalBillingDef: CreateGlobalBillingModel = {
        billingKind: toRaw(fields).billingKind,
        currency: toRaw(fields).currency,
        budgetLimit: toRaw(fields).budgetlimit,
        hostingFee: toRaw(fields).hostingFee,
        targetMargin: toRaw(fields).targetMargin,
        timeFrame: toRaw(fields).timeFrame,
      };
      const id = await globalBillingStore.create(globalBillingDef);
      await globalBillingStore.fetchAll();
      notification.success({
        message: 'Success!',
        description: 'GlobalBilling created successfully.',
      });
      emit('newId', id);
    } catch (error) {
      notification.error({
        message: 'Error!',
        description: (error as Error).message ?? 'An error occurred.',
      });
      console.error('Error creating globalBilling:', error);
      throw error;
    }
  };

  const formItemLayoutWithOutLabel = {
    wrapperCol: {
      xs: { span: 24, offset: 0 },
      sm: { span: 20, offset: 4 },
    },
  };

  const dynamicValidateForm =
    reactive<CreateGlobalBillingFormData>(initialValues);

  const rulesRef = reactive<RulesObject<CreateGlobalBillingFormData>>({
    billingKind: [
      {
        required: true,
        message: 'Please insert an unique globalBilling name.',
        validator: CreateIsUniqueGlobalBillingKind(globalBillingStore),
        trigger: 'change',
        type: 'string',
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
</script>

<template>
  <a-form
    ref="formRef"
    :model="dynamicValidateForm"
    v-bind="formItemLayoutWithOutLabel"
    :wrapper-col="{ span: 24 }"
  >
    <a-form-item
      has-feedback
      name="billingKind"
      class="column"
      :whitespace="true"
      :rules="rulesRef.billingKind"
    >
      <a-input
        id="inputCreateGlobalBillingGlobalBillingKind"
        v-model:value="dynamicValidateForm.billingKind"
        class="inputField"
        placeholder="GlobalBilling Kind"
        :disabled="dynamicValidateForm.inputsDisabled"
        :rules="rulesRef.billingKind"
      />
    </a-form-item>
    <a-form-item
      has-feedback
      name="budgetLimit"
      class="column"
      :whitespace="true"
    >
      <a-input-number
        id="inputCreateGlobalBillingBudgetLimit"
        v-model:value="dynamicValidateForm.budgetLimit"
        class="inputField"
        placeholder="Budget Limit"
        :disabled="dynamicValidateForm.inputsDisabled"
        :controls="false"
      />
    </a-form-item>
    <a-form-item
      has-feedback
      name="hostingFee"
      class="column"
      :whitespace="true"
    >
      <a-input-number
        id="inputCreateGlobalBillingHostingFee"
        v-model:value="dynamicValidateForm.hostingFee"
        class="inputField"
        placeholder="Hosting Fee"
        :disabled="dynamicValidateForm.inputsDisabled"
        :controls="false"
      />
    </a-form-item>
    <a-form-item has-feedback name="currency" class="column" :whitespace="true">
      <a-input
        id="inputCreateGlobalBillingCurrency"
        v-model:value="dynamicValidateForm.currency"
        class="inputField"
        placeholder="Currency"
        :disabled="dynamicValidateForm.inputsDisabled"
      />
    </a-form-item>
    <a-form-item
      has-feedback
      name="targetMargin"
      class="column"
      :whitespace="true"
    >
      <a-input-number
        id="inputCreateGlobalBillingTargetMargin"
        v-model:value="dynamicValidateForm.targetMargin"
        class="inputField"
        placeholder="Target Margin"
        :disabled="dynamicValidateForm.inputsDisabled"
        :controls="false"
        :precision="0"
      />
    </a-form-item>
    <a-form-item
      has-feedback
      name="TimeFrame"
      class="column"
      :whitespace="false"
      :rules="[{ required: false }]"
    >
      <a-select
        id="inputCreateGlobalBillingTimeFrame"
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
        <a-select-option :value="undefined">{{ 'None' }}</a-select-option>
      </a-select>
    </a-form-item>
  </a-form>
</template>

<style scoped>
  .column {
    margin: 0;
  }
</style>
