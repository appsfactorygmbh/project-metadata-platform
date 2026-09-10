<script lang="ts" setup>
  import { inject, computed, onMounted, ref, watch } from 'vue';
  import {
    globalBillingRoutingSymbol,
    globalBillingStoreSymbol,
  } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';
  import { useThemeToken } from '@/utils/hooks';
  import { PlusOutlined } from '@ant-design/icons-vue';
  import { ResourceActions } from '@/models/utils';

  const token = useThemeToken();

  const collapsed = ref<boolean>(false);
  const selectedKeys = ref<string[]>([]);
  const globalBillingStore = inject(globalBillingStoreSymbol)!;

  const { routerGlobalBillingId, setGlobalBillingId } = inject(
    globalBillingRoutingSymbol,
  )!;
  const { getGlobalBillingList, getIsLoadingGlobalBillingList } =
    storeToRefs(globalBillingStore);

  const router = useRouter();

  const isLoading = computed(() => getIsLoadingGlobalBillingList.value);
  const globalBillingData = computed(() => getGlobalBillingList.value);

  const selectedGlobalBillingId = ref<string>('');
  watch(
    () => routerGlobalBillingId.value,
    async () => {
      if (routerGlobalBillingId.value == '') {
        if (selectedGlobalBillingId.value != '') {
          setGlobalBillingId(selectedGlobalBillingId.value);
        }
      } else {
        try {
          await globalBillingStore?.fetch(Number(routerGlobalBillingId.value));
          selectedKeys.value = [routerGlobalBillingId.value];
        } catch (error) {
          if ((error as Error).message === 'This action is unauthorized.') {
            router.push('/403');
          } else {
            console.error('Failed to fetch Global Billing:', error);
          }
        }
      }
    },
  );

  interface VueComponentWithEl extends HTMLElement {
    $el: HTMLElement;
  }

  // used for scrolling to the selected globalBilling on mount
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

  const clickTab = async (globalBillingId: string) => {
    selectedGlobalBillingId.value = globalBillingId;
    setGlobalBillingId(globalBillingId);
  };

  onMounted(async () => {
    if (globalBillingStore.getGlobalBilling?.id != undefined) {
      setGlobalBillingId(String(globalBillingStore.getGlobalBilling?.id));
    }
    await globalBillingStore?.fetchAll();
    if (routerGlobalBillingId.value) {
      try {
        await globalBillingStore?.fetch(Number(routerGlobalBillingId.value));
        selectedKeys.value = [routerGlobalBillingId.value];
        scrollToSelectedMenuItem();
      } catch (error) {
        if ((error as Error).message === 'This action is unauthorized.') {
          router.push('/403');
        } else {
          console.error('Failed to fetch Global Billing:', error);
        }
      }
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
          v-if="
            globalBillingStore.getPermissions.includes(ResourceActions.Create)
          "
          key="create-globalBilling"
          class="create-menu-item"
          @click="router.push('/settings/global-billing-management/create')"
        >
          <template #icon>
            <PlusOutlined />
          </template>
          <span>Create GlobalBilling</span>
        </a-menu-item>
        <a-menu-item
          v-for="globalBilling in globalBillingData"
          :key="String(globalBilling.id)"
          @click="clickTab(String(globalBilling.id))"
        >
          <span>{{ globalBilling.billingKind }}</span>
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
      <!-- renders the GlobalBillingInformationView -->
      <RouterView v-slot="{ Component }">
        <component
          :is="Component"
          @global-billing-deleted="selectedGlobalBillingId = ''"
        />
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
