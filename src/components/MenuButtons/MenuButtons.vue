<script lang="ts" setup>
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';
  import { LogoutOutlined, SettingOutlined } from '@ant-design/icons-vue';
  import { useCurrentUserStore } from '@/store/CurrentUserStore';
  import { useAuth } from 'vue-auth3';
  import { useRouter } from 'vue-router';

  // Router instance
  const router = useRouter();
  const currentUserStore = useCurrentUserStore();
  const auth = useAuth();

  const goToSetting = () => {
    router.push('/settings');
  };

  const buttons: FloatButtonModel[] = [
    {
      name: 'SettingsButton',
      onClick: () => {
        goToSetting();
      },
      icon: SettingOutlined,
      status: 'activated',
      tooltip: 'Click here to navigate to the settings page',
    },
    {
      name: 'LogoutButton',
      onClick: () => {
        auth.logout({
          makeRequest: false,
        });
        currentUserStore.clearUser();
      },
      icon: LogoutOutlined,
      status: 'activated',
      tooltip: 'Click here to log out',
    },
  ];
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
