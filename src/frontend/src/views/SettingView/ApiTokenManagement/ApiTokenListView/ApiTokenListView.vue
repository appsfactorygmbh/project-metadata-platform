<script lang="ts" setup>
  import { inject, computed, onMounted, ref, watch } from 'vue';
  import {
    apiTokenRoutingSymbol,
    apiTokenStoreSymbol,
  } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';
  import { useThemeToken } from '@/utils/hooks';
  import { PlusOutlined } from '@ant-design/icons-vue';
  const token = useThemeToken();

  const collapsed = ref<boolean>(false);
  const selectedKeys = ref<string[]>([]);
  const apiTokenStore = inject(apiTokenStoreSymbol)!;
  const { routerApiTokenId, setApiTokenId } = inject(apiTokenRoutingSymbol)!;
  const { getIsLoading, getApiTokens } = storeToRefs(apiTokenStore);

  const router = useRouter();

  const isLoading = computed(() => getIsLoading.value);
  const tokensData = computed(() => getApiTokens.value);

  const selectedApiTokenId = ref<string>('');

  watch(
    () => routerApiTokenId.value,
    async () => {
      // if no query is present -> check if data is in store -> if so set the userId query
      if (routerApiTokenId.value == '') {
        if (selectedApiTokenId.value != '') {
          setApiTokenId(selectedApiTokenId.value);
        }
      }
      await apiTokenStore?.fetchApiToken(Number(routerApiTokenId.value));
      selectedKeys.value = [routerApiTokenId.value];
    },
  );

  const clickTab = async (apiTokenId: string) => {
    selectedApiTokenId.value = apiTokenId;
    setApiTokenId(apiTokenId);
  };

  // when mounted -> look if there is already data loaded into the store -> if so set the tokenId to the one in the store
  // this is used for when coming back to the API-Token Management tab to have the same user selected as before
  onMounted(async () => {
    if (apiTokenStore.getApiToken?.id != undefined) {
      setApiTokenId(String(apiTokenStore.getApiToken?.id));
    }
    await apiTokenStore?.fetchAll();
    await apiTokenStore?.fetchApiToken(Number(routerApiTokenId.value));
    selectedKeys.value = [routerApiTokenId.value];
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
          key="create-token"
          class="create-menu-item"
          @click="router.push('/settings/api-token-management/create')"
        >
          <template #icon>
            <PlusOutlined />
          </template>
          <span>Create API-Token</span>
        </a-menu-item>
        <a-menu-item
          v-for="ApiToken in tokensData"
          :key="ApiToken.id"
          :title="ApiToken.name"
          @click="clickTab(String(ApiToken.id))"
        >
          <div class="menu-item-content">
            <span class="user-name">{{ ApiToken.name }}</span>
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
</style>
