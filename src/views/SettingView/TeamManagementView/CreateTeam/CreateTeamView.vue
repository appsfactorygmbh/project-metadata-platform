<script setup lang="ts">
  import { FormModal } from '@/components/Modal';
  import { useFormStore } from '@/components/Form';
  import type { CreateTeamFormData } from './CreateTeamFormData.ts';
  import { teamStoreSymbol } from '@/store/injectionSymbols.ts';
  import { inject } from 'vue';
  import router from '@/router/router.ts';

  const onClose = () => {
    router.push('/settings/user-management');
  };

  const formStore = useFormStore('CreateTeamForm');
  const teamStore = inject(teamStoreSymbol)!;
  const initialFormValues: CreateTeamFormData = {
    teamName: '',
    ptl: '',
    businessUnit: '',
    inputsDisabled: false,
  };
</script>

<template>
  <FormModal title="Create Team" :form-store="formStore" @close="onClose">
    <CreateUserForm
      :form-store="formStore"
      :initial-values="initialFormValues"
      :team-store="teamStore"
    />
  </FormModal>
</template>
