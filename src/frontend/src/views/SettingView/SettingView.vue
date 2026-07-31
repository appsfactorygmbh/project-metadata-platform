<script lang="ts" setup>
  //import icons for navigation buttons
  import type { FloatButtonModel } from '@/components/Button';
  import {
    AppstoreAddOutlined,
    BarsOutlined,
    LeftOutlined,
    TeamOutlined,
    UserOutlined,
    RobotOutlined,
    HomeOutlined,
    GoldOutlined,
    EnvironmentOutlined,
    PicRightOutlined,
  } from '@ant-design/icons-vue';
  import { onMounted, ref } from 'vue';
  import { useRouter } from 'vue-router';
  import { useThemeToken } from '@/utils/hooks';

  const token = useThemeToken();

  // Component state using refs
  const collapsed = ref<boolean>(false);
  const selectedKeys = ref<string[]>([]);
  const tab = ref<string>('');

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
    isLink: false,
    status: 'activated',
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
      case 'Team': {
        router.push(`/settings/team-management`);
        break;
      }
      case 'Department': {
        router.push(`/settings/department-management`);
        break;
      }
      case 'Business Unit': {
        router.push(`/settings/business-unit-management`);
        break;
      }
      case 'Office Location': {
        router.push(`/settings/office-location-management`);
        break;
      }
      case 'Company': {
        router.push(`/settings/company-management`);
        break;
      }
      case 'Global Logs': {
        router.push(`/settings/global-logs`);
        break;
      }
      case 'API-Token': {
        router.push(`/settings/api-token-management`);
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
        case '/settings/team-management': {
          tab.value = 'Team';
          selectedKeys.value = ['2'];
          break;
        }
        case '/settings/department-management': {
          tab.value = 'Department';
          selectedKeys.value = ['3'];
          break;
        }
        case '/settings/business-unit-management': {
          tab.value = 'Business Unit';
          selectedKeys.value = ['4'];
          break;
        }
        case '/settings/office-location-management': {
          tab.value = 'Office Location';
          selectedKeys.value = ['5'];
          break;
        }
        case '/settings/company-management': {
          tab.value = 'Company';
          selectedKeys.value = ['6'];
          break;
        }
        case '/settings/api-token-management': {
          tab.value = 'API-Token';
          selectedKeys.value = ['7'];
          break;
        }
        case '/settings/global-plugins': {
          tab.value = 'Global Plugins';
          selectedKeys.value = ['8'];
          break;
        }
        case '/settings/global-logs': {
          tab.value = 'Global Logs';
          selectedKeys.value = ['9'];
          break;
        }
        case '/settings/user-management':
        default: {
          tab.value = 'User';
          selectedKeys.value = ['1'];
          break;
        }
      }
    }
  });

  const version = __APP_VERSION__;
</script>

<template>
  <a-layout class="layout">
    <!-- sidebar -->
    <a-layout-sider
      v-model:collapsed="collapsed"
      class="sideSlider"
      collapsible
      :width="318"
    >
      <!-- return to homepage button-->
      <div class="listHeader">
        <FloatingButtonGroup
          class="backButton"
          :buttons="[backButton]"
        ></FloatingButtonGroup>
      </div>
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
        <a-menu-item key="2" class="teamManagement" @click="clickTab('Team')">
          <TeamOutlined class="icons" />
          <span>Team Management</span>
        </a-menu-item>
        <a-menu-item
          key="3"
          class="departmentManagement"
          @click="clickTab('Department')"
        >
          <PicRightOutlined class="icons" />
          <span>Department Management</span>
        </a-menu-item>
        <a-menu-item
          key="4"
          class="businessUnitManagement"
          @click="clickTab('Business Unit')"
        >
          <GoldOutlined class="icons" />
          <span>BU Management</span>
        </a-menu-item>
        <a-menu-item
          key="5"
          class="officeLocationManagement"
          @click="clickTab('Office Location')"
        >
          <EnvironmentOutlined class="icons" />
          <span>Location Management</span>
        </a-menu-item>
        <a-menu-item
          key="6"
          class="companyManagement"
          @click="clickTab('Company')"
        >
          <HomeOutlined class="icons" />
          <span>Company Management</span>
        </a-menu-item>
        <a-menu-item
          key="7"
          class="apiTokenManagement"
          @click="clickTab('API-Token')"
        >
          <RobotOutlined class="icons" />
          <span>API-Token Management</span>
        </a-menu-item>
        <a-menu-item
          key="8"
          class="globalPlugins"
          @click="clickTab('Global Plugins')"
        >
          <AppstoreAddOutlined class="icons" />
          <span>Global Plugins</span>
        </a-menu-item>

        <a-menu-item
          key="9"
          class="globalLogs"
          @click="clickTab('Global Logs')"
        >
          <BarsOutlined class="icons" />
          <span>Global Logs</span>
        </a-menu-item>
      </a-menu>
      <footer class="app-version">v{{ version }}</footer>
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
  .ant-float-btn-group {
    position: absolute;
    left: 20px;
    top: 20px;
    z-index: 1;
    /* Ensure they're above other elements */
    height: fit-content;
    width: fit-content;
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
    background-color: v-bind('token.colorPrimary');
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

  .app-version {
    position: absolute;
    bottom: 90px;
    width: 100%;
    left: 1%;
    text-align: left;
    color: v-bind('token.colorTextSecondary');
    font-size: 0.85em;
    opacity: 0.7;
  }
</style>
