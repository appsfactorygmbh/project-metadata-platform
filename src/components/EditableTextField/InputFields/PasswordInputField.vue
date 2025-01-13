<script setup lang="ts">
  import type { FormStore } from '@/components/Form';
  import type { FormSubmitType, RulesObject } from '@/components/Form/types';
  import { userStoreSymbol } from '@/store/injectionSymbols';
  import {
    hasEightCharacters,
    hasSpecialCharacter,
    hasDigit,
    hasUpperCaseLetter,
    hasLowerCaseLetter,
  } from '@/utils/form/userValidation';
  import type { Rule } from 'ant-design-vue/es/form';
  import useNotification from 'ant-design-vue/es/notification/useNotification';

  const { formStore, placeholder, userId } = defineProps({
    userId: {
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
  });

  type EditPasswordFormData = {
    currentPassword: string;
    newPassword: string;
    confirmPassword: string;
  };

  const formRef = ref();
  const userStore = inject(userStoreSymbol)!;

  const dynamicValidateForm = reactive<EditPasswordFormData>({
    currentPassword: '',
    newPassword: '',
    confirmPassword: '',
  });

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
        message: 'Please insert at least 8 characters.',
        validator: hasEightCharacters,
        trigger: 'change',
        type: 'string',
      },
      {
        required: true,
        message: 'Please insert a special character.',
        validator: hasSpecialCharacter,
        trigger: 'change',
        type: 'string',
      },
      {
        required: true,
        message: 'Please insert a number.',
        validator: hasDigit,
        trigger: 'change',
        type: 'string',
      },
      {
        required: true,
        message: 'Please insert a upper case letter.',
        validator: hasUpperCaseLetter,
        trigger: 'change',
        type: 'string',
      },
      {
        required: true,
        message: 'Please insert a lower case letter.',
        validator: hasLowerCaseLetter,
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

  const [notificationApi] = useNotification();

  const onSubmit: FormSubmitType = (fields) => {
    try {
      userStore?.update(userId, { password: toRaw(fields).newPassword });
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

  formStore.setOnSubmit(onSubmit);
  formStore.setModel(dynamicValidateForm);
  formStore.setRules(rulesRef);
</script>

<template>
  <a-form ref="formRef" :model="dynamicValidateForm" class="form">
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
        :placeholder="placeholder"
        :rules="rulesRef.confirmPassword"
        class="password"
      >
      </a-input>
    </a-form-item>
  </a-form>
</template>
