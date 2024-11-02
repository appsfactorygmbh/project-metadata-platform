<script lang="ts" setup>
  import { ref } from 'vue';
  import { inject, onMounted } from 'vue';
  import { userStoreSymbol } from '@/store/injectionSymbols';
  import { useUserRouting } from '@/utils/hooks';
  import { storeToRefs } from 'pinia';

  // Component state using refs
  const collapsed = ref<boolean>(false);
  const selectedKeys = ref<string[]>(['1']);
  const userStore = inject(userStoreSymbol)!;
  const { routerUserId, setUserId } = useUserRouting();
  const { getIsLoadingUsers, getIsLoading, getUsers } = storeToRefs(userStore);

  const isLoading = computed(
    () => getIsLoadingUsers.value || getIsLoading.value,
  );
  const usersData = computed(() => getUsers.value);
  const routerUser = computed(() => routerUserId.value);

  watch(
    () => routerUser.value,
    async () => {
      await userStore?.fetchUser(routerUser.value);
    },
  );

  const clickTab = async (userID: number) => {
    setUserId(userID);
  };

  onMounted(async () => {
    await userStore?.fetchUsers();

    if (routerUser.value === 0) {
      setUserId(userStore?.getUsers[0]?.id ?? 0);
    } else {
      await userStore?.fetchUser(routerUser.value);
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
      :width="250"
    >
      <!-- navigation elements -->
      <a-menu
        v-if="!isLoading"
        v-model:selected-keys="selectedKeys"
        class="menuItem"
        mode="inline"
      >
        <a-menu-item
          v-for="user in usersData"
          :key="user.id"
          class="users"
          @click="clickTab(user.id)"
        >
          <span>{{ user.name }}</span>
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
    height: 93vh;
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
