<script setup lang="ts">
  import { FormModal } from '@/components/Modal';
  import { useFormStore } from '@/components/Form';
  import type { CreateDepartmentFormData } from './CreateDepartmentFormData.ts';
  import { CreateDepartmentForm } from './';
  import router from '@/router/router.ts';
  import {
    departmentRoutingSymbol,
    departmentStoreSymbol,
  } from '@/store/injectionSymbols.ts';
  import { ResourceActions } from '@/models/utils/ResourceActions.ts';

  const { setDepartmentId } = inject(departmentRoutingSymbol)!;

  const onCancel = async () => {
    await router.push('/settings/department-management');
  };

  const onSave = async (id: number) => {
    await router.push('/settings/department-management');
    setDepartmentId(String(id));
  };

  const formStore = useFormStore('CreateDepartmentForm');
  const departmentStore = inject(departmentStoreSymbol)!;

  if (departmentStore === undefined) {
    throw new Error('department store cant be undefined');
  }

  const initialFormValues: CreateDepartmentFormData = {
    departmentName: '',
    inputsDisabled: false,
  };
</script>

<template>
  <FormModal
    title="Create Department"
    :form-store="formStore"
    :disabled="!departmentStore.getPermissions.includes(ResourceActions.Create)"
    @cancel="onCancel"
  >
    <CreateDepartmentForm
      :form-store="formStore"
      :initial-values="initialFormValues"
      :department-store="departmentStore"
      @new-id="onSave"
    />
  </FormModal>
</template>
