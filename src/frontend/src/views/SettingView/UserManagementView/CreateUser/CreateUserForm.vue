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
    isUniqueEmployeeNr,
  } from '@/utils/form/userValidation.ts';
  import { useTeamStore } from '@/store/TeamStore.ts';
  import { storeToRefs } from 'pinia';

  const { formStore, initialValues, userStore } = defineProps<{
    formStore: FormStore;
    initialValues: CreateUserFormData;
    userStore: UserStore;
  }>();
  const teamStore = useTeamStore();

  const { getTeams } = storeToRefs(teamStore);
  const [notificationApi, contextHolder] = notification.useNotification();

  const onSubmit: FormSubmitType = (fields) => {
    try {
      const userDef: CreateUserModel = {
        externalId: toRaw(fields).employeeNumber,
        userName: toRaw(fields).email,
        password: toRaw(fields).password === '' ? null : toRaw(fields).password,
        active: toRaw(fields).active,
        urnIetfParamsScimSchemasExtensionEnterprise20User: {
          organization: toRaw(fields).company,
        },
        urnIetfParamsScimSchemasExtensionPmpUser: {
          departments: toRaw(fields).departments,
          jobTitles: toRaw(fields).jobTitles,
          businessUnits: toRaw(fields).businessUnits,
          team: toRaw(fields).teams,
          teamSupport: toRaw(fields).teamSupport,
        },
        meta: {},
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
    if (value === '' && dynamicValidateForm.password !== '') {
      return Promise.reject(new Error('Please confirm the password.'));
    } else if (value !== dynamicValidateForm.password) {
      return Promise.reject(new Error("The passwords don't match."));
    } else {
      return Promise.resolve();
    }
  };

  const rulesRef = reactive<RulesObject<CreateUserFormData>>({
    email: [
      {
        required: true,
        message: 'Email is required.',
        trigger: ['change', 'blur'],
        type: 'string',
      },
      {
        required: true,
        message: 'Please insert a valid email.',
        validator: isValidEmail,
        trigger: ['change', 'blur'],
        type: 'string',
      },
      {
        required: true,
        message: 'Please insert an unique email.',
        validator: isUniqueEmail,
        trigger: ['change', 'blur'],
        type: 'email',
      },
    ],
    employeeNumber: [
      {
        required: true,
        message: 'Employee Number is required.',
        trigger: ['change', 'blur'],
        type: 'string',
      },
      {
        required: true,
        message: 'Please insert an unique employee number.',
        validator: isUniqueEmployeeNr,
        trigger: ['change', 'blur'],
        type: 'string',
      },
    ],
    password: [
      {
        required: true,
        message: 'Please insert at least 8 characters.',
        validator: hasEightCharacters,
        trigger: ['change', 'blur'],
        type: 'string',
      },
      {
        required: true,
        message: 'Please insert a special character.',
        validator: hasSpecialCharacter,
        trigger: ['change', 'blur'],
        type: 'string',
      },
      {
        required: true,
        message: 'Please insert a number.',
        validator: hasDigit,
        trigger: ['change', 'blur'],
        type: 'string',
      },
      {
        required: true,
        message: 'Please insert a upper case letter.',
        validator: hasUpperCaseLetter,
        trigger: ['change', 'blur'],
        type: 'string',
      },
      {
        required: true,
        message: 'Please insert a lower case letter.',
        validator: hasLowerCaseLetter,
        trigger: ['change', 'blur'],
        type: 'string',
      },
    ],
    confirmPassword: [
      {
        required: true,
        message: 'Please confirm the password.',
        validator: validateConfirmPassword,
        trigger: ['change', 'blur'],
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
      name="employeeNumber"
      class="column"
      :whitespace="false"
      :rules="rulesRef.employeeNumber"
    >
      <a-input
        id="inputCreateUserEmployeeNumber"
        v-model:value="dynamicValidateForm.employeeNumber"
        class="inputField"
        placeholder="Employee Number"
        :disabled="dynamicValidateForm.inputsDisabled"
        :rules="rulesRef.employeeNumber"
      />
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
      />
    </a-form-item>

    <a-form-item
      has-feedback
      name="active"
      class="column"
      :whitespace="false"
      :rules="[{ required: false }]"
    >
      <a-switch
        v-model:checked="dynamicValidateForm.active"
        class="custom-color-switch"
        :disabled="dynamicValidateForm.inputsDisabled"
        checked-children="Active"
        un-checked-children="Inactive"
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
      v-if="dynamicValidateForm.password != ''"
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

    <a-form-item
      has-feedback
      name="jobTitles"
      class="selectcolumntop"
      :whitespace="false"
      :rules="[{ required: false }]"
    >
      <a-select
        id="inputCreateUserJobTitles"
        v-model:value="dynamicValidateForm.jobTitles"
        mode="tags"
        placeholder="Jobtitles"
        :not-found-content="null"
        :open="false"
        :disabled="dynamicValidateForm.inputsDisabled"
      >
      </a-select>
    </a-form-item>

    <a-form-item
      has-feedback
      name="teams"
      :whitespace="false"
      :rules="[{ required: false }]"
    >
      <a-select
        id="inputCreateUserTeams"
        v-model:value="dynamicValidateForm.teams"
        mode="multiple"
        placeholder="Teams"
        :disabled="dynamicValidateForm.inputsDisabled"
      >
        <a-select-option
          v-for="team in getTeams"
          :key="team.teamName"
          :value="team.teamName"
        ></a-select-option>
      </a-select>
    </a-form-item>

    <a-form-item
      has-feedback
      name="teamSupport"
      :whitespace="false"
      :rules="[{ required: false }]"
    >
      <a-select
        id="inputCreateUserTeamSupport"
        v-model:value="dynamicValidateForm.teamSupport"
        mode="multiple"
        placeholder="TeamSupport"
        :disabled="dynamicValidateForm.inputsDisabled"
      >
        <a-select-option
          v-for="team in getTeams"
          :key="team.teamName"
          :value="team.teamName"
        ></a-select-option>
      </a-select>
    </a-form-item>

    <a-form-item
      has-feedback
      name="departments"
      :whitespace="false"
      :rules="[{ required: false }]"
    >
      <a-select
        id="inputCreateUserDepartments"
        v-model:value="dynamicValidateForm.departments"
        mode="tags"
        placeholder="Departments"
        :not-found-content="null"
        :open="false"
        :disabled="dynamicValidateForm.inputsDisabled"
      >
      </a-select>
    </a-form-item>

    <a-form-item
      has-feedback
      name="businessUnits"
      class="selectcolumnbottom"
      :whitespace="false"
      :rules="[{ required: false }]"
    >
      <a-select
        id="inputCreateUserBusinessUnits"
        v-model:value="dynamicValidateForm.businessUnits"
        mode="tags"
        placeholder="Business Units"
        :not-found-content="null"
        :open="false"
        :disabled="dynamicValidateForm.inputsDisabled"
      >
      </a-select>
    </a-form-item>

    <a-form-item
      has-feedback
      name="company"
      class="column"
      :whitespace="false"
      :rules="[{ required: false }]"
    >
      <a-input
        id="inputCreateUserCompany"
        v-model:value="dynamicValidateForm.company"
        class="inputField"
        placeholder="Company"
        :disabled="dynamicValidateForm.inputsDisabled"
      />
    </a-form-item>
  </a-form>
  <contextHolder />
</template>

<style scoped>
  .column {
    margin: 0;
  }

  .selectcolumnbottom {
    margin-bottom: 5%;
  }
  .selectcolumntop {
    margin-top: 5%;
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
