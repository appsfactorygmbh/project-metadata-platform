<script lang="ts" setup>
  import { type FormSubmitType, useFormStore } from '@/components/Form';
  import { AuthLayout, LoginForm, SSOAuthButton } from '.';
  import { computed } from 'vue';
  import { useToken } from 'ant-design-vue/es/theme/internal';
  import useBreakpoint from 'ant-design-vue/es/_util/hooks/useBreakpoint';
  import { useAuth } from 'vue-auth3';
  import axios from 'axios';
  import { useRoute } from 'vue-router';
  import { msalInstance, msalService } from '@/services/msalService';
  import { BrowserAuthError } from '@azure/msal-browser';

  const auth = useAuth();
  const router = useRouter();
  const route = useRoute();
  const formStore = useFormStore('loginForm');

  const feedbackMessage = ref('');

  const onSubmit: FormSubmitType = (fields) => {
    auth
      .login({
        data: {
          email: fields.email,
          password: fields.password,
        },
        staySignedIn: fields.remember,
      })
      .then((response) => {
        callback();
        console.log(response);
      })
      .catch((error) => {
        console.error(error);
        if (axios.isAxiosError(error))
          feedbackMessage.value = error.response?.data.message || 'An error occurred';
        formStore.updateField('password', '');
      });
  };

  formStore.setOnSubmit(onSubmit);

  const token = useToken()[1];
  const screens = useBreakpoint();

  const refreshToken = computed(() => auth.token()?.split('|')[1]);

  const callback = () => {
    const redirect = (route.query.redirect as string) ?? '/';
    router.push(redirect);
  };

  onMounted(() => {
    msalInstance
      .handleRedirectPromise()
      .then((response) => {
        if (response && response.account) {
          msalInstance.setActiveAccount(response.account);
          return callback();
        }
      })
      .catch((error) => {
        console.warn('MSAL Redirect Error:', error.message);
        if (error instanceof BrowserAuthError) {
          window.history.replaceState(
            {},
            document.title,
            window.location.pathname,
          );
          msalService.getAccessTokenSilent().then((token) => {
            if (token) return callback();
          });
        }
      });
    auth?.load().then(() => {
      const authCheck = auth.check();
      if (authCheck) return callback();
      auth
        .refresh({ data: { refreshToken: refreshToken.value } })
        .then(() => {
          return callback();
        })
        .catch((error) => {
          console.debug('Silent refresh failed, user needs to login', error);
        });
    });
  });

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
      <a-typography-title :style="styles.title"> Login </a-typography-title>
      <a-typography-text :style="styles.text">
        Welcome back! Please enter your credentials to continue.
      </a-typography-text>
    </div>
    <LoginForm :form-store="formStore" :feedback-message="feedbackMessage" />
    <div :style="styles.header">
      <a-typography-text :style="styles.text"> or </a-typography-text>
    </div>
    <SSOAuthButton />
  </AuthLayout>
</template>
