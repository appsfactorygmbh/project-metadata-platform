<script lang="ts" setup>
  import { type PropType, reactive, computed, ref } from 'vue';
  import type { FormSubmitType } from '@/components/Form/types';
  import type { Rule } from 'ant-design-vue/es/form';
  import { type FormStore } from '@/components/Form';
  import InputField from './InputField.vue';
  import { useTeamStore } from '@/store';
  import useNotification from 'ant-design-vue/es/notification/useNotification';

  const props = defineProps({
    teamId: {
      type: Number,
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
      default: '',
    },
    default: {
      type: String,
      required: true,
    },
    rules: {
      type: Array as PropType<Rule[]>,
      default: () => [],
    },
  });
  const teamStore = useTeamStore();
  const [notificationApi] = useNotification();
  const formRef = ref();

  const dynamicValidateForm = reactive<Record<string, string>>({
    [props.attributeName]: props.default,
  });

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

    const payload: Record<string, string | null> = {
      [props.attributeName]: value,
    };

    await teamStore
      .update(props.teamId, payload)
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
          `Error updating ${props.attributeName} for team ${props.teamId}:`,
          error,
        );
      });
  };

  props.formStore.setModel(dynamicValidateForm);
  props.formStore.setRules({ [props.attributeName]: props.rules });
  props.formStore.setOnSubmit(onSubmit);

  const currentAttributeRulesForTemplate = computed(() => props.rules);
</script>

<template>
  <a-form ref="formRef" :model="dynamicValidateForm">
    <a-form-item
      :name="props.attributeName"
      :rules="currentAttributeRulesForTemplate"
      class="formItem"
      :has-feedback="!!(props.rules && props.rules.length > 0)"
    >
      <InputField
        v-model:value="dynamicValidateForm[props.attributeName]"
        :placeholder="props.placeholder"
        :default="props.default"
        :rules="rules"
      />
    </a-form-item>
  </a-form>
</template>

<style lang="css" scoped>
  .formItem {
    margin: 0;
  }
</style>
