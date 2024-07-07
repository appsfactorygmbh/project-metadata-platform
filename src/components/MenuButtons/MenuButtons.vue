<script lang="ts" setup>
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';
  import { LogoutOutlined, SettingOutlined } from '@ant-design/icons-vue';

  const buttons: FloatButtonModel[] = [
    {
      name: 'SettingsButton',
      onClick: () => {
        console.log('Settings clicked');
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

<script lang="ts">
  export default {
    methods: {
      goToSetting() {
        this.$router.push({ name: 'settings' });
      },
    },
  };
</script>

<style lang="scss" scoped>
  .menu {
    top: 20px;
  }
</style>
