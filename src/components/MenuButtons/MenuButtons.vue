<script lang="ts" setup>
  import type { ButtonModel } from '@/components/Button/ButtonModel';
  import { LogoutOutlined, SettingOutlined } from '@ant-design/icons-vue';

  const buttons: ButtonModel[] = [
    {
      name: 'SettingsButton',
      onClick: () => {
        console.log('Settings clicked');
      },
      icon: SettingOutlined,
      disabled: false,
      tooltip: 'Click here to navigate to the settings page',
    },
    {
      name: 'LogoutButton',
      onClick: () => {
        console.log('Logout clicked');
      },
      icon: LogoutOutlined,
      disabled: false,
      tooltip: 'Click here to log out',
    },
  ];
</script>

<template>
  <!-- add icons for profile, plugins, global logs, signout -->
  <FloatingButtonGroup :buttons="buttons" class="menu" />
</template>

<style lang="scss" scoped>
  .menu {
    top: 20px;
  }
</style>
