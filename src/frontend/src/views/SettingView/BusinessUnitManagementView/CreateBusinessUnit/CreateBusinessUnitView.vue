<script setup lang="ts">
  import { FormModal } from '@/components/Modal';
  import { useFormStore } from '@/components/Form';
  import type { CreateBusinessUnitFormData } from './CreateBusinessUnitFormData.ts';
  import { CreateBusinessUnitForm } from './';
  import router from '@/router/router.ts';
  import {
    businessUnitRoutingSymbol,
    businessUnitStoreSymbol,
  } from '@/store/injectionSymbols.ts';
  import { ResourceActions } from '@/models/utils/ResourceActions.ts';

  const { setBusinessUnitId } = inject(businessUnitRoutingSymbol)!;

  const onCancel = async () => {
    await router.push('/settings/business-unit-management');
  };

  const onSave = async (id: number) => {
    await router.push('/settings/business-unit-management');
    setBusinessUnitId(String(id));
  };

  const formStore = useFormStore('CreateBusinessUnitForm');
  const businessUnitStore = inject(businessUnitStoreSymbol)!;

  if (businessUnitStore === undefined) {
    throw new Error('business Unit store cant be undefined');
  }

  const initialFormValues: CreateBusinessUnitFormData = {
    businessUnitName: '',
    inputsDisabled: false,
  };
</script>

<template>
  <FormModal
    title="Create Business Unit"
    :form-store="formStore"
    :disabled="
      !businessUnitStore.getPermissions.includes(ResourceActions.Create)
    "
    @cancel="onCancel"
  >
    <CreateBusinessUnitForm
      :form-store="formStore"
      :initial-values="initialFormValues"
      :business-unit-store="businessUnitStore"
      @new-id="onSave"
    />
  </FormModal>
</template>
