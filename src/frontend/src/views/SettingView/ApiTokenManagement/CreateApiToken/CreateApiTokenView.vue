<script setup lang="ts">
  import { FormModal } from '@/components/Modal';
  import { useFormStore } from '@/components/Form';
  import router from '@/router/router.ts';
  import {
    apiTokenRoutingSymbol,
    apiTokenStoreSymbol,
  } from '@/store/injectionSymbols.ts';
  import type { CreateApiTokenFormData } from './CreateApiTokenFormData.ts';
  import { CreateApiTokenForm } from './';
  import { ResourceActions } from '@/models/utils/ResourceActions.ts';

  const { setApiTokenId } = inject(apiTokenRoutingSymbol)!;

  const onCancel = async () => {
    await router.push('/settings/api-token-management');
  };

  const onSave = async (id: number) => {
    await router.push('/settings/api-token-management');
    setApiTokenId(String(id));
  };

  const formStore = useFormStore('CreateApiTokenForm');
  const apiTokenStore = inject(apiTokenStoreSymbol)!;

  if (apiTokenStore === undefined) {
    throw new Error('api token store cant be undefined');
  }

  const initialFormValues: CreateApiTokenFormData = {
    name: '',
    scopes: [],
    inputsDisabled: false,
  };
</script>

<template>
  <FormModal
    title="Create API-Token"
    :form-store="formStore"
    :disabled="!apiTokenStore.getPermissions.includes(ResourceActions.Create)"
    @cancel="onCancel"
  >
    <CreateApiTokenForm
      :form-store="formStore"
      :initial-values="initialFormValues"
      :api-token-store="apiTokenStore"
      @new-id="onSave"
    />
  </FormModal>
</template>
