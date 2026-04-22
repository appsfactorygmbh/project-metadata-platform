<script lang="ts" setup>
  import type { FormSubmitType } from '@/components/Form/types';
  import { type FormStore } from '@/components/Form';
  import type { UpdateUserModel } from '@/models/User';

  import { type PropType, reactive, ref } from 'vue';
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

  const dynamicValidateForm = reactive<Record<string, string[] | string>>({
    [props.attributeName]: props.default
      ? props.default.split(',').map((item) => item.trim())
      : [],
  });

  const [notificationApi] = useNotification();

  const searchValue = ref('');

  const onSearch = (val: string) => {
    searchValue.value = val;
  };

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
    if (searchValue.value.trim()) {
      const newTag = searchValue.value.trim();
      if (!value.includes(newTag)) {
        value.push(newTag);
      }
      searchValue.value = '';
    }

    const payload: UpdateUserModel = {
      operations: [
        {
          op:
            value === null || value.length == 0
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

  const handlePaste = (e: ClipboardEvent) => {
    e.preventDefault();

    const pastedData = e.clipboardData?.getData('text');
    if (!pastedData) return;

    const newTags = pastedData
      .split(',')
      .map((tag) => tag.trim())
      .filter((tag) => tag.length > 0);

    const currentValues = dynamicValidateForm[props.attributeName] as string[];

    dynamicValidateForm[props.attributeName] = Array.from(
      new Set([...currentValues, ...newTags]),
    );
  };

  props.formStore.setModel(dynamicValidateForm as any);
  props.formStore.setOnSubmit(onSubmit);

  const formRef = ref();
</script>

<template>
  <a-form ref="formRef" :model="dynamicValidateForm">
    <a-form-item :name="props.attributeName" class="formItem">
      <a-select
        v-model:value="dynamicValidateForm[props.attributeName]"
        mode="tags"
        :placeholder="props.placeholder"
        :token-separators="[',']"
        @paste="handlePaste"
        @search="onSearch"
      >
      </a-select>
    </a-form-item>
  </a-form>
</template>

<style lang="css" scoped>
  .formItem {
    margin: 0;
    min-width: 210px;
    max-width: 100%;
  }
</style>
