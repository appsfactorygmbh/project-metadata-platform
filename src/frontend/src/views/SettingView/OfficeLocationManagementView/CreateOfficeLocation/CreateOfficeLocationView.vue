<script setup lang="ts">
  import { FormModal } from '@/components/Modal';
  import { useFormStore } from '@/components/Form';
  import type { CreateOfficeLocationFormData } from './CreateOfficeLocationFormData.ts';
  import { CreateOfficeLocationForm } from './';
  import router from '@/router/router.ts';
  import {
    officeLocationRoutingSymbol,
    officeLocationStoreSymbol,
  } from '@/store/injectionSymbols.ts';

  const { setOfficeLocationId } = inject(officeLocationRoutingSymbol)!;

  const onCancel = async () => {
    await router.push('/settings/office-location-management');
  };

  const onSave = async (id: number) => {
    await router.push('/settings/office-location-management');
    setOfficeLocationId(String(id));
  };

  const formStore = useFormStore('CreateOfficeLocationForm');
  const officeLocationStore = inject(officeLocationStoreSymbol)!;

  if (officeLocationStore === undefined) {
    throw new Error('office location store cant be undefined');
  }

  const initialFormValues: CreateOfficeLocationFormData = {
    officeLocationName: '',
    inputsDisabled: false,
  };
</script>

<template>
  <FormModal
    title="Create Office Location"
    :form-store="formStore"
    @cancel="onCancel"
  >
    <CreateOfficeLocationForm
      :form-store="formStore"
      :initial-values="initialFormValues"
      :office-location-store="officeLocationStore"
      @new-id="onSave"
    />
  </FormModal>
</template>
