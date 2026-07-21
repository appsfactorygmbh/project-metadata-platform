<script setup lang="ts">
  import { type FormStore, type FormSubmitType } from '@/components/Form';
  import { message } from 'ant-design-vue';
  import { reactive, toRaw } from 'vue';
  import type { RulesObject } from '@/components/Form/types';
  import type { CreateCompanyFormData } from './CreateCompanyFormData.ts';
  import { CreateIsUniqueCompanyName } from '@/utils/form/userValidation.ts';
  import type { CreateCompanyModel } from '@/models/Company/CreateCompanyModel.ts';
  import type { CompanyStore } from '@/store/CompanyStore.ts';

  const { formStore, initialValues, companyStore } = defineProps<{
    formStore: FormStore;
    initialValues: CreateCompanyFormData;
    companyStore: CompanyStore;
  }>();

  const emit = defineEmits<(e: 'newId', id: number) => void>();
  const onSubmit: FormSubmitType = async (fields) => {
    try {
      const companyDef: CreateCompanyModel = {
        companyName: toRaw(fields).companyName,
      };
      const id = await companyStore.create(companyDef);
      await companyStore.fetchAll();
      message.success('Company created', 2);
      emit('newId', id);
    } catch (error) {
      message.error('An error occurred. The company could not be created', 10);
      console.error('Error creating company:', error);
    }
  };

  const formItemLayoutWithOutLabel = {
    wrapperCol: {
      xs: { span: 24, offset: 0 },
      sm: { span: 20, offset: 4 },
    },
  };

  const dynamicValidateForm = reactive<CreateCompanyFormData>(initialValues);

  const rulesRef = reactive<RulesObject<CreateCompanyFormData>>({
    companyName: [
      {
        required: true,
        message: 'Please insert an unique company name.',
        validator: CreateIsUniqueCompanyName(companyStore),
        trigger: 'change',
        type: 'string',
      },
    ],
    inputsDisabled: [
      {
        required: false,
      },
    ],
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
      name="companyName"
      class="column"
      :whitespace="true"
      :rules="rulesRef.companyName"
    >
      <a-input
        id="inputCreateCompanyCompanyName"
        v-model:value="dynamicValidateForm.companyName"
        class="inputField"
        placeholder="Company Name"
        :disabled="dynamicValidateForm.inputsDisabled"
        :rules="rulesRef.companyName"
      />
    </a-form-item>
  </a-form>
</template>

<style scoped>
  .column {
    margin: 0;
  }
</style>
