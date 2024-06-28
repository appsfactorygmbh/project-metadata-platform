<script lang="ts" setup>
  import {
    UserOutlined,
    LogoutOutlined,
    BarsOutlined,
    AppstoreAddOutlined,
  } from '@ant-design/icons-vue';

  // Place holder for the buttons for now
  const placeHolder = () => {
    console.log('Icon clicked');
  };
</script>

<template>
  <!-- add icons for profile, plugins, global logs, signout -->
  <a-float-button-group class="menu">
    <a-float-button class="button" ghost @click="placeHolder">
      <template #icon><UserOutlined class="icon" /> </template>
    </a-float-button>
    <a-float-button class="button" ghost @click="goToSetting">
      <template #icon><AppstoreAddOutlined class="icon" /> </template>
    </a-float-button>
    <a-float-button class="button" ghost @click="placeHolder">
      <template #icon><BarsOutlined class="icon" /> </template>
    </a-float-button>
    <a-float-button class="button" ghost @click="placeHolder">
      <template #icon><LogoutOutlined class="icon" /> </template>
    </a-float-button>
  </a-float-button-group>
</template>

<script lang="ts">
  export default {
    methods: {
      goToSetting() {
        this.$router.push({ name: 'Setting' });
      },
    },
  };
</script>

<style lang="scss" scoped>
  .menu {
    top: 20px;
  }
</style>
