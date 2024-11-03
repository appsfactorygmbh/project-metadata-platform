<script setup lang="ts">
  import { type FormSubmitType } from '@/components/Form';
  import { notification } from 'ant-design-vue';
  import { type FormStore } from '@/components/Form';
  import { reactive, ref, toRaw } from 'vue';
  import type { RulesObject } from '@/components/Form/types';
  import type { CreateUserFormData } from './CreateUserFormData.ts';
  import type { Rule } from 'ant-design-vue/es/form/interface';
  import { message } from 'ant-design-vue';
  import type { CreateUserModel, UserModel } from '@/models/User';
  import type { UserStore } from '@/store/UserStore.ts';

  const { formStore, initialValues, userStore } = defineProps<{
    formStore: FormStore;
    initialValues: CreateUserFormData;
    userStore: UserStore;
  }>();

  const [notificationApi, contextHolder] = notification.useNotification();

  const onSubmit: FormSubmitType = (fields) => {
    try {
      const userDef: CreateUserModel = {
        name: toRaw(fields).name,
        username: toRaw(fields).username,
        email: toRaw(fields).email,
        password: toRaw(fields).password,
      };
      userStore?.createUser(userDef);
    } catch {
      notificationApi.error({
        message: 'An error occurred. The user could not be created',
      });
      console.log('Error creating user');
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

  const isValidPassword = (pw: string) => {
    const pwRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*[^A-Za-z0-9]).{8,}$/;
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
          'Please enter a Password, which has upper/lower case letters, special characters and at least 8 characters long.',
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
    } else if (value !== dynamicValidateForm.password) {
      return Promise.reject("The passwords don't match.");
    } else {
      return Promise.resolve();
    }
  };

  // Creates a regex for all possible E-Mail addresses and checks if the given one fits the pattern
  const validateEmail = (_rule: Rule, value: string) => {
    const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;

    if (value && emailRegex.test(value)) {
      return Promise.resolve();
    } else {
      return Promise.reject('Please enter a valid email.');
    }
  };

  const validateUsername = (_rule: Rule, value: string) => {
    if (value === '') {
      return Promise.reject('Please enter a username.');
    } else {
      const users: UserModel[] = userStore?.getUsers;

      if (value === users?.find((user) => user.username === value)?.username) {
        return Promise.reject('Username already exists.');
      } else {
        return Promise.resolve();
      }
    }
  };

  const rulesRef = reactive<RulesObject<CreateUserFormData>>({
    name: [
      {
        required: true,
        message: 'Please insert a name.',
        trigger: 'change',
        type: 'string',
      },
    ],
    username: [
      {
        required: true,
        message: 'Please insert an unique username.',
        validator: validateUsername,
        trigger: 'change',
        type: 'string',
      },
    ],
    email: [
      {
        required: true,
        message: 'Please insert a valid email.',
        validator: validateEmail,
        trigger: 'change',
        type: 'string',
      },
    ],
    password: [
      {
        required: true,
        message:
          'Please insert a Password, which has upper/lower case letters, special characters and at least 8 characters.',
        validator: validatePassword,
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
      name="name"
      class="column"
      :whitespace="true"
      :rules="rulesRef.name"
    >
      <a-input
        id="inputCreateUserName"
        v-model:value="dynamicValidateForm.name"
        class="inputField"
        placeholder="Name"
        :disabled="dynamicValidateForm.inputsDisabled"
        :rules="rulesRef.name"
      >
      </a-input>
    </a-form-item>
    <a-form-item
      has-feedback
      name="username"
      class="column"
      :whitespace="true"
      :rules="rulesRef.username"
    >
      <a-input
        id="inputCreateUserUsername"
        v-model:value="dynamicValidateForm.username"
        class="inputField"
        placeholder="Username"
        :disabled="dynamicValidateForm.inputsDisabled"
        :rules="rulesRef.username"
      >
      </a-input>
    </a-form-item>
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
      >
      </a-input>
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
      >
      </a-input>
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
      >
      </a-input>
    </a-form-item>
  </a-form>
  <contextHolder></contextHolder>
</template>

<style scoped>
  .column {
    margin: 0;
  }
</style>
