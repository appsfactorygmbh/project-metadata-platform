<script lang="ts" setup>
  import type { FormStore } from '@/components/Form';
  import type { RulesObject } from '@/components/Form/types';
  import { defineProps, reactive, ref, type StyleValue } from 'vue';
  import MailOutlined from '@ant-design/icons-vue/MailOutlined';
  import LockOutlined from '@ant-design/icons-vue/LockOutlined';
  import { useToken } from 'ant-design-vue/es/theme/internal';

  const { formStore } = defineProps<{
    formStore: FormStore;
  }>();

  type LoginFormData = {
    email: string;
    password: string;
    remember: boolean;
  };

  const modelRef = reactive<LoginFormData>({
    email: '',
    password: '',
    remember: false,
  });

  const rulesRef = reactive<
    RulesObject<LoginFormData | Record<string, unknown>>
  >({
    email: [
      {
        type: 'string', // or 'email'
        required: true,
        message: 'Please enter a valid email address!',
        trigger: 'change',
      },
    ],
    password: [
      {
        required: true,
        message: 'Please enter your password!',
        type: 'string',
        trigger: 'change',
      },
    ],
  });

  formStore.setModel(modelRef);
  formStore.setRules(rulesRef);

  const token = useToken()[1];
  const loginError = ref(null);

  console.log(token);

  const styles = reactive<Record<string, StyleValue>>({
    footer: {
      marginTop: `${token.value.marginLG}px`,
      textAlign: 'center',
      width: '100%',
    },
    forgotPassword: {
      float: 'right',
    },
  });
</script>

<template>
  <a-form
    name="standard_login"
    :model="modelRef"
    :initial-values="{ remember: true }"
    layout="vertical"
    required-mark="optional"
  >
    <a-form-item
      name="username"
      v-bind="formStore.validateInfos.email"
      :rules="[
        {
          type: 'email',
          required: true,
          message: 'Please enter a valid email address!',
        },
      ]"
    >
      <a-input v-model:value="modelRef.email" placeholder="E-Mail">
        <template #prefix>
          <MailOutlined />
        </template>
      </a-input>
    </a-form-item>
    <a-form-item
      name="password"
      v-bind="formStore.validateInfos.password"
      :rules="[
        {
          required: true,
          message: 'Please enter your password!',
        },
      ]"
    >
      <a-input-password
        v-model:value="modelRef.password"
        type="password"
        placeholder="Password"
      >
        <template #prefix>
          <LockOutlined />
        </template>
      </a-input-password>
    </a-form-item>
    <a-form-item>
      <a-form-item name="remember" value-prop-name="checked" no-style>
        <a-checkbox v-model:checked="modelRef.remember"
          >Stay logged in</a-checkbox
        >
      </a-form-item>
      <RouterLink :style="styles.forgotPassword" to="/forgot-password">
        Forgot password?
      </RouterLink>
    </a-form-item>
    <a-form-item :style="{ marginBottom: '0px' }">
      <a-button
        block
        type="primary"
        @click="() => formStore.submit().catch(() => {})"
      >
        Login</a-button
      >
      <a-space v-if="loginError" :style="{ paddingTop: '10px' }">
        <a-typography-text type="danger">{{ loginError }}</a-typography-text>
      </a-space>
      <div :style="styles.footer">
        <a-typography-text>Don't have an account? </a-typography-text>
        <RouterLink to="">Register now</RouterLink>
      </div>
    </a-form-item>
  </a-form>
</template>
