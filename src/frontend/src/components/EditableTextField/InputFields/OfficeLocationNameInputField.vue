<script lang="ts" setup>
  import type { FormSubmitType, RulesObject } from '@/components/Form/types';
  import type { Rule } from 'ant-design-vue/es/form';
  import { type FormStore } from '@/components/Form';
  import { type PropType, reactive, toRaw } from 'vue';
  import InputField from './InputField.vue';
  import { useOfficeLocationStore } from '@/store';
  import useNotification from 'ant-design-vue/es/notification/useNotification';
  import type { OfficeLocationModel } from '@/models/OfficeLocation';

  const props = defineProps({
    officeLocationId: {
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
    officeLocationName: string;
  };

  const officeLocationStore = useOfficeLocationStore();

  const dynamicValidateForm = reactive<FormType>({
    officeLocationName: props.default,
  });

  const isUniqueOfficeLocationName = (_rule: Rule, name: string) => {
    const officeLocations: OfficeLocationModel[] =
      officeLocationStore.getOfficeLocations;
    const currentOfficeLocation: OfficeLocationModel | undefined =
      officeLocationStore.getOfficeLocation;
    if (!currentOfficeLocation) {
      return Promise.reject(new Error('Current officeLocation undefined'));
    }
    if (
      officeLocations?.every(
        (officeLocation) =>
          officeLocation.officeLocationName !== name ||
          name === currentOfficeLocation.officeLocationName,
      )
    ) {
      return Promise.resolve();
    }
    return Promise.reject(
      new Error('This officeLocation name is already in use.'),
    );
  };

  const rulesRef = reactive<RulesObject<FormType>>({
    officeLocationName: [
      {
        required: true,
        message: 'Please insert an unique officeLocation name.',
        validator: isUniqueOfficeLocationName,
        trigger: 'change',
        type: 'string',
      },
    ],
  });

  const [notificationApi] = useNotification();

  const onSubmit: FormSubmitType = (fields) => {
    const newOfficeLocationName = {
      officeLocationName: toRaw(fields).officeLocationName,
    };
    officeLocationStore
      .update(props.officeLocationId, newOfficeLocationName)
      .then(() => {
        notificationApi.success({
          message: 'OfficeLocation Name updated',
        });
      })
      .catch((error) => {
        notificationApi.error({
          message:
            'An error occurred. The officeLocation name could not be updated',
        });
        console.error('Error updating officeLocation:', error);
      });
  };

  props.formStore.setModel(dynamicValidateForm);
  props.formStore.setRules(rulesRef);
  props.formStore.setOnSubmit(onSubmit);
</script>

<template>
  <a-form ref="formRef" :model="dynamicValidateForm">
    <a-form-item
      :rules="rulesRef.officeLocationName"
      name="officeLocationName"
      class="formItem officeLocationName"
      has-feedback
    >
      <InputField
        v-model:value="dynamicValidateForm.officeLocationName"
        :placeholder="props.placeholder"
        :default="props.default"
        :rules="rulesRef.officeLocationName"
      />
    </a-form-item>
  </a-form>
</template>

<style lang="css" scoped>
  .formItem {
    margin: 0;
  }
</style>
