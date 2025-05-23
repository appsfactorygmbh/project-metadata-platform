<script setup lang="ts">
  import { FormModal } from '@/components/Modal';
  import { useFormStore } from '@/components/Form';
  import type { CreateTeamFormData } from './CreateTeamFormData.ts';
  import { CreateTeamForm } from './';
  import router from '@/router/router.ts';
  import { useTeamStore } from '@/store/TeamStore.ts';
  import { teamRoutingSymbol } from '@/store/injectionSymbols.ts';

  const { setTeamId } = inject(teamRoutingSymbol)!;

  const onCancel = async () => {
    await router.push('/settings/team-management');
  };

  const onSave = async (id: number) => {
    await router.push('/settings/team-management');
    setTeamId(String(id));
  };

  const formStore = useFormStore('CreateTeamForm');
  const teamStore = useTeamStore();
  const initialFormValues: CreateTeamFormData = {
    teamName: '',
    ptl: '',
    businessUnit: '',
    inputsDisabled: false,
  };
</script>

<template>
  <FormModal title="Create Team" :form-store="formStore" @cancel="onCancel">
    <CreateTeamForm
      :form-store="formStore"
      :initial-values="initialFormValues"
      :team-store="teamStore"
      @new-id="onSave"
    />
  </FormModal>
</template>
