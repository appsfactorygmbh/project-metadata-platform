<script lang="ts" setup>
  import { useEditing } from '@/utils/hooks/useEditing';
  import { defineProps, inject, ref } from 'vue';
  import { useFormStore } from '@/components/Form';
  import { reactive, toRaw } from 'vue';
  import type { PropType } from 'vue';
  import type { EditPasswordFormData } from './EditPasswordFormData';
  import { type FormSubmitType } from '@/components/Form';
  import type { RulesObject } from '../Form/types';
  import type { Rule } from 'ant-design-vue/es/form';
  import { userStoreSymbol } from '@/store/injectionSymbols';

  const formStore = useFormStore('createUserForm');
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
        trigger: 'change',
        type: 'string',
      },
    ],
    newPassword: [
      {
        required: true,
        message:
          'Please insert a Password, which has upper/lower case letters, special characters, a digit and at least 8 characters.',
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

  const onSubmit: FormSubmitType = (fields) => {
    const newPassword = {
      password: toRaw(fields).newPassword,
    };
    userStore.patchUser(props.userId, newPassword);
  };

  const resetFields = () => {
    dynamicValidateForm.currentPassword = '';
    dynamicValidateForm.newPassword = '';
    dynamicValidateForm.confirmPassword = '';
  };

  formStore.setModel(dynamicValidateForm);
  formStore.setRules(rulesRef);
  formStore.setOnSubmit(onSubmit);

  const saveChanges = () => {
    formStore.submit();
    formStore.resetFields();
    resetFields();
    stopEditing();
  };

  const cancleEdit = () => {
    formStore.resetFields();
    resetFields();
    stopEditing();
  };
</script>

<template>
  <a-card
    :body-style="{
      display: 'flex',
      alignItems: 'center',
    }"
    class="info"
  >
    <label class="label">{{ label }}:</label>
    <template v-if="!isLoading">
      <p v-if="!isEditing" class="text">{{ value }}</p>
      <a-form v-else ref="formRef" :model="dynamicValidateForm" class="form">
        <a-form-item
          name="currentPassword"
          class="formItem"
          :whitespace="true"
          :rules="rulesRef.currentPassword"
        >
          <a-input
            v-model:value="dynamicValidateForm.currentPassword"
            placeholder="Enter your current password"
            :rules="rulesRef.currentPassword"
            type="password"
            class="input"
          >
          </a-input>
        </a-form-item>
        <a-form-item
          name="newPassword"
          class="formItem"
          :whitespace="true"
          :rules="rulesRef.newPassword"
        >
          <a-input
            v-model:value="dynamicValidateForm.newPassword"
            placeholder="Enter your new password"
            :rules="rulesRef.newPassword"
            type="password"
          >
          </a-input>
        </a-form-item>
        <a-form-item
          name="confirmPassword"
          class="lastFormItem"
          :whitespace="true"
          :rules="rulesRef.confirmPassword"
        >
          <a-input
            v-model:value="dynamicValidateForm.confirmPassword"
            placeholder="Confirm your new password"
            :rules="rulesRef.confirmPassword"
            type="password"
          >
          </a-input>
        </a-form-item>
      </a-form>

      <EditButtons
        :is-editing="isEditing"
        :safe-disabled="userStore.getIsLoadingUpdate"
        :is-loading="userStore.getIsLoadingUpdate"
        @start-editing="startEditing"
        @cancle-edit="cancleEdit"
        @safe-edits="saveChanges"
      />
    </template>
    <a-skeleton
      v-else
      active
      :paragraph="false"
      style="margin-left: 1em; width: 10em"
    />
  </a-card>
  <contextHolder></contextHolder>
</template>

<style>
  .edit {
    border: none;
    margin: 0.6em 0 0.6em;
    margin-left: auto;
    background-color: rgba(0, 0, 0, 0.88);
    width: 2em;
    height: 2em;
    display: flex;
    justify-content: center;
    align-items: center;
  }

  .input {
    margin: 0 !important;
  }
  .text {
    font-weight: 400;
  }

  .formItem {
    margin-bottom: 1em;
  }

  .lastFormItem {
    margin: 0;
  }
</style>
