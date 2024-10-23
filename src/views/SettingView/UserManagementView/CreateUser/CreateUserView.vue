<script setup lang="ts">
  import { FormModal } from '@/components/Modal';
  import { CreateUserForm } from './';
  import { useFormStore } from '@/components/Form';
  import type { CreateUserFormData } from './CreateUserFormData.ts';

  const formStore = useFormStore('createUserForm');
  const initialFormValues: CreateUserFormData = {
    name: '',
    username: '',
    email: '',
    password: '',
    confirmPassword: '',
    inputsDisabled: false,
  };
</script>

<template>
  <FormModal title="Create User" :form-store="formStore">
    <CreateUserForm
      :form-store="formStore"
      :initial-values="initialFormValues"
    />
  </FormModal>
</template>
