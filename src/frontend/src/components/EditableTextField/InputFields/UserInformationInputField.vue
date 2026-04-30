<script lang="ts" setup>
  import type { FormSubmitType } from '@/components/Form/types';
  import { type FormStore } from '@/components/Form';
  import { type PropType, reactive } from 'vue';
  import type { UpdateUserModel } from '@/models/User';
  import InputField from './InputField.vue';
  import { useUserStore } from '@/store';
  import useNotification from 'ant-design-vue/es/notification/useNotification';
  import { PatchOperations } from '@/api/generated';

  const props = defineProps({
    userId: {
      type: String,
      required: true,
    },
    attributeName: {
      type: String,
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

  const userStore = useUserStore();

  const dynamicValidateForm = reactive<Record<string, string>>({
    [props.attributeName]: props.default,
  });

  const [notificationApi] = useNotification();

  const onSubmit: FormSubmitType = async (fields) => {
    const value = fields[props.attributeName];
    console.log(`value read from the store (${props.attributeName}): ${value}`);
    if (value === undefined) {
      console.error(
        `[DynamicFieldEditor] Value for ${props.attributeName} is undefined during submission.`,
      );
      notificationApi.error({
        message: `Submission Error`,
        description: `Value for ${props.attributeName} is missing. Please try again.`,
      });
      return;
    }

    const payload: UpdateUserModel = {
      operations: [
        {
          op:
            value === null || value === ''
              ? PatchOperations.Remove
              : PatchOperations.Replace,
          path: props.attributeName,
          value: value,
        },
      ],
    };

    await userStore
      .update(props.userId, payload)
      .then((res) => {
        console.log('res successfull ' + JSON.stringify(res));
        notificationApi.success({
          message: `${props.attributeName} updated successfully.`,
        });
      })
      .catch((error) => {
        const errorMessage =
          error.response?.data?.message ||
          error.message ||
          `An error occurred.`;
        notificationApi.error({
          message: `Error updating ${props.attributeName}`,
          description: `${errorMessage} The ${props.attributeName.toLowerCase()} could not be updated.`,
        });
        console.error(
          `Error updating ${props.attributeName} for team ${props.userId}:`,
          error,
        );
      });
  };

  props.formStore.setModel(dynamicValidateForm);
  props.formStore.setOnSubmit(onSubmit);
</script>

<template>
  <a-form ref="formRef" :model="dynamicValidateForm">
    <a-form-item name="props.attributeName" class="formItem">
      <InputField
        v-model:value="dynamicValidateForm[props.attributeName]"
        :placeholder="props.placeholder"
        :default="props.default"
      />
    </a-form-item>
  </a-form>
</template>

<style lang="css" scoped>
  .formItem {
    margin: 0;
  }
</style>
