<script lang="ts" setup>
  import { computed, onMounted, ref, watch } from 'vue';
  import { inject } from 'vue';
  import { userStoreSymbol } from '@/store/injectionSymbols';
  import { useUserRouting } from '@/utils/hooks';
  import { storeToRefs } from 'pinia';

  const collapsed = ref<boolean>(false);
  const selectedKeys = ref<string[]>([]);
  const userStore = inject(userStoreSymbol)!;
  const { routerUserId, setUserId } = useUserRouting();
  const { getIsLoadingUsers, getIsLoading, getUsers } = storeToRefs(userStore);

  const isLoading = computed(
    () => getIsLoadingUsers.value || getIsLoading.value,
  );
  const usersData = computed(() => getUsers.value);
  const routerUser = computed(() => routerUserId.value);
  userStore.fetchMe(); // fetch me for information

  watch(
    () => routerUser.value,
    async () => {
      await userStore?.fetchUser(routerUser.value);
      selectedKeys.value = [routerUser.value.toString()];
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
      selectedKeys.value = [routerUser.value.toString()];
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
    background: #fff;
    height: 100vh;
    overflow: auto;
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
    background-color: gray !important;
    color: white !important;
    height: 0;
  }
</style>
