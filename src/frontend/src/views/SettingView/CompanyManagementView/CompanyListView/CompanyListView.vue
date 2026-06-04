<script lang="ts" setup>
  import { inject, computed, onMounted, ref, watch } from 'vue';
  import {
    companyRoutingSymbol,
    companyStoreSymbol,
  } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';
  import { useThemeToken } from '@/utils/hooks';
  import { PlusOutlined } from '@ant-design/icons-vue';

  const token = useThemeToken();

  const collapsed = ref<boolean>(false);
  const selectedKeys = ref<string[]>([]);
  const companyStore = inject(companyStoreSymbol)!;

  const { routerCompanyId, setCompanyId } = inject(companyRoutingSymbol)!;
  const { getCompanies, getIsLoadingCompanies } = storeToRefs(companyStore);

  const router = useRouter();

  const isLoading = computed(() => getIsLoadingCompanies.value);
  const companyData = computed(() => getCompanies.value);

  const selectedCompanyId = ref<string>('');
  watch(
    () => routerCompanyId.value,
    async () => {
      if (routerCompanyId.value == '') {
        if (selectedCompanyId.value != '') {
          console.log('write ');
          setCompanyId(selectedCompanyId.value);
        }
      }
      await companyStore?.fetch(Number(routerCompanyId.value));
      selectedKeys.value = [routerCompanyId.value];
    },
  );

  interface VueComponentWithEl extends HTMLElement {
    $el: HTMLElement;
  }

  // used for scrolling to the selected company on mount
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

  const clickTab = async (companyId: string) => {
    selectedCompanyId.value = companyId;
    setCompanyId(companyId);
  };

  onMounted(async () => {
    if (companyStore.getCompany?.id != undefined) {
      setCompanyId(String(companyStore.getCompany?.id));
    }
    await companyStore?.fetchAll();
    if (routerCompanyId.value) {
      await companyStore?.fetch(Number(routerCompanyId.value));
      selectedKeys.value = [routerCompanyId.value];
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
          key="create-company"
          class="create-menu-item"
          @click="router.push('/settings/company-management/create')"
        >
          <template #icon>
            <PlusOutlined />
          </template>
          <span>Create Company</span>
        </a-menu-item>
        <a-menu-item
          v-for="company in companyData"
          :key="String(company.id)"
          @click="clickTab(String(company.id))"
        >
          <span>{{ company.companyName }}</span>
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
      <!-- renders the CompanyInformationView -->
      <RouterView v-slot="{ Component }">
        <component :is="Component" @company-deleted="selectedCompanyId = ''" />
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
