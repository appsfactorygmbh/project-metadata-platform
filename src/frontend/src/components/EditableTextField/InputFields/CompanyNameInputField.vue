<script lang="ts" setup>
  import type { FormSubmitType, RulesObject } from '@/components/Form/types';
  import type { Rule } from 'ant-design-vue/es/form';
  import { type FormStore } from '@/components/Form';
  import { type PropType, reactive, toRaw } from 'vue';
  import InputField from './InputField.vue';
  import { useCompanyStore } from '@/store';
  import useNotification from 'ant-design-vue/es/notification/useNotification';
  import type { CompanyModel } from '@/models/Company';

  const props = defineProps({
    companyId: {
      type: Number,
      required: true,
    },
    formStore: {
      type: Object as PropType<FormStore>,
      required: true,
    },
    placeholder: {
      type: String,
      required: true,
    },
    default: {
      type: String,
      required: true,
    },
  });

  type FormType = {
    companyName: string;
  };

  const companyStore = useCompanyStore();

  const dynamicValidateForm = reactive<FormType>({
    companyName: props.default,
  });

  const isUniqueCompanyName = (_rule: Rule, name: string) => {
    const companies: CompanyModel[] = companyStore.getCompanies;
    const currentCompany: CompanyModel | undefined = companyStore.getCompany;
    if (!currentCompany) {
      return Promise.reject(new Error('Current company undefined'));
    }
    if (
      companies?.every(
        (company) =>
          company.companyName !== name || name === currentCompany.companyName,
      )
    ) {
      return Promise.resolve();
    }
    return Promise.reject(new Error('This company name is already in use.'));
  };

  const rulesRef = reactive<RulesObject<FormType>>({
    companyName: [
      {
        required: true,
        message: 'Please insert an unique company name.',
        validator: isUniqueCompanyName,
        trigger: 'change',
        type: 'string',
      },
    ],
  });

  const [notificationApi] = useNotification();

  const onSubmit: FormSubmitType = (fields) => {
    const newCompanyName = {
      companyName: toRaw(fields).companyName,
    };
    companyStore
      .update(props.companyId, newCompanyName)
      .then(() => {
        notificationApi.success({
          message: 'Company Name updated',
        });
      })
      .catch((error) => {
        notificationApi.error({
          message: 'An error occurred. The company name could not be updated',
        });
        console.error('Error updating company:', error);
      });
  };

  props.formStore.setModel(dynamicValidateForm);
  props.formStore.setRules(rulesRef);
  props.formStore.setOnSubmit(onSubmit);
</script>

<template>
  <a-form ref="formRef" :model="dynamicValidateForm">
    <a-form-item
      :rules="rulesRef.companyName"
      name="companyName"
      class="formItem companyName"
      has-feedback
    >
      <InputField
        v-model:value="dynamicValidateForm.companyName"
        :placeholder="props.placeholder"
        :default="props.default"
        :rules="rulesRef.companyName"
      />
    </a-form-item>
  </a-form>
</template>

<style lang="css" scoped>
  .formItem {
    margin: 0;
  }
</style>
