<script lang="ts" setup>
  // import { Setting } from '@/components/SettingView';
  import {
    AppstoreAddOutlined,
    BarsOutlined,
    UserOutlined,
    LeftOutlined,
  } from '@ant-design/icons-vue';
  import { ref } from 'vue';
  import { useRouter } from 'vue-router';

  // Component state using refs
  const collapsed = ref<boolean>(false);
  const selectedKeys = ref<string[]>(['1']);
  const tab = ref<string>('');

  // Router instance
  const router = useRouter();

  // Methods
  const goToMain = () => {
    router.go(-1);
  };

  const clickTab = (name: string) => {
    tab.value = name;
  };

  const placeholder = () => {
    console.log('Icon clicked');
  };
</script>

<template>
  <a-layout class="layout" style="min-height: 100vh">
    <a-layout-sider
      v-model:collapsed="collapsed"
      class="sideSlider"
      collapsible
    >
      <a-layout-header style="background: #030622; padding: 0" />
      <a-float-button class="icon" ghost @click="goToMain">
        <template #icon><LeftOutlined /> </template>
      </a-float-button>

      <div class="logo" />
      <a-menu
        v-model:selectedKeys="selectedKeys"
        class="menuItem"
        theme="dark"
        mode="inline"
      >
        <a-sub-menu key="sub1">
          <template #title>
            <span>
              <user-outlined />
              <span>User</span>
            </span>
          </template>
          <a-menu-item key="4" @click="clickTab('User')">User 1</a-menu-item>
          <a-menu-item key="5" @click="clickTab('User')">User 2</a-menu-item>
          <a-menu-item key="6" @click="clickTab('User')">User 3</a-menu-item>
        </a-sub-menu>
        <a-menu-item key="2" class="item2" @click="clickTab('Plugin Creation')">
          <AppstoreAddOutlined />
          <span>Plugin Creation</span>
        </a-menu-item>
        <a-menu-item key="3" class="item3" @click="clickTab('Global Logs')">
          <BarsOutlined />
          <span>Global Logs</span>
        </a-menu-item>
      </a-menu>
    </a-layout-sider>
    <a-layout>
      <a-layout-content class="addressBar" style="margin: 0 16px">
        <a-breadcrumb style="margin: 16px 0">
          <a-breadcrumb-item class="breadcrumbItem" @click="placeholder"
            >Setting</a-breadcrumb-item
          >
          <a-breadcrumb-item> {{ tab }} </a-breadcrumb-item>
        </a-breadcrumb>
        <div
          :style="{ padding: '24px', background: '#fff', minHeight: '650px' }"
        >
          test
        </div>
      </a-layout-content>
    </a-layout>
  </a-layout>
</template>

<style>
  #components-layout-demo-side .logo {
    height: 32px;
    margin: 16px;
    background: rgba(255, 255, 255, 0.3);
  }

  .site-layout .site-layout-background {
    background: #ffffff;
  }
  [data-theme='light'] .site-layout .site-layout-background {
    background: #ffffff;
  }

  .icon {
    left: 20px;
    top: 10px;
  }
</style>
