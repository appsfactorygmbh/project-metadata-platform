<script lang="ts" setup>
  import { inject, computed, onMounted, ref, watch } from 'vue';
  import { userRoutingSymbol, userStoreSymbol } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';
  import { useThemeToken } from '@/utils/hooks';
  import { PlusOutlined } from '@ant-design/icons-vue';
  import { ResourceActions } from '@/models/utils';
  const token = useThemeToken();

  const collapsed = ref<boolean>(false);
  const selectedKeys = ref<string[]>([]);
  const userStore = inject(userStoreSymbol)!;
  const { routerUserId, setUserId } = inject(userRoutingSymbol)!;
  const { getIsLoading, getUsers } = storeToRefs(userStore);

  const router = useRouter();

  const isLoading = computed(() => getIsLoading.value);
  const usersData = computed(() => getUsers.value);

  const selectedUserId = ref<string>('');

  watch(
    () => routerUserId.value,
    async () => {
      if (routerUserId.value == '') {
        if (selectedUserId.value != '') {
          setUserId(selectedUserId.value);
        }
      } else {
        await userStore?.fetchUser(routerUserId.value);
        selectedKeys.value = [routerUserId.value];
      }
    },
  );

  const clickTab = async (userID: string) => {
    selectedUserId.value = userID;
    setUserId(userID);
  };

  const getNameFromEmail = (email: string) => email.split('@')[0];

  onMounted(async () => {
    await userStore?.fetchAll();
    if (userStore.getUser?.externalId != undefined) {
      setUserId(userStore.getUser?.externalId);
    }
    if (routerUserId.value) {
      await userStore?.fetchUser(routerUserId.value);
    }
    selectedKeys.value = [routerUserId.value];
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
          v-if="userStore.getPermissions.includes(ResourceActions.Create)"
          key="create-user"
          class="create-menu-item"
          @click="router.push('/settings/user-management/create')"
        >
          <template #icon>
            <PlusOutlined />
          </template>
          <span>Create User</span>
        </a-menu-item>
        <a-menu-item
          v-for="user in usersData"
          :key="user.externalId"
          :title="getNameFromEmail(user.userName)"
          @click="clickTab(user.externalId)"
        >
          <div class="menu-item-content">
            <span class="user-name">{{ getNameFromEmail(user.userName) }}</span>

            <a-tag v-if="user.isScimProvisioned" color="blue" class="scim-tag">
              SCIM
            </a-tag>
          </div>
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

  .menu-item-content {
    display: flex;
    align-items: center;
    justify-content: space-between;
    width: 100%;
    overflow: hidden;
  }

  .user-name {
    overflow: hidden;
    white-space: nowrap;
    text-overflow: ellipsis;
    flex: 1;
    min-width: 0;
  }

  .scim-tag {
    flex-shrink: 0;
    margin-left: 8px;
    margin-right: 0;
    font-size: 10px;
    line-height: 16px;
    height: 18px;
  }

  :deep(.ant-menu-title-content) {
    display: flex;
    overflow: hidden;
  }

  .button {
    width: 100%;
    margin-top: 0.5%;
  }
</style>
