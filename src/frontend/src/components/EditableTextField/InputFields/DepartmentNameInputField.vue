<script lang="ts" setup>
  import type { FormSubmitType, RulesObject } from '@/components/Form/types';
  import type { Rule } from 'ant-design-vue/es/form';
  import { type FormStore } from '@/components/Form';
  import { type PropType, reactive, toRaw } from 'vue';
  import InputField from './InputField.vue';
  import { useDepartmentStore } from '@/store';
  import useNotification from 'ant-design-vue/es/notification/useNotification';
  import type { DepartmentModel } from '@/models/Department';

  const props = defineProps({
    departmentId: {
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
    departmentName: string;
  };

  const departmentStore = useDepartmentStore();

  const dynamicValidateForm = reactive<FormType>({
    departmentName: props.default,
  });

  const isUniqueDepartmentName = (_rule: Rule, name: string) => {
    const departments: DepartmentModel[] = departmentStore.getDepartments;
    const currentDepartment: DepartmentModel | undefined =
      departmentStore.getDepartment;
    if (!currentDepartment) {
      return Promise.reject(new Error('Current department undefined'));
    }
    if (
      departments?.every(
        (department) =>
          department.departmentName !== name ||
          name === currentDepartment.departmentName,
      )
    ) {
      return Promise.resolve();
    }
    return Promise.reject(new Error('This department name is already in use.'));
  };

  const rulesRef = reactive<RulesObject<FormType>>({
    departmentName: [
      {
        required: true,
        message: 'Please insert an unique department name.',
        validator: isUniqueDepartmentName,
        trigger: 'change',
        type: 'string',
      },
    ],
  });

  const [notificationApi] = useNotification();

  const onSubmit: FormSubmitType = (fields) => {
    const newDepartmentName = {
      departmentName: toRaw(fields).departmentName,
    };
    departmentStore
      .update(props.departmentId, newDepartmentName)
      .then(() => {
        notificationApi.success({
          message: 'Department Name updated',
        });
      })
      .catch((error) => {
        notificationApi.error({
          message:
            'An error occurred. The department name could not be updated',
        });
        console.error('Error updating department:', error);
      });
  };

  props.formStore.setModel(dynamicValidateForm);
  props.formStore.setRules(rulesRef);
  props.formStore.setOnSubmit(onSubmit);
</script>

<template>
  <a-form ref="formRef" :model="dynamicValidateForm">
    <a-form-item
      :rules="rulesRef.departmentName"
      name="departmentName"
      class="formItem departmentName"
      has-feedback
    >
      <InputField
        v-model:value="dynamicValidateForm.departmentName"
        :placeholder="props.placeholder"
        :default="props.default"
        :rules="rulesRef.departmentName"
      />
    </a-form-item>
  </a-form>
</template>

<style lang="css" scoped>
  .formItem {
    margin: 0;
  }
</style>
