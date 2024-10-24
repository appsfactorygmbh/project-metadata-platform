<script setup lang="ts">
  import { type FormSubmitType } from '@/components/Form';
  import { notification } from 'ant-design-vue';
  import { type FormStore } from '@/components/Form';
  import { ref, toRaw, reactive } from 'vue';
  import type { CustomRulesObject, RulesObject } from '@/components/Form/types';
  import type { CreateUserFormData } from './CreateUserFormData.ts';
  import type { Rule } from 'ant-design-vue/es/form/interface';

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
      createUser(userDef);
    } catch {
      notificationApi.error({
        message: 'An error occurred. The plugin could not be created',
      });
      console.log('fehler');
    }
  };

  type CreateUserModel = {
    id: number;
    name: string;
    username: string;
    email: string;
    password: string;
  };

  const createUser = (userDef: CreateUserModel) => {
    userDef;
    /*const index = projectEditStore?.initialAdd(userDef);

    if (index !== undefined) {
      const newUser = {
        ...userDef,
        editKey: index,
        isDeleted: false,
      };
      projectEditStore?.addNewPlugin(newUser);
    }*/
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
    } else if (dynamicValidateForm.confirmPassword !== '') {
      formRef.value.validateField('confirmPassword');
    } else {
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

  const customRules = reactive<CustomRulesObject<CreateUserFormData>>({
    email: [
      {
        ruleTarget: 'field',
        keyProp: 'email',
        validator: (_, value) => {
          if (!isValidEmail(value)) {
            return Promise.reject('Please enter a valid email.');
          }
          return Promise.resolve();
        },
        message: 'Please enter a valid email.',
      },
    ],
    password: [
      {
        ruleTarget: 'field',
        keyProp: 'password',
        validator: (_, value) => {
          validatePassword(_, value);
        },
        message: 'The password must be at least 8 characters long.',
      },
    ],
    confirmPassword: [
      {
        ruleTarget: 'field',
        keyProp: 'confirmPassword',
        validator: (_, value) => {
          validateConfirmPassword(_, value);
        },
        message: 'The password must be at least 8 characters long.',
      },
    ],
  });

  // Creates a regex for all possible E-Mail addresses and checks if the given one fits the pattern
  const isValidEmail = (email: string) => {
    const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    return emailRegex.test(email);
  };

  formStore.setOnSubmit(onSubmit);
  formStore.setModel(dynamicValidateForm);
  formStore.setRules(rulesRef);
  formStore.setCustomRules(customRules);

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
      name="password"
      class="column"
      :no-style="true"
      :whitespace="true"
      :rules="rulesRef.password"
    >
      <a-input-password
        id="inputCreateUserPassword"
        v-model:value="dynamicValidateForm.password"
        class="inputField"
        placeholder="Password"
        :disabled="dynamicValidateForm.inputsDisabled"
        :rules="rulesRef.password"
        :visibility-toggle="false"
        style="margin: 10px 0"
      >
      </a-input-password>
    </a-form-item>
    <a-form-item
      name="confirmPassword"
      class="column"
      :no-style="true"
      :whitespace="true"
      :rules="rulesRef.confirmPassword"
    >
      <a-input-password
        id="inputCreateUserConfirmPassword"
        v-model:value="dynamicValidateForm.confirmPassword"
        class="inputField"
        placeholder="Confirm Password"
        :disabled="dynamicValidateForm.inputsDisabled"
        :rules="rulesRef.confirmPassword"
        :visibility-toggle="false"
        style="margin: 10px 0"
      >
      </a-input-password>
    </a-form-item>
  </a-form>
  <contextHolder></contextHolder>
</template>
