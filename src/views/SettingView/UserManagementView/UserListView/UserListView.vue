<script lang="ts" setup>
  import { inject, computed, onMounted, ref, watch } from 'vue';
  import { userRoutingSymbol, userStoreSymbol } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';
  import { useThemeToken } from '@/utils/hooks';

  const token = useThemeToken();

  const collapsed = ref<boolean>(false);
  const selectedKeys = ref<string[]>([]);
  const userStore = inject(userStoreSymbol)!;
  const { routerUserId, setUserId } = inject(userRoutingSymbol)!;
  const { getIsLoading, getUsers } = storeToRefs(userStore);

  const isLoading = computed(() => getIsLoading.value);
  const usersData = computed(() => getUsers.value);

  userStore.fetchMe(); // fetch me for information

  watch(
    () => routerUserId.value,
    async () => {
      await userStore?.fetchUser(routerUserId.value);
      selectedKeys.value = [routerUserId.value];
    },
  );

  const clickTab = async (userID: string) => {
    setUserId(userID);
  };

  const getNameFromEmail = (email: string) => email.split('@')[0];

  onMounted(async () => {
    await userStore?.fetchAll();
    if (routerUserId.value === 'undefined') {
      setUserId(userStore?.getUsers[0]?.id ?? '0');
    } else {
      await userStore?.fetchUser(routerUserId.value);
      selectedKeys.value = [routerUserId.value];
    }
  });
</script>

<template>
  <a-layout class="layout">
    <a-layout-sider
      v-model:collapsed="collapsed"
      class="sideSlider"
      collapsible
      :width="250"
    >
      <a-menu
        v-if="!isLoading"
        v-model:selected-keys="selectedKeys"
        mode="inline"
        class="menuItem"
      >
        <a-menu-item
          v-for="user in usersData"
          :key="user.id"
          @click="clickTab(user.id)"
        >
          <span>{{ getNameFromEmail(user.email) }}</span>
        </a-menu-item>
      </a-menu>
      <a-skeleton
        v-else
        active
        :paragraph="false"
        style="margin-left: 1em; width: 15em"
      />
    </a-layout-sider>
    <a-layout-content>
      <div class="content">
        <RouterView />
      </div>
    </a-layout-content>
  </a-layout>
</template>

<style scoped>
  .layout {
    height: 100vh;
  }

  .ant-layout-sider {
    background-color: v-bind('token.colorBgElevated');
    height: 90vh;
    overflow: auto;
    border-radius: 10px;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  }

  .content {
    padding: 10px;
    min-height: calc(100vh - 20px);
  }

  span {
    font-size: 1em;
  }

  .ant-layout-content {
    margin: 0 16px;
  }

  :deep(.ant-layout-sider-trigger) {
    background-color: v-bind('token.colorBgElevated');
    color: white !important;
    height: 0;
  }
  .menuItem {
    background-color: v-bind('token.colorBgElevated');
  }
</style>
