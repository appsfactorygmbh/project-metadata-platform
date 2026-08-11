<script setup lang="ts">
  import { ref, computed, inject, onMounted } from 'vue';
  import { storeToRefs } from 'pinia';
  import { authorizationStoreSymbol } from '@/store/injectionSymbols';
  import type { Key } from 'ant-design-vue/es/_util/type';
  import { useThemeToken } from '@/utils/hooks';
  import {
    CaretDownOutlined,
    CopyOutlined,
    CheckOutlined,
  } from '@ant-design/icons-vue';
  import type { FilterModel } from '@/models/Authorization';
  import { nanoid } from 'nanoid';
  const token = useThemeToken();
  const authorizationStore = inject(authorizationStoreSymbol)!;

  const {
    getPermissionFilters,
    getResources,
    getIsLoading,
    getIsLoadingResources,
  } = storeToRefs(authorizationStore);

  const resourceData = computed(() => getResources.value);
  const isLoading = computed(() => getIsLoading.value);
  const isLoadingResources = computed(() => getIsLoadingResources.value);
  const processedPermissions = computed(() => {
    if (!getPermissionFilters.value) return [];

    return getPermissionFilters.value.map((permission) => {
      const expandedKeys: string[] = [];

      const buildTree = (filter: FilterModel): FilterTreeNode => {
        const key = filter.value + '_' + nanoid();
        expandedKeys.push(key);
        return {
          title: filter.value,
          key: key,
          children: filter.children?.map((child) => buildTree(child)) ?? [],
        };
      };

      const treeData = [buildTree(permission.filter)];

      return {
        ...permission,
        treeData,
        expandedKeys,
      };
    });
  });
  const activeKey = ref<Key>();
  const copiedStates = ref<Record<string, boolean>>({});

  onMounted(async () => {
    await authorizationStore.fetchResources();

    if (resourceData.value && resourceData.value.length > 0) {
      activeKey.value = resourceData.value[0];
      await authorizationStore.fetchPermissions(activeKey.value.toString());
    }
  });

  const clickTab = async (resourceKey: Key) => {
    copiedStates.value = {};
    await authorizationStore.fetchPermissions(resourceKey.toString());
  };
  interface FilterTreeNode {
    title: string;
    key: string;
    children: FilterTreeNode[];
  }

  const copyPermissions = async (filter: FilterModel, actionId: string) => {
    const filterToString = (filter: FilterModel): string => {
      let filterString = filter.value;
      if (filter.children == null || filter.children.length == 0)
        return filterString;

      filterString =
        filterString +
        ' ( ' +
        filter.children.map((child) => filterToString(child)).join(' ') +
        ')';

      return filterString;
    };

    const filterString = filterToString(filter);
    await navigator.clipboard.writeText(filterString);
    copiedStates.value[actionId] = true;

    setTimeout(() => {
      copiedStates.value[actionId] = false;
    }, 2000);
  };
</script>

<template>
  <div class="container">
    <a-tabs
      v-if="!isLoadingResources"
      v-model:active-key="activeKey"
      class="tabs"
      @change="clickTab"
    >
      <a-tab-pane
        v-for="resource in resourceData"
        :key="resource"
        :tab="resource"
      >
        <a-spin :spinning="isLoading">
          <div class="cards-grid">
            <a-card
              v-for="permission in processedPermissions"
              :key="permission.action"
              :title="permission.action"
              class="permission-card"
            >
              <template #extra>
                <a-tooltip
                  :title="copiedStates[permission.action] ? 'Copied' : 'Copy'"
                  placement="top"
                >
                  <a-button
                    type="text"
                    class="button"
                    @click="
                      copyPermissions(permission.filter, permission.action)
                    "
                  >
                    <CheckOutlined v-if="copiedStates[permission.action]" />
                    <CopyOutlined v-else />
                  </a-button>
                </a-tooltip>
              </template>
              <a-tree
                :key="permission.treeData[0].key"
                :tree-data="permission.treeData"
                :default-expanded-keys="permission.expandedKeys"
                :selectable="false"
                :show-line="{ showLeafIcon: false }"
              >
                <template #switcherIcon="{ isLeaf }">
                  <CaretDownOutlined
                    v-if="!isLeaf"
                    class="ant-tree-switcher-icon"
                  /> </template
              ></a-tree>
            </a-card>
          </div>
        </a-spin>
      </a-tab-pane>
    </a-tabs>

    <div v-else class="skeleton-container">
      <a-skeleton
        active
        :paragraph="{ rows: 0 }"
        style="width: 300px; margin-bottom: 24px"
      />
    </div>
  </div>
</template>

<style scoped>
  .container {
    padding: 24px;
    overflow-y: auto;

    height: 100%;
    max-height: 100%;
    background-color: v-bind('token.colorBgContainer');
  }

  .cards-grid {
    display: flex;
    flex-wrap: wrap;
    gap: 20px;
    align-items: stretch;
  }

  .permission-card {
    display: flex;

    overflow-x: auto;
    flex-direction: column;
    border-radius: calc(v-bind('token.borderRadius') * 1.5px);
    box-shadow: 0 1px 4px rgba(0, 0, 0, 0.05);
    border: 1px solid v-bind('token.colorBorderSecondary');
    transition: box-shadow 0.3s ease;
    height: 100%;
  }

  .permission-card:hover {
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
  }

  .permission-card :deep(.ant-card-head) {
    min-height: 48px;
    padding: 0 20px;
    border-bottom: 1px solid v-bind('token.colorBorderSecondary');
  }

  .permission-card :deep(.ant-card-head-title) {
    font-weight: 600;
  }

  .skeleton-container {
    padding-top: 12px;
  }
</style>
