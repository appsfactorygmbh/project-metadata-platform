<script lang="ts" setup>
  import { ref } from 'vue';
  //import { useRouter } from 'vue-router';

  // Component state using refs
  const collapsed = ref<boolean>(false);
  const selectedKeys = ref<string[]>(['2']);
  /*const tab = ref<string>('');

  // Router instance
  const router = useRouter();

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
  };*/
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
      <!-- navigation elements -->
      <a-menu
        v-model:selectedKeys="selectedKeys"
        class="menuItem"
        mode="inline"
      >
        <a-menu-item key="1" class="user">
          <span>User 1</span>
        </a-menu-item>
        <a-menu-item key="2" class="user">
          <span>User 2</span>
        </a-menu-item>
        <a-menu-item key="3" class="user">
          <span>User 3</span>
        </a-menu-item>
      </a-menu>
    </a-layout-sider>
    <a-layout-content>
      <div style="padding: 10px; min-height: 650px">
        <RouterView />
      </div>
    </a-layout-content>
  </a-layout>
</template>

<style scoped>
  .layout {
    height: 100vh;
  }

  .ant-layout-header {
    background: #fff;
    padding: 0;
  }

  .ant-layout-sider {
    background: #fff;
    height: 92vh;
    overflow: auto;
  }

  span {
    font-size: 1em;
  }

  .ant-layout-content {
    margin: 0 16px;
  }

  /* Style for the expandable button on bottom*/
  :deep(.ant-layout-sider-trigger) {
    background-color: gray !important;
    color: white !important;
    height: 0;
  }
</style>
