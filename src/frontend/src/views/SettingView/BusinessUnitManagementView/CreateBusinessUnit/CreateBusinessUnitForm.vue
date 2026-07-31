<script setup lang="ts">
  import { type FormStore, type FormSubmitType } from '@/components/Form';
  import { App } from 'ant-design-vue';
  import { reactive, toRaw } from 'vue';
  import type { RulesObject } from '@/components/Form/types';
  import type { CreateBusinessUnitFormData } from './CreateBusinessUnitFormData.ts';
  import { CreateIsUniqueBusinessUnitName } from '@/utils/form/userValidation.ts';
  import type { CreateBusinessUnitModel } from '@/models/BusinessUnit/CreateBusinessUnitModel.ts';
  import type { BusinessUnitStore } from '@/store/BusinessUnitStore.ts';

  const { formStore, initialValues, businessUnitStore } = defineProps<{
    formStore: FormStore;
    initialValues: CreateBusinessUnitFormData;
    businessUnitStore: BusinessUnitStore;
  }>();
  const { notification } = App.useApp();
  const emit = defineEmits<(e: 'newId', id: number) => void>();
  const onSubmit: FormSubmitType = async (fields) => {
    try {
      const businessUnitDef: CreateBusinessUnitModel = {
        businessUnitName: toRaw(fields).businessUnitName,
      };
      const id = await businessUnitStore.create(businessUnitDef);
      await businessUnitStore.fetchAll();
      notification.success({
        message: 'Success!',
        description: 'Business Unit created successfully.',
      });
      emit('newId', id);
    } catch (error) {
      notification.error({
        message: 'Error!',
        description: (error as Error).message ?? 'An error occurred.',
      });
      console.error('Error creating business unit:', error);
      throw error;
    }
  };

  const formItemLayoutWithOutLabel = {
    wrapperCol: {
      xs: { span: 24, offset: 0 },
      sm: { span: 20, offset: 4 },
    },
  };

  const dynamicValidateForm =
    reactive<CreateBusinessUnitFormData>(initialValues);

  const rulesRef = reactive<RulesObject<CreateBusinessUnitFormData>>({
    businessUnitName: [
      {
        required: true,
        message: 'Please insert an unique business unit name.',
        validator: CreateIsUniqueBusinessUnitName(businessUnitStore),
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
      name="businessUnitName"
      class="column"
      :whitespace="true"
      :rules="rulesRef.businessUnitName"
    >
      <a-input
        id="inputCreateBusinessUnitBusinessUnitName"
        v-model:value="dynamicValidateForm.businessUnitName"
        class="inputField"
        placeholder="Business Unit Name"
        :disabled="dynamicValidateForm.inputsDisabled"
        :rules="rulesRef.businessUnitName"
      />
    </a-form-item>
  </a-form>
</template>

<style scoped>
  .column {
    margin: 0;
  }
</style>
