<script lang="ts" setup>
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';
  import { msalService } from '@/services/msalService';
  import { authStore } from '@/store/AuthStore';
  import { useTheme } from '@/utils/hooks';
  import { LogoutOutlined, SettingOutlined } from '@ant-design/icons-vue';
  import { SunIcon, MoonIcon } from '@heroicons/vue/24/outline';
  import { useAuth } from 'vue-auth3';
  import { useRouter } from 'vue-router';

  // Router instance
  const router = useRouter();
  const auth = useAuth();
  const { toggleDark, isDark } = useTheme();

  const goToSetting = () => {
    router.push('/settings');
  };

  const buttons = computed(
    () =>
      [
        {
          name: 'SettingsButton',
          onClick: () => {
            goToSetting();
          },
          icon: SettingOutlined,
          size: 'middle',
          status: 'activated',
          tooltip: 'Click here to navigate to the settings page',
          isLink: true,
          destination: '/settings',
        },
        {
          name: 'LogoutButton',
          onClick: async () => {
            if (authStore._authMethod == 'oidc') {
              await msalService.logout();
            } else {
              auth.logout({
                makeRequest: false,
              });
            }
            window.location.reload();
          },
          icon: LogoutOutlined,
          size: 'middle',
          status: 'activated',
          tooltip: 'Click here to log out',
          isLink: false,
        },
        {
          name: 'ToggleThemeButton',
          onClick: () => {
            toggleDark();
          },
          icon: isDark.value ? SunIcon : MoonIcon,
          size: 'middle',
          status: 'activated',
          tooltip: 'Click here to toggle the theme',
          isLink: false,
        },
      ] satisfies FloatButtonModel[],
  );
</script>

<template>
  <FloatingButtonGroup :buttons="buttons" class="menu" />
</template>

<style scoped>
  .menu {
    top: 20px;
    height: max-content;
  }
</style>
