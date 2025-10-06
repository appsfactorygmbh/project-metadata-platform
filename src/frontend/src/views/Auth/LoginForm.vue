<script lang="ts" setup>
  import type { FormStore } from '@/components/Form';
  import type { RulesObject } from '@/components/Form/types';
  import { reactive } from 'vue';
  import { LockOutlined, UserOutlined } from '@ant-design/icons-vue';
  import { useToken } from 'ant-design-vue/es/theme/internal';

  const { formStore, feedbackMessage } = defineProps<{
    formStore: FormStore;
    feedbackMessage?: string;
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
        type: 'string',
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

  const submit = async () => {
    formStore.submit().catch(() => {});
  };

  const token = useToken()[1];

  console.log(token);
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
    <a-form-item name="email" v-bind="formStore.validateInfos.email">
      <a-input v-model:value="modelRef.email" placeholder="Email">
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
        <a-checkbox v-model:checked="modelRef.remember">
          Stay logged in
        </a-checkbox>
      </a-form-item>
    </a-form-item>
    <a-form-item :style="{ marginBottom: '0px' }">
      <a-button block type="primary" @click="submit"> Login </a-button>
      <a-space
        v-if="feedbackMessage && feedbackMessage != ''"
        :style="{ paddingTop: '10px' }"
      >
        <a-typography-text type="danger">
          {{ feedbackMessage }}
        </a-typography-text>
      </a-space>
    </a-form-item>
  </a-form>
</template>
