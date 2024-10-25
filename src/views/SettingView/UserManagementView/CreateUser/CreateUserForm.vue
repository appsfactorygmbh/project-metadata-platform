<script setup lang="ts">
  import { type FormSubmitType } from '@/components/Form';
  import { notification } from 'ant-design-vue';
  import { type FormStore } from '@/components/Form';
  import { ref, toRaw, reactive } from 'vue';
  import type { RulesObject } from '@/components/Form/types';
  import type { CreateUserFormData } from './CreateUserFormData.ts';
  import type { Rule } from 'ant-design-vue/es/form/interface';
  import { message } from 'ant-design-vue';

  const { formStore, initialValues } = defineProps<{
    formStore: FormStore;
    initialValues: CreateUserFormData;
  }>();

  const [notificationApi, contextHolder] = notification.useNotification();

  const onSubmit: FormSubmitType = (fields) => {
    try {
      console.log(fields);

      const userDef: CreateUserModel = {
        id: 0,
        name: toRaw(fields).name,
        username: toRaw(fields).username,
        email: toRaw(fields).email,
        password: toRaw(fields).password,
      };
      userDef;
      //userStore.createUser(userDef);
    } catch {
      notificationApi.error({
        message: 'An error occurred. The user could not be created',
      });
      console.log('fehler');
    } finally {
      message.success('User created', 2);
    }
  };

  type CreateUserModel = {
    id: number;
    name: string;
    username: string;
    email: string;
    password: string;
  };

  const formItemLayoutWithOutLabel = {
    wrapperCol: {
      xs: { span: 24, offset: 0 },
      sm: { span: 20, offset: 4 },
    },
  };

  const dynamicValidateForm = reactive<CreateUserFormData>(initialValues);

  const validatePassword = async (_rule: Rule, value: string) => {
    console.log(value, dynamicValidateForm.confirmPassword);

    if (value === '') {
      return Promise.reject('Please enter a password.');
    } else {
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

    if (emailRegex.test(value)) {
      return Promise.resolve();
    } else {
      return Promise.reject('Please enter a valid email.');
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
        message: 'Please insert an username.',
        trigger: 'change',
        type: 'string',
      },
    ],
    email: [
      {
        required: true,
        message: 'Please insert an email.',
        validator: validateEmail,
        trigger: 'change',
        type: 'string',
      },
    ],
    password: [
      {
        required: true,
        message: 'Please insert a password.',
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
  >
    <a-form-item
      name="name"
      class="column"
      :no-style="true"
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
      name="username"
      class="column"
      :no-style="true"
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
      :no-style="true"
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
      :no-style="true"
      :whitespace="true"
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
      :no-style="true"
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
