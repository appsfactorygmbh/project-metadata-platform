<script setup lang="ts">
  import { type FormStore, type FormSubmitType } from '@/components/Form';
  import { App } from 'ant-design-vue';
  import { reactive, toRaw } from 'vue';
  import type { RulesObject } from '@/components/Form/types';
  import type { CreateOfficeLocationFormData } from './CreateOfficeLocationFormData.ts';
  import { CreateIsUniqueOfficeLocationName } from '@/utils/form/userValidation.ts';
  import type { CreateOfficeLocationModel } from '@/models/OfficeLocation/CreateOfficeLocationModel.ts';
  import type { OfficeLocationStore } from '@/store/OfficeLocationStore.ts';

  const { formStore, initialValues, officeLocationStore } = defineProps<{
    formStore: FormStore;
    initialValues: CreateOfficeLocationFormData;
    officeLocationStore: OfficeLocationStore;
  }>();
  const { notification } = App.useApp();
  const emit = defineEmits<(e: 'newId', id: number) => void>();
  const onSubmit: FormSubmitType = async (fields) => {
    try {
      const officeLocationDef: CreateOfficeLocationModel = {
        officeLocationName: toRaw(fields).officeLocationName,
      };
      const id = await officeLocationStore.create(officeLocationDef);
      await officeLocationStore.fetchAll();
      notification.success({
        message: 'Success!',
        description: 'Office Location created successfully.',
      });
      emit('newId', id);
    } catch (error) {
      notification.error({
        message: 'Error!',
        description: (error as Error).message ?? 'An error occurred.',
      });
      console.error('Error creating office location:', error);
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
    reactive<CreateOfficeLocationFormData>(initialValues);

  const rulesRef = reactive<RulesObject<CreateOfficeLocationFormData>>({
    officeLocationName: [
      {
        required: true,
        message: 'Please insert an unique office location name.',
        validator: CreateIsUniqueOfficeLocationName(officeLocationStore),
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
      name="officeLocationName"
      class="column"
      :whitespace="true"
      :rules="rulesRef.officeLocationName"
    >
      <a-input
        id="inputCreateOfficeLocationOfficeLocationName"
        v-model:value="dynamicValidateForm.officeLocationName"
        class="inputField"
        placeholder="Office Location Name"
        :disabled="dynamicValidateForm.inputsDisabled"
        :rules="rulesRef.officeLocationName"
      />
    </a-form-item>
  </a-form>
  <contextHolder />
</template>

<style scoped>
  .column {
    margin: 0;
  }
</style>
