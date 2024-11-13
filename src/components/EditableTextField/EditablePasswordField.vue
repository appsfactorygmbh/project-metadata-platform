<script lang="ts" setup>
  import { useEditing } from '@/utils/hooks/useEditing';
  import {
    CheckOutlined,
    CloseOutlined,
    EditOutlined,
  } from '@ant-design/icons-vue';
  import { defineEmits, defineProps, ref } from 'vue';
  import { useFormStore } from '@/components/Form';
  import { reactive } from 'vue';
  import type { PropType } from 'vue';
  import type { EditPasswordFormData } from './EditPasswordFormData';
  import type { RulesObject } from '../Form/types';
  import type { Rule } from 'ant-design-vue/es/form';

  const formStore = useFormStore('createUserForm');

  const emit = defineEmits(['update']);

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

  const saveEdits = () => {
    stopEditing();
    emit('update', fieldValue.value);
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
  // formStore.setOnSubmit()
  formStore.setModel(dynamicValidateForm);
  formStore.setRules(rulesRef);
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
      <a-form v-else ref="formRef" :model="dynamicValidateForm">
        <a-form-item
          name="currentPassword"
          class="formItem"
          :whitespace="true"
          :rules="rulesRef.currentPassword"
        >
          <a-input
            id="inputCreateUserName"
            v-model:value="dynamicValidateForm.currentPassword"
            placeholder="Name"
            :rules="rulesRef.currentPassword"
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
            id="inputCreateUserName"
            v-model:value="dynamicValidateForm.newPassword"
            placeholder="Name"
            :rules="rulesRef.newPassword"
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
            id="inputCreateUserName"
            v-model:value="dynamicValidateForm.confirmPassword"
            placeholder="Name"
            :rules="rulesRef.confirmPassword"
          >
          </a-input>
        </a-form-item>
      </a-form>

      <a-button v-if="!isEditing" class="edit" @click="startEditing">
        <EditOutlined class="icon" />
      </a-button>
      <div v-else class="buttonGroup">
        <a-button class="edit check" @click="saveEdits">
          <CheckOutlined class="icon" />
        </a-button>
        <a-button class="edit abort" @click="stopEditing">
          <CloseOutlined class="icon" />
        </a-button>
      </div>
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
    background-color: rgba(0, 0, 0, 0.88);
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

  .check {
    background-color: #24c223;
  }

  .abort {
    background-color: #ff002e;
  }

  .buttonGroup {
    display: flex;
    flex-direction: row;
    margin-left: auto;
    margin-top: 0;
    margin-bottom: 0;
    gap: 10px;
  }

  .icon {
    color: white;
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
  }

  .formItem {
    margin-bottom: 1em;
  }

  .lastFormItem {
    margin: 0;
  }
</style>
