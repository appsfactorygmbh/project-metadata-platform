<script setup lang="ts">
  import { FormModal } from '@/components/Modal';
  import { useFormStore } from '@/components/Form';
  import type { CreateGlobalBillingFormData } from './CreateGlobalBillingFormData.ts';
  import { CreateGlobalBillingForm } from './';
  import router from '@/router/router.ts';
  import {
    globalBillingRoutingSymbol,
    globalBillingStoreSymbol,
  } from '@/store/injectionSymbols.ts';
  import { ResourceActions } from '@/models/utils/ResourceActions.ts';

  const { setGlobalBillingId } = inject(globalBillingRoutingSymbol)!;

  const onCancel = async () => {
    await router.push('/settings/global-billing-management');
  };

  const onSave = async (id: number) => {
    await router.push('/settings/global-billing-management');
    setGlobalBillingId(String(id));
  };

  const formStore = useFormStore('CreateGlobalBillingForm');
  const globalBillingStore = inject(globalBillingStoreSymbol)!;

  if (globalBillingStore === undefined) {
    throw new Error('global Billing store cant be undefined');
  }

  const initialFormValues: CreateGlobalBillingFormData = {
    billingKind: '',
    currency: undefined,
    budgetLimit: undefined,
    hostingFee: undefined,
    targetMargin: undefined,
    timeFrame: undefined,
    inputsDisabled: false,
  };
</script>

<template>
  <FormModal
    title="Create GlobalBilling"
    :form-store="formStore"
    :disabled="
      !globalBillingStore.getPermissions.includes(ResourceActions.Create)
    "
    @cancel="onCancel"
  >
    <CreateGlobalBillingForm
      :form-store="formStore"
      :initial-values="initialFormValues"
      :global-billing-store="globalBillingStore"
      @new-id="onSave"
    />
  </FormModal>
</template>
