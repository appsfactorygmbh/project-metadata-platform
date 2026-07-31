<script setup lang="ts">
  import { type FormStore, type FormSubmitType } from '@/components/Form';
  import { App } from 'ant-design-vue';
  import { reactive, toRaw } from 'vue';
  import type { RulesObject } from '@/components/Form/types';
  import type { CreateDepartmentFormData } from './CreateDepartmentFormData.ts';
  import { CreateIsUniqueDepartmentName } from '@/utils/form/userValidation.ts';
  import type { CreateDepartmentModel } from '@/models/Department/CreateDepartmentModel.ts';
  import type { DepartmentStore } from '@/store/DepartmentStore.ts';

  const { formStore, initialValues, departmentStore } = defineProps<{
    formStore: FormStore;
    initialValues: CreateDepartmentFormData;
    departmentStore: DepartmentStore;
  }>();
  const { notification } = App.useApp();
  const emit = defineEmits<(e: 'newId', id: number) => void>();
  const onSubmit: FormSubmitType = async (fields) => {
    try {
      const departmentDef: CreateDepartmentModel = {
        departmentName: toRaw(fields).departmentName,
      };
      const id = await departmentStore.create(departmentDef);
      await departmentStore.fetchAll();
      notification.success({
        message: 'Success!',
        description: 'Department created successfully.',
      });
      emit('newId', id);
    } catch (error) {
      notification.error({
        message: 'Error!',
        description: (error as Error).message ?? 'An error occurred.',
      });
      console.error('Error creating department:', error);
      throw error;
    }
  };

  const formItemLayoutWithOutLabel = {
    wrapperCol: {
      xs: { span: 24, offset: 0 },
      sm: { span: 20, offset: 4 },
    },
  };

  const dynamicValidateForm = reactive<CreateDepartmentFormData>(initialValues);

  const rulesRef = reactive<RulesObject<CreateDepartmentFormData>>({
    departmentName: [
      {
        required: true,
        message: 'Please insert an unique department name.',
        validator: CreateIsUniqueDepartmentName(departmentStore),
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
      name="departmentName"
      class="column"
      :whitespace="true"
      :rules="rulesRef.departmentName"
    >
      <a-input
        id="inputCreateDepartmentDepartmentName"
        v-model:value="dynamicValidateForm.departmentName"
        class="inputField"
        placeholder="Department Name"
        :disabled="dynamicValidateForm.inputsDisabled"
        :rules="rulesRef.departmentName"
      />
    </a-form-item>
  </a-form>
</template>

<style scoped>
  .column {
    margin: 0;
  }
</style>
