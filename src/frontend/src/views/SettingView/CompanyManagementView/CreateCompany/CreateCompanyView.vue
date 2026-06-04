<script setup lang="ts">
  import { FormModal } from '@/components/Modal';
  import { useFormStore } from '@/components/Form';
  import type { CreateCompanyFormData } from './CreateCompanyFormData.ts';
  import { CreateCompanyForm } from './';
  import router from '@/router/router.ts';
  import {
    companyRoutingSymbol,
    companyStoreSymbol,
  } from '@/store/injectionSymbols.ts';

  const { setCompanyId } = inject(companyRoutingSymbol)!;

  const onCancel = async () => {
    await router.push('/settings/company-management');
  };

  const onSave = async (id: number) => {
    await router.push('/settings/company-management');
    setCompanyId(String(id));
  };

  const formStore = useFormStore('CreateCompanyForm');
  const companyStore = inject(companyStoreSymbol)!;

  if (companyStore === undefined) {
    throw new Error('company store cant be undefined');
  }

  const initialFormValues: CreateCompanyFormData = {
    companyName: '',
    inputsDisabled: false,
  };
</script>

<template>
  <FormModal title="Create Company" :form-store="formStore" @cancel="onCancel">
    <CreateCompanyForm
      :form-store="formStore"
      :initial-values="initialFormValues"
      :company-store="companyStore"
      @new-id="onSave"
    />
  </FormModal>
</template>
