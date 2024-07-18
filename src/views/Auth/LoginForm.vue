<script lang="ts" setup>
  import type { FormStore } from '@/components/Form';
  import type { RulesObject } from '@/components/Form/types';
  import { defineProps, reactive, type StyleValue } from 'vue';
  import { UserOutlined, LockOutlined } from '@ant-design/icons-vue';
  import { useToken } from 'ant-design-vue/es/theme/internal';

  const { formStore, feedbackMessage } = defineProps<{
    formStore: FormStore;
    feedbackMessage?: string;
  }>();

  type LoginFormData = {
    username: string;
    password: string;
    remember: boolean;
  };

  const modelRef = reactive<LoginFormData>({
    username: '',
    password: '',
    remember: false,
  });

  const rulesRef = reactive<
    RulesObject<LoginFormData | Record<string, unknown>>
  >({
    username: [
      {
        type: 'string', // or 'username'
        required: true,
        message: 'Please enter a valid username address!',
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

  const submit = async () => {
    formStore.submit().catch(() => {});
  };

  const token = useToken()[1];

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
    @keyup.enter="submit"
  >
    <a-form-item name="username" v-bind="formStore.validateInfos.username">
      <a-input v-model:value="modelRef.username" placeholder="Username">
        <template #prefix>
          <UserOutlined />
        </template>
      </a-input>
    </a-form-item>
    <a-form-item name="password" v-bind="formStore.validateInfos.password">
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
      <a-button block type="primary" @click="submit"> Login</a-button>
      <a-space
        v-if="feedbackMessage && feedbackMessage != ''"
        :style="{ paddingTop: '10px' }"
      >
        <a-typography-text type="danger">{{
          feedbackMessage
        }}</a-typography-text>
      </a-space>
      <div :style="styles.footer">
        <a-typography-text>Don't have an account? </a-typography-text>
        <RouterLink to="/register">Register now</RouterLink>
      </div>
    </a-form-item>
  </a-form>
</template>
