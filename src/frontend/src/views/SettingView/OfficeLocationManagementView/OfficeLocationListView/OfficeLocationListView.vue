<script lang="ts" setup>
  import { inject, computed, onMounted, ref, watch } from 'vue';
  import {
    officeLocationRoutingSymbol,
    officeLocationStoreSymbol,
  } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';
  import { useThemeToken } from '@/utils/hooks';
  import { PlusOutlined } from '@ant-design/icons-vue';
  import { ResourceActions } from '@/models/utils';
  import { Empty } from 'ant-design-vue';
  import { useInfiniteScroll } from '@vueuse/core';

  const token = useThemeToken();

  const collapsed = ref<boolean>(false);
  const selectedKeys = ref<string[]>([]);
  const officeLocationStore = inject(officeLocationStoreSymbol)!;

  const { routerOfficeLocationId, setOfficeLocationId } = inject(
    officeLocationRoutingSymbol,
  )!;
  const {
    getOfficeLocations,
    getIsLoadingOfficeLocations,
    getCursor,
    getIsFetchingMore,
  } = storeToRefs(officeLocationStore);

  const router = useRouter();

  const isLoading = computed(() => getIsLoadingOfficeLocations.value);
  const officeLocationData = computed(() => getOfficeLocations.value);

  const selectedOfficeLocationId = ref<string>('');
  watch(
    () => routerOfficeLocationId.value,
    async () => {
      if (routerOfficeLocationId.value == '') {
        if (selectedOfficeLocationId.value != '') {
          console.log('write ');
          setOfficeLocationId(selectedOfficeLocationId.value);
        }
      } else {
        try {
          await officeLocationStore?.fetch(
            Number(routerOfficeLocationId.value),
          );
          selectedKeys.value = [routerOfficeLocationId.value];
        } catch (error) {
          if ((error as Error).message === 'This action is unauthorized.') {
            router.push('/403');
          } else {
            console.error('Failed to fetch Office Location:', error);
          }
        }
      }
    },
  );

  interface VueComponentWithEl extends HTMLElement {
    $el: HTMLElement;
  }

  // used for scrolling to the selected officeLocation on mount
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

  const clickTab = async (officeLocationId: string) => {
    selectedOfficeLocationId.value = officeLocationId;
    setOfficeLocationId(officeLocationId);
  };

  const loadMore = async () => {
    await officeLocationStore?.fetchAll(false);
  };

  useInfiniteScroll(
    siderRef,
    async () => {
      if (getCursor.value && !getIsFetchingMore.value) {
        await loadMore();
      }
    },
    {
      distance: 150,
    },
  );
  onMounted(async () => {
    if (officeLocationStore.getOfficeLocation?.id != undefined) {
      setOfficeLocationId(String(officeLocationStore.getOfficeLocation?.id));
    }
    await officeLocationStore?.fetchAll(true);
    if (routerOfficeLocationId.value) {
      try {
        await officeLocationStore?.fetch(Number(routerOfficeLocationId.value));
        selectedKeys.value = [routerOfficeLocationId.value];
        scrollToSelectedMenuItem();
      } catch (error) {
        if ((error as Error).message === 'This action is unauthorized.') {
          router.push('/403');
        } else {
          console.error('Failed to fetch Office Location:', error);
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
      <template v-if="!isLoading">
        <a-empty
          v-if="
            officeLocationData.length == 0 &&
            !officeLocationStore.getPermissions.includes(ResourceActions.Create)
          "
          :image="Empty.PRESENTED_IMAGE_SIMPLE"
          class="empty-state"
        />
        <a-menu
          v-model:selected-keys="selectedKeys"
          mode="inline"
          class="menuItem"
        >
          <a-menu-item
            v-if="
              officeLocationStore.getPermissions.includes(
                ResourceActions.Create,
              )
            "
            key="create-officeLocation"
            class="create-menu-item"
            @click="router.push('/settings/office-location-management/create')"
          >
            <template #icon>
              <PlusOutlined />
            </template>
            <span>Create Office Location</span>
          </a-menu-item>
          <a-menu-item
            v-for="officeLocation in officeLocationData"
            :key="String(officeLocation.id)"
            :title="officeLocation.officeLocationName"
            @click="clickTab(String(officeLocation.id))"
          >
            <span>{{ officeLocation.officeLocationName }}</span>
          </a-menu-item>
        </a-menu>
        <div v-if="getIsFetchingMore" class="loading-spinner-container">
          <a-spin size="small" />
        </div>
      </template>
      <a-skeleton
        v-else
        active
        :paragraph="false"
        style="margin-left: 1em; width: 15em"
      />
    </a-layout-sider>
    <a-layout-content>
      <!-- renders the OfficeLocationInformationView -->
      <RouterView v-slot="{ Component }">
        <component
          :is="Component"
          @office-location-deleted="selectedOfficeLocationId = ''"
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
  .loading-spinner-container {
    display: flex;
    justify-content: center;
    align-items: center;
    padding: 16px 0;
    background-color: v-bind('token.colorBgElevated');
  }
</style>
