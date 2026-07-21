<script setup lang="ts">
  import { FormModal } from '@/components/Modal';
  import { CreateUserForm } from './';
  import { useFormStore } from '@/components/Form';
  import type { CreateUserFormData } from './CreateUserFormData.ts';
  import { userStoreSymbol } from '@/store/injectionSymbols.ts';
  import { inject } from 'vue';
  import router from '@/router/router.ts';
  import { ResourceActions } from '@/models/utils/ResourceActions.ts';

  const onClose = () => {
    router.push('/settings/user-management');
  };

  const formStore = useFormStore('createUserForm');
  const userStore = inject(userStoreSymbol)!;
  const initialFormValues: CreateUserFormData = {
    employeeNumber: '',
    email: '',
    password: '',
    confirmPassword: '',
    active: false,
    jobTitles: [],
    teams: [],
    teamSupport: [],
    departments: [],
    businessUnits: [],
    company: undefined,
    officeLocation: undefined,
    inputsDisabled: false,
  };
</script>

<template>
  <FormModal
    title="Create User"
    :form-store="formStore"
    :disabled="!userStore.getPermissions.includes(ResourceActions.Create)"
    @close="onClose"
  >
    <CreateUserForm
      :form-store="formStore"
      :initial-values="initialFormValues"
      :user-store="userStore"
    />
  </FormModal>
</template>
