<script lang="ts" setup>
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';
  import { LogoutOutlined, SettingOutlined } from '@ant-design/icons-vue';

  const router = useRouter();

  function goToSetting() {
        router.push({ name: 'Setting' });
  };

  const buttons: FloatButtonModel[] = [
    {
      name: 'SettingsButton',
      onClick: () => {
        goToSetting()
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
  <FloatingButtonGroup :buttons="buttons" class="menu" />
</template>

<style scoped>
  .menu {
    top: 20px;
  }
</style>
