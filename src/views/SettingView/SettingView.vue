<script lang="ts" setup>
  //import icons for navigation buttons
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

  // Method to go back to the mainpage
  const goToMain = () => {
    router.push('/');
  };

  //Methods for URL link  by clickin the navigation buttons
  const clickTab = (name: string) => {
    tab.value = name;
    switch (name) {
      case 'User': {
        router.push(`/settings/users`);
        break;
      }
      case 'Plugin Creation': {
        router.push(`/settings/plugins`);
        break;
      }
      case 'Global Logs': {
        router.push(`/settings/global-logs`);
        break;
      }
      default: {
        router.push(`/settings`);
        break;
      }
    }
  };

  const placeholder = () => {
    console.log('Icon clicked');
  };
</script>

<template>
  <a-layout class="layout">
    <!-- sidebar -->
    <a-layout-sider
      v-model:collapsed="collapsed"
      class="sideSlider"
      collapsible
      :width="250"
    >
      <!-- return to homepage button-->
      <a-layout-header />
      <a-float-button class="iconBack" ghost @click="goToMain">
        <template #icon><LeftOutlined /> </template>
      </a-float-button>

      <!-- navigation elements -->
      <a-menu
        v-model:selectedKeys="selectedKeys"
        class="menuItem"
        mode="inline"
      >
        <a-sub-menu key="sub1">
          <template #title>
            <span>
              <user-outlined class="icons" />
              <span>User</span>
            </span>
          </template>
          <a-menu-item key="4" @click="clickTab('User')">User 1</a-menu-item>
          <a-menu-item key="5" @click="clickTab('User')">User 2</a-menu-item>
          <a-menu-item key="6" @click="clickTab('User')">User 3</a-menu-item>
        </a-sub-menu>
        <a-menu-item key="2" class="item2" @click="clickTab('Plugin Creation')">
          <AppstoreAddOutlined class="icons" />
          <span>Plugin Creation</span>
        </a-menu-item>
        <a-menu-item key="3" class="item3" @click="clickTab('Global Logs')">
          <BarsOutlined class="icons" />
          <span>Global Logs</span>
        </a-menu-item>
      </a-menu>
    </a-layout-sider>
    <a-layout class="addressBar" style="padding: 0 24px 24px">
      <a-layout-content>
        <!-- breadcrumbs -->
        <a-breadcrumb>
          <a-breadcrumb-item @click="placeholder">Setting</a-breadcrumb-item>
          <a-breadcrumb-item> {{ tab }} </a-breadcrumb-item>
        </a-breadcrumb>
        <div style="padding: 24px; min-height: 650px">
          <!-- <RouterView /> -->
        </div>
      </a-layout-content>
    </a-layout>
  </a-layout>
</template>

<style scoped>
  .iconBack {
    left: 20px;
    top: 10px;
  }
  /* Style for the sidebar icons */
  .icons * {
    width: 1.4em;
    height: 1.4em;
  }

  .layout {
    min-height: 100vh;
  }

  .ant-layout-header {
    background: #fff;
    padding: 0;
  }

  .ant-layout-sider {
    background: #fff;
  }
  /* Style for the sidebar menu */
  :deep(.ant-menu-item) {
    font-size: 1.2em;
  }

  span {
    font-size: 1.2em;
  }

  .ant-layout-content {
    margin: 0 16px;
  }

  .ant-breadcrumb {
    margin: 16px 0;
  }

  .addressBar {
    padding: 0 24px 24px;
  }

  /* Style for the expandable button on bottom*/
  :deep(.ant-layout-sider-trigger) {
    background-color: gray !important;
    color: white !important;
  }
</style>
