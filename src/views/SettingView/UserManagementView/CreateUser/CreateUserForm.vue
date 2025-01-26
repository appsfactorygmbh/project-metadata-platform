<script setup lang="ts">
  import { type FormStore, type FormSubmitType } from '@/components/Form';
  import { message, notification } from 'ant-design-vue';
  import { reactive, ref, toRaw } from 'vue';
  import type { RulesObject } from '@/components/Form/types';
  import type { CreateUserFormData } from './CreateUserFormData.ts';
  import type { Rule } from 'ant-design-vue/es/form/interface';
  import type { CreateUserModel } from '@/models/User';
  import type { UserStore } from '@/store/UserStore.ts';
  import {
    isUniqueEmail,
    isValidEmail,
    hasEightCharacters,
    hasSpecialCharacter,
    hasDigit,
    hasLowerCaseLetter,
    hasUpperCaseLetter,
  } from '@/utils/form/userValidation.ts';

  const { formStore, initialValues, userStore } = defineProps<{
    formStore: FormStore;
    initialValues: CreateUserFormData;
    userStore: UserStore;
  }>();

  const [notificationApi, contextHolder] = notification.useNotification();

  const onSubmit: FormSubmitType = (fields) => {
    try {
      const userDef: CreateUserModel = {
        email: toRaw(fields).email,
        password: toRaw(fields).password,
      };
      userStore?.create(userDef);
    } catch (error) {
      notificationApi.error({
        message: 'An error occurred. The user could not be created',
      });
      console.error('Error creating user:', error);
    } finally {
      message.success('User created', 2);
    }
  };

  const formItemLayoutWithOutLabel = {
    wrapperCol: {
      xs: { span: 24, offset: 0 },
      sm: { span: 20, offset: 4 },
    },
  };

  const dynamicValidateForm = reactive<CreateUserFormData>(initialValues);

  const validateConfirmPassword = async (_rule: Rule, value: string) => {
    if (value === '') {
      return Promise.reject('Please confirm the password.');
    } else if (value !== dynamicValidateForm.password) {
      return Promise.reject("The passwords don't match.");
    } else {
      return Promise.resolve();
    }
  };

  const rulesRef = reactive<RulesObject<CreateUserFormData>>({
    email: [
      {
        required: true,
        message: 'Please insert a valid email.',
        validator: isValidEmail,
        trigger: 'change',
        type: 'string',
      },
      {
        required: true,
        message: 'Please insert an unique email.',
        validator: isUniqueEmail,
        trigger: 'change',
        type: 'email',
      },
    ],
    password: [
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
        message: 'Please confirm the password.',
        validator: validateConfirmPassword,
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

  const formRef = ref();
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
      name="email"
      class="column"
      :whitespace="true"
      :rules="rulesRef.email"
    >
      <a-input
        id="inputCreateUserEmail"
        v-model:value="dynamicValidateForm.email"
        class="inputField"
        placeholder="E-Mail"
        :disabled="dynamicValidateForm.inputsDisabled"
        :rules="rulesRef.email"
      />
    </a-form-item>
    <a-form-item
      has-feedback
      name="password"
      class="column"
      :whitespace="false"
      :rules="rulesRef.password"
    >
      <a-input
        id="inputCreateUserPassword"
        v-model:value="dynamicValidateForm.password"
        class="inputField"
        placeholder="Password"
        :disabled="dynamicValidateForm.inputsDisabled"
        :rules="rulesRef.password"
        type="password"
      />
    </a-form-item>
    <a-form-item
      has-feedback
      name="confirmPassword"
      class="column"
      :whitespace="true"
      :rules="rulesRef.confirmPassword"
    >
      <a-input
        id="inputCreateUserConfirmPassword"
        v-model:value="dynamicValidateForm.confirmPassword"
        class="inputField"
        placeholder="Confirm Password"
        :disabled="dynamicValidateForm.inputsDisabled"
        :rules="rulesRef.confirmPassword"
        type="password"
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
