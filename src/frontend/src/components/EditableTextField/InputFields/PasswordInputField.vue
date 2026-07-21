<script setup lang="ts">
  import {
    hasEightCharacters,
    hasSpecialCharacter,
    hasDigit,
    hasUpperCaseLetter,
    hasLowerCaseLetter,
  } from '@/utils/form/userValidation';
  import type { Rule } from 'ant-design-vue/es/form';

  const props = defineProps({
    value: {
      type: String,
      default: '',
    },
  });

  const emit = defineEmits(['update:value']);
  const localState = reactive({
    password: props.value,
    confirmPassword: '',
  });

  watch(
    () => props.value,
    (val) => {
      localState.password = val;
    },
  );

  const validateConfirmPassword = async (_rule: Rule, value: string) => {
    if (value === '') {
      return Promise.reject('Please confirm the password.');
    } else if (value !== localState.password) {
      return Promise.reject("The passwords don't match.");
    } else {
      return Promise.resolve();
    }
  };

  const passwordRules: Rule[] = [
    {
      validator: async (_rule, value) => {
        if (!value) return Promise.resolve();
        return Promise.resolve();
      },
      trigger: 'change',
    },
    {
      validator: hasEightCharacters,
      trigger: 'change',
      message: 'At least 8 characters required.',
    },
    {
      validator: hasSpecialCharacter,
      trigger: 'change',
      message: 'Special character required.',
    },
    { validator: hasDigit, trigger: 'change', message: 'Number required.' },
    {
      validator: hasUpperCaseLetter,
      trigger: 'change',
      message: 'Uppercase letter required.',
    },
    {
      validator: hasLowerCaseLetter,
      trigger: 'change',
      message: 'Lowercase letter required.',
    },
  ];

  const confirmRules: Rule[] = [
    {
      validator: validateConfirmPassword,
      trigger: ['change', 'blur'],
    },
  ];

  const handlePasswordChange = (val: string) => {
    localState.password = val;
    emit('update:value', val);
  };
</script>

<template>
  <div class="password-fieldset">
    <a-form-item
      has-feedback
      name="password"
      class="formItem"
      :rules="localState.password ? passwordRules : []"
    >
      <a-input
        :value="localState.password"
        type="password"
        placeholder="Enter new password"
        @update:value="handlePasswordChange"
      />
    </a-form-item>

    <a-form-item
      v-if="localState.password"
      has-feedback
      name="confirmPassword"
      class="lastFormItem"
      :rules="localState.password ? confirmRules : []"
    >
      <a-input
        v-model:value="localState.confirmPassword"
        type="password"
        placeholder="Confirm new password"
      />
    </a-form-item>
  </div>
</template>

<style scoped>
  .formItem {
    margin-bottom: 12px;
  }

  .lastFormItem {
    margin-bottom: 0;
  }
</style>
