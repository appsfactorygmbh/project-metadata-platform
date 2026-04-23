<script lang="ts" setup>
  import { inject, computed, onMounted, ref, watch } from 'vue';
  import {
    apiTokenRoutingSymbol,
    apiTokenStoreSymbol,
  } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';
  import { useThemeToken } from '@/utils/hooks';

  const token = useThemeToken();

  const collapsed = ref<boolean>(false);
  const selectedKeys = ref<string[]>([]);
  const apiTokenStore = inject(apiTokenStoreSymbol)!;

  const { routerApiTokenId, setApiTokenId } = inject(apiTokenRoutingSymbol)!;
  const { getApiTokens, getIsLoadingApiTokens } = storeToRefs(apiTokenStore);

  const isLoading = computed(() => getIsLoadingApiTokens.value);
  const apiTokenData = computed(() => getApiTokens.value);

  const selectedApiTokenId = ref<string>('');
  watch(
    () => routerApiTokenId.value,
    async () => {
      if (routerApiTokenId.value == '') {
        if (selectedApiTokenId.value != '') {
          console.log('write ');
          setApiTokenId(selectedApiTokenId.value);
        }
      }
      await apiTokenStore?.fetchApiToken(Number(routerApiTokenId.value));
      selectedKeys.value = [routerApiTokenId.value];
    },
  );

  interface VueComponentWithEl extends HTMLElement {
    $el: HTMLElement;
  }

  // used for scrolling to the selected team on mount
  const siderRef = ref<VueComponentWithEl | null>(null);

  const scrollToSelectedMenuItem = async () => {
    await nextTick();
    if (siderRef.value && selectedKeys.value && selectedKeys.value.length > 0) {
      const siderElement = siderRef.value.$el || siderRef.value;

      const selectedItemElement = siderElement.querySelector(
        '.ant-menu-item-selected',
      ) as HTMLElement;

      if (selectedItemElement) {
        selectedItemElement.scrollIntoView({
          behavior: 'smooth',
          block: 'nearest',
        });
      }
    }
  };

  const clickTab = async (apiTokenId: string) => {
    selectedApiTokenId.value = apiTokenId;
    setApiTokenId(apiTokenId);
  };

  onMounted(async () => {
    if (apiTokenStore.getApiToken?.id != undefined) {
      setApiTokenId(String(apiTokenStore.getApiToken?.id));
    }
    await apiTokenStore?.fetchAll();
    if (routerApiTokenId.value) {
      await apiTokenStore?.fetchApiToken(Number(routerApiTokenId.value));
      selectedKeys.value = [routerApiTokenId.value];
      scrollToSelectedMenuItem();
    }
  });
</script>

<template>
  <a-layout class="layout">
    <a-layout-sider
      ref="siderRef"
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
          v-for="apiToken in apiTokenData"
          :key="String(apiToken.id)"
          @click="clickTab(String(apiToken.id))"
        >
          <span>{{ apiToken.name }}</span>
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
      <!-- renders the TeamInformationView -->
      <RouterView v-slot="{ Component }">
        <component :is="Component" @team-deleted="selectedApiTokenId = ''" />
      </RouterView>
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
