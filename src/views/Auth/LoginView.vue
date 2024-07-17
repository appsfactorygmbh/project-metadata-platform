<script lang="ts" setup>
  import { useFormStore, type FormSubmitType } from '@/components/Form';
  import { AuthLayout, LoginForm } from '.';
  import { computed } from 'vue';
  import { useToken } from 'ant-design-vue/es/theme/internal';
  import useBreakpoint from 'ant-design-vue/es/_util/hooks/useBreakpoint';
  import { useAuth } from 'vue-auth3';

  const auth = useAuth();

  const formStore = useFormStore('loginForm');

  const onSubmit: FormSubmitType = (fields) => {
    console.log(fields);
    auth
      .login({
        data: {
          username: fields.username,
          password: fields.password,
        },
        staySignedIn: fields.remember,
      })
      .then((response) => {
        console.log(response);
      })
      .catch((error) => {
        console.error(error);
        formStore.updateField('password', '');
      });
  };

  formStore.setOnSubmit(onSubmit);

  const token = useToken()[1];
  const screens = useBreakpoint();

  const styles = computed(() => ({
    header: {
      marginBottom: `${token.value.marginXL}px`,
    },
    text: {
      color: token.value.colorTextSecondary,
    },
    title: {
      fontSize: screens.value.md
        ? token.value.fontSizeHeading2
        : token.value.fontSizeHeading3,
    },
  }));
</script>

<template>
  <AuthLayout>
    <div :style="styles.header">
      <a-typography-title :style="styles.title">Login</a-typography-title>
      <a-typography-text :style="styles.text">
        Welcome back! Please enter your credentials to continue.
      </a-typography-text>
    </div>
    <LoginForm :form-store="formStore" />
  </AuthLayout>
</template>
