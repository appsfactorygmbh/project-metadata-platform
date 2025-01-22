<script lang="ts" setup>
  //import icons for navigation buttons
  import type { FloatButtonModel } from '@/components/Button';
  import {
    AppstoreAddOutlined,
    BarsOutlined,
    LeftOutlined,
    UserOutlined,
  } from '@ant-design/icons-vue';
  import { onMounted, ref } from 'vue';
  import { useRouter } from 'vue-router';
  import { useThemeToken } from '@/utils/hooks';

  const token = useThemeToken();

  // Component state using refs
  const collapsed = ref<boolean>(false);
  const selectedKeys = ref<string[]>([]);
  const tab = ref<string>('Global Plugins');

  // Router instance
  const router = useRouter();

  // Method to go back to the mainpage
  const goToMain = () => {
    router.push('/');
  };

  const backButton: FloatButtonModel = {
    icon: LeftOutlined,
    onClick: goToMain,
    name: 'Back',
    size: 'large',
    type: 'primary',
  };

  //Methods for URL link  by clickin the navigation buttons
  const clickTab = (name: string) => {
    tab.value = name;
    switch (name) {
      case 'User': {
        router.push(`/settings/user-management`);
        break;
      }
      case 'Global Plugins': {
        router.push(`/settings/global-plugins`);
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

  onMounted(() => {
    // set the selected tab based on the current route
    if (router.currentRoute) {
      switch (router?.currentRoute.value.path) {
        case '/settings/user-management': {
          selectedKeys.value = ['1'];
          break;
        }
        case '/settings/global-plugins': {
          selectedKeys.value = ['2'];
          break;
        }
        case '/settings/global-logs': {
          selectedKeys.value = ['3'];
          break;
        }
        default: {
          selectedKeys.value = ['2'];
          break;
        }
      }
    }
  });
</script>

<template>
  <a-layout class="layout">
    <!-- sidebar -->
    <a-layout-sider
      v-model:collapsed="collapsed"
      class="sideSlider"
      collapsible
      :width="280"
    >
      <!-- return to homepage button-->
      <a-layout-header class="listHeader" />
      <FloatingButton :button="backButton" class="iconBack" />

      <!-- navigation elements -->
      <a-menu
        v-model:selected-keys="selectedKeys"
        class="menuItem"
        mode="inline"
      >
        <a-menu-item key="1" class="userManagement" @click="clickTab('User')">
          <UserOutlined class="icons" />
          <span>User Management</span>
        </a-menu-item>
        <a-menu-item
          key="2"
          class="globalPlugins"
          @click="clickTab('Global Plugins')"
        >
          <AppstoreAddOutlined class="icons" />
          <span>Global Plugins</span>
        </a-menu-item>
        <a-menu-item
          key="3"
          class="globalLogs"
          @click="clickTab('Global Logs')"
        >
          <BarsOutlined class="icons" />
          <span>Global Logs</span>
        </a-menu-item>
      </a-menu>
    </a-layout-sider>
    <a-layout class="addressBar">
      <a-layout-content>
        <!-- breadcrumbs -->
        <a-breadcrumb>
          <a-breadcrumb-item>Settings</a-breadcrumb-item>
          <a-breadcrumb-item> {{ tab }} </a-breadcrumb-item>
        </a-breadcrumb>
        <div style="padding: 10px; min-height: 650px; height: 100vh">
          <RouterView />
        </div>
      </a-layout-content>
    </a-layout>
  </a-layout>
</template>

<style scoped>
  .iconBack {
    left: 20px;
    top: 20px;
  }
  /* Style for the sidebar icons */
  .icons * {
    width: 1.4em;
    height: 1.4em;
  }

  .layout {
    min-height: 100%;
    background-color: v-bind('token.colorFill');
  }

  /* Style for the sidebar menu */
  :deep(.ant-menu-root) {
    border-inline-end: none !important;
  }

  :deep(.ant-menu-item) {
    font-size: 1.2em;
  }

  :deep(.ant-menu-item-selected),
  :deep(.ant-menu-item):active {
    background-color: v-bind('token.colorFillContentHover');
    color: v-bind('token.colorText');
  }
  :deep(.ant-menu-item):hover {
    background-color: v-bind('token.colorFillContentHover') !important;
  }
  :deep(.ant-menu-item-selected):hover {
    background-color: v-bind('token.colorBgElevated');
  }

  span {
    font-size: 1.2em;
  }

  .ant-layout-content {
    margin: 0;
  }

  .ant-breadcrumb {
    margin: 15px 15px 5px;
  }

  .addressBar {
    padding: 0;
  }

  /* Style for the expandable button on bottom*/
  :deep(.ant-layout-sider-trigger) {
    background-color: v-bind('token.colorBgSpotlight');
  }
  .sideSlider {
    background-color: v-bind('token.colorBgElevated');
  }
  .listHeader {
    height: 80px;
    background-color: v-bind('token.colorBgElevated');
  }
  :deep(.ant-layout-content) {
    background-color: v-bind('token.colorFill');
  }
  .menuItem {
    background-color: v-bind('token.colorBgElevated');
  }
</style>
