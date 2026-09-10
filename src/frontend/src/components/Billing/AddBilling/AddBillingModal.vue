<script setup lang="ts">
  import { FormModal } from '@/components/Modal';

  import { useFormStore } from '@/components/Form';
  import type { AddBillingFormData } from './AddBillingFormData.ts';
  import { Currencies, TimeFrame } from '@/api/generated/index.ts';

  const props = defineProps({
    pluginName: { type: String, required: true },
    projectId: { type: Number, required: true },
    pluginId: { type: Number, required: true },
  });

  const formStore = useFormStore('addBillingForm');

  const initialFormValues: AddBillingFormData = {
    billingId: undefined,
    contractIds: [],
    currency: undefined as Currencies | undefined,
    budgetLimit: undefined,
    hostingFee: undefined,
    targetMargin: undefined,
    timeFrame: undefined as TimeFrame | undefined,
    date: undefined as Date | undefined,
    notes: undefined as string | undefined,
    inputsDisabled: true,
  };

  const emit = defineEmits(['added-billing', 'cancel']);
</script>

<template>
  <FormModal
    :title="'Add Billing to Plugin: ' + props.pluginName"
    :form-store="formStore"
    @cancel="() => emit('cancel')"
  >
    <AddBillingForm
      :plugin-id="props.pluginId"
      :project-id="props.projectId"
      :form-store="formStore"
      :initial-values="initialFormValues"
      @added-billing="() => emit('added-billing')"
    />
  </FormModal>
</template>
