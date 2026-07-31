<script setup lang="ts">
  import { type FormStore, type FormSubmitType } from '@/components/Form';
  import { App } from 'ant-design-vue';
  import { reactive, toRaw } from 'vue';
  import type { RulesObject } from '@/components/Form/types';
  import type { CreateApiTokenFormData } from './CreateApiTokenFormData.ts';
  import type { ApiTokenStore } from '@/store/ApiTokenStore.ts';
  import type { CreateApiTokenModel } from '@/models/ApiToken/CreateApiTokenModel.ts';
  import {
    CreateisUniqueTokenName,
    CreateOnlyOneScimToken,
  } from '@/utils/form/userValidation.ts';
  import { TokenScopes } from '@/api/generated/index.ts';
  const { notification } = App.useApp();
  const { formStore, initialValues, apiTokenStore } = defineProps<{
    formStore: FormStore;
    initialValues: CreateApiTokenFormData;
    apiTokenStore: ApiTokenStore;
  }>();
  const emit = defineEmits<(e: 'newId', id: number) => void>();

  const onSubmit: FormSubmitType = async (fields) => {
    try {
      const tokenDef: CreateApiTokenModel = {
        name: toRaw(fields).name,
        scopes: toRaw(fields).scopes,
      };
      const id = await apiTokenStore.create(tokenDef);
      await apiTokenStore.fetchAll();
      emit('newId', id);
    } catch (error) {
      notification.error({
        message: 'Error!',
        description: (error as Error).message ?? 'An error occurred.',
      });
      console.error('Error creating token:', error);
      throw error;
    }
  };

  const formItemLayoutWithOutLabel = {
    wrapperCol: {
      xs: { span: 24, offset: 0 },
      sm: { span: 20, offset: 4 },
    },
  };

  const dynamicValidateForm = reactive<CreateApiTokenFormData>(initialValues);

  const rulesRef = reactive<RulesObject<CreateApiTokenFormData>>({
    name: [
      {
        required: true,
        message: 'Token Name is required.',
        trigger: ['change', 'blur'],
        type: 'string',
      },
      {
        required: true,
        message: 'Please insert an unique token name.',
        validator: CreateisUniqueTokenName(apiTokenStore),
        trigger: ['change', 'blur'],
        type: 'string',
      },
    ],
    scopes: [
      {
        required: true,
        message: 'There already exists a Token with the SCIM scope.',
        validator: CreateOnlyOneScimToken(apiTokenStore),
        trigger: ['change', 'blur'],
        type: 'array',
      },
    ],
    inputsDisabled: [
      {
        required: false,
      },
    ],
  });

  const scopeOptions = computed(() => {
    const tokens = apiTokenStore.getApiTokens;
    const scimAlreadyExists = tokens?.some((token) =>
      token.scopes.includes('SCIM'),
    );

    return Object.values(TokenScopes).map((val) => {
      const isScim = val === 'SCIM';
      return {
        value: val,
        label: val,
        disabled: isScim && scimAlreadyExists,
      };
    });
  });

  formStore.setOnSubmit(onSubmit);
  formStore.setModel(dynamicValidateForm);
  formStore.setRules(rulesRef);
</script>

<template>
  <a-form
    ref="formRef"
    :model="dynamicValidateForm"
    v-bind="formItemLayoutWithOutLabel"
    :wrapper-col="{ span: 24 }"
  >
    <a-form-item
      has-feedback
      name="name"
      class="column"
      :whitespace="true"
      :rules="rulesRef.name"
    >
      <a-input
        id="inputCreateName"
        v-model:value="dynamicValidateForm.name"
        class="inputField"
        placeholder="Token Name"
        :disabled="dynamicValidateForm.inputsDisabled"
        :rules="rulesRef.name"
      />
    </a-form-item>
    <a-form-item
      has-feedback
      name="scopes"
      class="column"
      :whitespace="true"
      :rules="rulesRef.scopes"
    >
      <a-select
        id="inputCreateScopes"
        v-model:value="dynamicValidateForm.scopes"
        class="inputField"
        mode="multiple"
        placeholder="Token Scopes"
        :disabled="dynamicValidateForm.inputsDisabled"
        :rules="rulesRef.scopes"
        :options="scopeOptions"
      />
    </a-form-item>
  </a-form>
</template>

<style scoped>
  .column {
    margin: 0;
  }
</style>
