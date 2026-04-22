<script lang="ts" setup>
  import type { PropType } from 'vue';
  import { reactive, ref, watch } from 'vue';
  import type { FormSubmitType } from '@/components/Form/types';
  import { type FormStore } from '@/components/Form';
  import type { UpdateUserModel } from '@/models/User';
  import { useUserStore } from '@/store';
  import useNotification from 'ant-design-vue/es/notification/useNotification';
  import { PatchOperations } from '@/api/generated';
  import { useEditing } from '@/utils/hooks';
  import EditButtons from '@/components/EditableTextField/EditButtons.vue';

  const props = defineProps({
    userId: {
      type: String,
      required: true,
    },
    attributeName: {
      type: String,
      required: true,
    },
    label: {
      type: String,
      required: false,
      default: undefined,
    },
    formStore: {
      type: Object as PropType<FormStore>,
      required: true,
    },
    default: {
      type: Boolean,
      required: true,
    },
    isLoading: {
      type: Boolean,
      required: true,
    },
    isEditingKey: {
      type: String,
      required: true,
    },
    hasEditKeys: {
      type: Boolean,
      required: true,
    },
  });

  const emit = defineEmits(['savedChanges']);

  const userStore = useUserStore();
  const [notificationApi] = useNotification();

  // Hook into the editing state
  const { isEditing } = useEditing(props.isEditingKey);

  // Initialize the reactive form state
  const dynamicValidateForm = reactive<Record<string, boolean>>({
    [props.attributeName]: props.default,
  });

  // Keep the switch in sync if the parent updates the default value (e.g., after an abort)
  watch(
    () => props.default,
    (newVal) => {
      dynamicValidateForm[props.attributeName] = newVal;
    },
  );

  const onSubmit: FormSubmitType = async (fields) => {
    const value = fields[props.attributeName];
    console.log(`value read from the store (${props.attributeName}): ${value}`);

    if (value === undefined) {
      console.error(
        `[DynamicSwitchEditor] Value for ${props.attributeName} is undefined during submission.`,
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
          op: PatchOperations.Replace,
          path: props.attributeName,
          value: value,
        },
      ],
    };

    await userStore
      .update(props.userId, payload)
      .then((res) => {
        console.log('res successful ' + JSON.stringify(res));
        notificationApi.success({
          message: `${props.label || props.attributeName} updated successfully.`,
        });
        emit('savedChanges');
      })
      .catch((error) => {
        const errorMessage =
          error.response?.data?.message ||
          error.message ||
          `An error occurred.`;
        notificationApi.error({
          message: `Error updating ${props.attributeName}`,
          description: `${errorMessage} The switch could not be updated.`,
        });
        console.error(
          `Error updating ${props.attributeName} for user ${props.userId}:`,
          error,
        );
        // Revert the switch locally if the API call fails
        dynamicValidateForm[props.attributeName] = props.default;
      });
  };

  props.formStore.setModel(dynamicValidateForm);
  props.formStore.setOnSubmit(onSubmit);

  const formRef = ref();
</script>

<template>
  <a-card
    :body-style="{
      display: 'flex',
      padding: '5px',
      alignItems: 'center',
      height: 'fit-content',
      overflow: 'auto',
    }"
    class="info"
  >
    <label v-if="label != null" class="label">{{ label }}:</label>

    <template v-if="!isLoading">
      <a-form ref="formRef" :model="dynamicValidateForm" class="switch-form">
        <a-form-item :name="props.attributeName" class="formItem">
          <a-switch
            v-model:checked="dynamicValidateForm[props.attributeName]"
            class="custom-color-switch"
            :disabled="!isEditing"
          />
        </a-form-item>
      </a-form>

      <EditButtons
        v-if="hasEditKeys"
        class="editButton"
        :is-editing-key="props.isEditingKey"
        :is-loading="props.isLoading"
        :safe-disabled="props.isLoading"
        :form-store="props.formStore"
        @saved-changes="emit('savedChanges')"
      />
    </template>

    <a-skeleton
      v-else
      active
      :paragraph="false"
      style="margin-left: 1em; width: 4em"
    />
  </a-card>
</template>

<style lang="css" scoped>
  .formItem {
    margin: 0;
  }

  :deep(.custom-color-switch.ant-switch-checked),
  :deep(.custom-color-switch.ant-switch-checked:hover) {
    background-color: #27d157;
  }

  :deep(.custom-color-switch:not(.ant-switch-checked)),
  :deep(.custom-color-switch:not(.ant-switch-checked):hover) {
    background-color: #ff002e;
  }
</style>
