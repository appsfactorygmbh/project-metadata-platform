<script lang="ts" setup>
  import type { FormSubmitType, RulesObject } from '@/components/Form/types';
  import type { Rule } from 'ant-design-vue/es/form';
  import { type FormStore } from '@/components/Form';
  import { type PropType, reactive, toRaw } from 'vue';
  import InputField from './InputField.vue';
  import { useBusinessUnitStore } from '@/store';
  import useNotification from 'ant-design-vue/es/notification/useNotification';
  import type { BusinessUnitModel } from '@/models/BusinessUnit';

  const props = defineProps({
    businessUnitId: {
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
    businessUnitName: string;
  };

  const businessUnitStore = useBusinessUnitStore();

  const dynamicValidateForm = reactive<FormType>({
    businessUnitName: props.default,
  });

  const isUniqueBusinessUnitName = (_rule: Rule, name: string) => {
    const businessUnits: BusinessUnitModel[] =
      businessUnitStore.getBusinessUnits;
    const currentBusinessUnit: BusinessUnitModel | undefined =
      businessUnitStore.getBusinessUnit;
    if (!currentBusinessUnit) {
      return Promise.reject(new Error('Current business unit undefined'));
    }
    if (
      businessUnits?.every(
        (businessUnit) =>
          businessUnit.businessUnitName !== name ||
          name === currentBusinessUnit.businessUnitName,
      )
    ) {
      return Promise.resolve();
    }
    return Promise.reject(
      new Error('This business unit name is already in use.'),
    );
  };

  const rulesRef = reactive<RulesObject<FormType>>({
    businessUnitName: [
      {
        required: true,
        message: 'Please insert an unique business unit name.',
        validator: isUniqueBusinessUnitName,
        trigger: 'change',
        type: 'string',
      },
    ],
  });

  const [notificationApi] = useNotification();

  const onSubmit: FormSubmitType = (fields) => {
    const newBusinessUnitName = {
      businessUnitName: toRaw(fields).businessUnitName,
    };
    businessUnitStore
      .update(props.businessUnitId, newBusinessUnitName)
      .then(() => {
        notificationApi.success({
          message: 'Business Unit Name updated',
        });
      })
      .catch((error) => {
        notificationApi.error({
          message:
            'An error occurred. The business unit name could not be updated',
        });
        console.error('Error updating businessUnit:', error);
      });
  };

  props.formStore.setModel(dynamicValidateForm);
  props.formStore.setRules(rulesRef);
  props.formStore.setOnSubmit(onSubmit);
</script>

<template>
  <a-form ref="formRef" :model="dynamicValidateForm">
    <a-form-item
      :rules="rulesRef.businessUnitName"
      name="businessUnitName"
      class="formItem businessUnitName"
      has-feedback
    >
      <InputField
        v-model:value="dynamicValidateForm.businessUnitName"
        :placeholder="props.placeholder"
        :default="props.default"
        :rules="rulesRef.businessUnitName"
      />
    </a-form-item>
  </a-form>
</template>

<style lang="css" scoped>
  .formItem {
    margin: 0;
  }
</style>
