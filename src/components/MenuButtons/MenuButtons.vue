<script lang="ts" setup>
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';
  import { LogoutOutlined, SettingOutlined } from '@ant-design/icons-vue';
  import { useRouter } from 'vue-router';

  // Router instance
  const router = useRouter();

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
        console.log('Logout clicked');
      },
      icon: LogoutOutlined,
      status: 'activated',
      tooltip: 'Click here to log out',
    },
  ];
</script>

<template>
  <!-- add icons for profile, plugins, global logs, signout -->
  <FloatingButtonGroup :buttons="buttons" class="menu" />
</template>

<style scoped>
  .menu {
    top: 20px;
  }
</style>
