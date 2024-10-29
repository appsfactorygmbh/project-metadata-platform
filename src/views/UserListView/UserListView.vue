<script lang="ts" setup>
  import { ref } from 'vue';
  import { inject, onMounted, reactive } from 'vue';
  import { userStoreSymbol } from '@/store/injectionSymbols';
  import type { UserModel } from '@/models/User';
  import { useUserRouting } from '@/utils/hooks';
  import type { ComputedRef } from 'vue';

  // Component state using refs
  const collapsed = ref<boolean>(false);
  const selectedKeys = ref<string[]>(['']);
  const usersStore = inject(userStoreSymbol);
  const { routerUserId, setUserId } = useUserRouting();

  watch(
    () => routerUserId.value,
    async () => {
      await usersStore?.fetchUser(routerUserId.value);
    },
  );

  const clickTab = async (userID: number) => {
    setUserId(userID);
  };

  onMounted(async () => {
    await usersStore?.fetchUsers();
    const users = usersStore?.getUsers;
    if (users) addData(users);

    const data: ComputedRef<UserModel[]> = computed(
      () => usersStore?.getUsers ?? [],
    );

    watch(
      data,
      (newData) => {
        addData(newData);
      },
      { deep: true },
    );

    if (routerUserId.value === 0) {
      setUserId(usersStore?.getUsers[0]?.id ?? 0);
    } else {
      await usersStore?.fetchUser(routerUserId.value);
    }
  });

  const usersData = reactive<
    Array<{ id: number; name: string; username: string; email: string }>
  >([]);

  function addData(loadedData: UserModel[]) {
    usersData.length = 0;
    loadedData.forEach((user) => {
      usersData.push({
        id: user.id,
        name: user.name,
        username: user.username,
        email: user.email,
      });
    });
  }
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
        v-model:selected-keys="selectedKeys"
        class="menuItem"
        mode="inline"
      >
        <a-menu-item
          v-for="user in usersData"
          :key="user.username"
          :class="['user']"
          @click="clickTab(user.id)"
        >
          <span>{{ user.name }}</span>
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
