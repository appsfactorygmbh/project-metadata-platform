<script lang="ts" setup>
  import { inject, computed, onMounted, ref, watch } from 'vue';
  import {
    businessUnitRoutingSymbol,
    businessUnitStoreSymbol,
  } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';
  import { useThemeToken } from '@/utils/hooks';
  import { PlusOutlined } from '@ant-design/icons-vue';

  const token = useThemeToken();

  const collapsed = ref<boolean>(false);
  const selectedKeys = ref<string[]>([]);
  const businessUnitStore = inject(businessUnitStoreSymbol)!;

  const { routerBusinessUnitId, setBusinessUnitId } = inject(
    businessUnitRoutingSymbol,
  )!;
  const { getBusinessUnits, getIsLoadingBusinessUnits } =
    storeToRefs(businessUnitStore);

  const router = useRouter();

  const isLoading = computed(() => getIsLoadingBusinessUnits.value);
  const businessUnitData = computed(() => getBusinessUnits.value);

  const selectedBusinessUnitId = ref<string>('');
  watch(
    () => routerBusinessUnitId.value,
    async () => {
      if (routerBusinessUnitId.value == '') {
        if (selectedBusinessUnitId.value != '') {
          console.log('write ');
          setBusinessUnitId(selectedBusinessUnitId.value);
        }
      }
      await businessUnitStore?.fetch(Number(routerBusinessUnitId.value));
      selectedKeys.value = [routerBusinessUnitId.value];
    },
  );

  interface VueComponentWithEl extends HTMLElement {
    $el: HTMLElement;
  }

  // used for scrolling to the selected businessUnit on mount
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

  const clickTab = async (businessUnitId: string) => {
    selectedBusinessUnitId.value = businessUnitId;
    setBusinessUnitId(businessUnitId);
  };

  onMounted(async () => {
    if (businessUnitStore.getBusinessUnit?.id != undefined) {
      setBusinessUnitId(String(businessUnitStore.getBusinessUnit?.id));
    }
    await businessUnitStore?.fetchAll();
    if (routerBusinessUnitId.value) {
      await businessUnitStore?.fetch(Number(routerBusinessUnitId.value));
      selectedKeys.value = [routerBusinessUnitId.value];
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
          key="create-businessUnit"
          class="create-menu-item"
          @click="router.push('/settings/business-unit-management/create')"
        >
          <template #icon>
            <PlusOutlined />
          </template>
          <span>Create Business Unit</span>
        </a-menu-item>
        <a-menu-item
          v-for="businessUnit in businessUnitData"
          :key="String(businessUnit.id)"
          @click="clickTab(String(businessUnit.id))"
        >
          <span>{{ businessUnit.businessUnitName }}</span>
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
      <!-- renders the BusinessUnitInformationView -->
      <RouterView v-slot="{ Component }">
        <component
          :is="Component"
          @business-unit-deleted="selectedBusinessUnitId = ''"
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
