<script lang="ts" setup>
  import { useEditing } from '@/utils/hooks/useEditing';
  import { ref } from 'vue';
  import { useFormStore } from '@/components/Form';
  import { inject, reactive, toRaw } from 'vue';
  import type { PropType } from 'vue';
  import type { FormSubmitType, RulesObject } from '../Form/types';
  import type { Rule } from 'ant-design-vue/es/form';
  import { userStoreSymbol } from '@/store/injectionSymbols';
  import useNotification from 'ant-design-vue/es/notification/useNotification';

  type EditPasswordFormData = {
    currentPassword: string;
    newPassword: string;
    confirmPassword: string;
  };

  const formStore = useFormStore('patchPasswordForm');
  const userStore = inject(userStoreSymbol)!;

  const props = defineProps({
    value: {
      type: String,
      required: true,
    },
    label: {
      type: String,
      required: true,
    },
    isLoading: {
      type: Boolean,
      required: true,
    },
    type: {
      type: String as PropType<'username' | 'email'>,
      default: 'text',
    },
    isEditingKey: {
      type: String,
      required: true,
    },
    userId: {
      type: Number,
      required: true,
    },
  });

  const fieldValue = ref('');

  watch(
    () => props.value,
    (newValue) => {
      fieldValue.value = newValue;
    },
  );

  const { isEditing, startEditing, stopEditing } = useEditing(
    props.isEditingKey,
  );

  const [notificationApi, contextHolder] = useNotification();

  const onSubmit: FormSubmitType = (fields) => {
    try {
      const password = {
        password: toRaw(fields).newPassword,
      };
      userStore?.patchUser(props.userId, password);
    } catch (error) {
      notificationApi.error({
        message: 'An error occurred. The user could not be created',
      });
      console.error('Error creating user:', error);
    } finally {
      notificationApi.success({
        message: 'Password updated',
      });
    }
  };

  const safeEdits = async () => {
    await formStore.submit();
    formStore.resetFields();
    stopEditing();
  };

  const cancleEdit = () => {
    formStore.resetFields();
    stopEditing();
  };

  const formRef = ref();

  const dynamicValidateForm = reactive<EditPasswordFormData>({
    currentPassword: '',
    newPassword: '',
    confirmPassword: '',
  });

  const isValidPassword = (pw: string) => {
    const pwRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}$/;
    if (pwRegex.test(pw)) {
      return true;
    }
  };

  const validatePassword = async (_rule: Rule, value: string) => {
    if (value === '') {
      return Promise.reject('Please enter a password.');
    } else {
      if (!isValidPassword(value)) {
        return Promise.reject(
          'Please enter a Password, which has upper/lower case letters, special characters, a digit and at least 8 characters.',
        );
      }

      if (dynamicValidateForm.confirmPassword !== '') {
        formRef.value.validateFields('confirmPassword');
      }

      return Promise.resolve();
    }
  };

  const validateConfirmPassword = async (_rule: Rule, value: string) => {
    if (value === '') {
      return Promise.reject('Please confirm the password.');
    } else if (value !== dynamicValidateForm.newPassword) {
      return Promise.reject("The passwords don't match.");
    } else {
      return Promise.resolve();
    }
  };

  const rulesRef = reactive<RulesObject<EditPasswordFormData>>({
    currentPassword: [
      {
        required: true,
        message: 'Please enter your current password.',
        trigger: 'change',
        type: 'string',
      },
    ],
    newPassword: [
      {
        required: true,
        message:
          'Please enter a Password, which has upper/lower case letters, special characters, a digit and at least 8 characters.',
        validator: validatePassword,
        trigger: 'change',
        type: 'string',
      },
    ],
    confirmPassword: [
      {
        required: true,
        message: 'This password does not match your new Password',
        validator: validateConfirmPassword,
        trigger: 'change',
        type: 'string',
      },
    ],
  });

  formStore.setOnSubmit(onSubmit);
  formStore.setModel(dynamicValidateForm);
  formStore.setRules(rulesRef);
</script>

<template>
  <contextHolder></contextHolder>
  <a-card class="info">
    <label class="label">{{ label }}:</label>
    <template v-if="!isLoading">
      <p v-if="!isEditing" class="text passwordLabel">{{ value }}</p>
      <a-form v-else ref="formRef" :model="dynamicValidateForm" class="form">
        <a-form-item
          name="currentPassword"
          class="formItem"
          :whitespace="true"
          :rules="rulesRef.currentPassword"
        >
          <a-input
            v-model:value="dynamicValidateForm.currentPassword"
            type="password"
            placeholder="Enter your current password"
            :rules="rulesRef.currentPassword"
          >
          </a-input>
        </a-form-item>
        <a-form-item
          has-feedback
          name="newPassword"
          class="formItem"
          :whitespace="true"
          :rules="rulesRef.newPassword"
        >
          <a-input
            v-model:value="dynamicValidateForm.newPassword"
            type="password"
            placeholder="Enter your new password"
            :rules="rulesRef.newPassword"
          >
          </a-input>
        </a-form-item>
        <a-form-item
          has-feedback
          name="confirmPassword"
          class="lastFormItem"
          :whitespace="true"
          :rules="rulesRef.confirmPassword"
        >
          <a-input
            v-model:value="dynamicValidateForm.confirmPassword"
            type="password"
            placeholder="Confirm your new password"
            :rules="rulesRef.confirmPassword"
            class="password"
          >
          </a-input>
        </a-form-item>
      </a-form>

      <EditButtons
        :is-editing="isEditing"
        :is-loading="isLoading"
        :safe-disabled="
          dynamicValidateForm.confirmPassword == '' ||
          dynamicValidateForm.currentPassword == '' ||
          dynamicValidateForm.newPassword == ''
        "
        @cancle-edit="cancleEdit"
        @safe-edits="safeEdits"
        @start-editing="startEditing"
      />
    </template>
    <a-skeleton
      v-else
      active
      :paragraph="false"
      style="margin-left: 1em; width: 10em"
    />
  </a-card>
</template>

<style>
  .edit {
    border: none;
    margin: 0.6em 0 0.6em;
    margin-left: auto;
    width: 2em;
    height: 2em;
    display: flex;
    justify-content: center;
    align-items: center;
  }

  .password {
    display: flex;
    margin-bottom: 10px;
  }

  .info {
    border: none;
    width: 100%;
    height: auto;
    max-width: 100%;
    font-size: 1.3em;
    font-weight: bold;
    display: flex;
    flex-flow: column wrap;
    justify-content: center;
  }

  .ant-card-body {
    padding: 12px !important;
    display: flex;
  }

  .info label {
    width: 5em;
    min-width: 5em;
    margin-right: 3em;
  }

  .input {
    margin: 0 !important;
  }
  .text {
    font-weight: 400;
    margin: 0;
  }

  .formItem {
    margin-bottom: 2em;
  }

  .lastFormItem {
    margin: 0;
  }

  .form {
    max-width: 400px;
  }
</style>
