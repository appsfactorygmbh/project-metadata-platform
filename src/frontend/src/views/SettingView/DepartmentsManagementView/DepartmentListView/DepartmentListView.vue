<script lang="ts" setup>
  import { inject, computed, onMounted, ref, watch } from 'vue';
  import {
    departmentRoutingSymbol,
    departmentStoreSymbol,
  } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';
  import { useThemeToken } from '@/utils/hooks';
  import { PlusOutlined } from '@ant-design/icons-vue';
  import { ResourceActions } from '@/models/utils';

  const token = useThemeToken();

  const collapsed = ref<boolean>(false);
  const selectedKeys = ref<string[]>([]);
  const departmentStore = inject(departmentStoreSymbol)!;

  const { routerDepartmentId, setDepartmentId } = inject(
    departmentRoutingSymbol,
  )!;
  const { getDepartments, getIsLoadingDepartments } =
    storeToRefs(departmentStore);

  const router = useRouter();

  const isLoading = computed(() => getIsLoadingDepartments.value);
  const departmentData = computed(() => getDepartments.value);

  const selectedDepartmentId = ref<string>('');
  watch(
    () => routerDepartmentId.value,
    async () => {
      if (routerDepartmentId.value == '') {
        if (selectedDepartmentId.value != '') {
          setDepartmentId(selectedDepartmentId.value);
        }
      } else {
        try {
          await departmentStore?.fetch(Number(routerDepartmentId.value));
          selectedKeys.value = [routerDepartmentId.value];
        } catch (error) {
          if ((error as Error).message === 'This action is unauthorized.') {
            router.push('/403');
          } else {
            console.error('Failed to fetch Department:', error);
          }
        }
      }
    },
  );

  interface VueComponentWithEl extends HTMLElement {
    $el: HTMLElement;
  }

  // used for scrolling to the selected department on mount
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

  const clickTab = async (departmentId: string) => {
    selectedDepartmentId.value = departmentId;
    setDepartmentId(departmentId);
  };

  onMounted(async () => {
    if (departmentStore.getDepartment?.id != undefined) {
      setDepartmentId(String(departmentStore.getDepartment?.id));
    }
    await departmentStore?.fetchAll();
    if (routerDepartmentId.value) {
      try {
        await departmentStore?.fetch(Number(routerDepartmentId.value));
        selectedKeys.value = [routerDepartmentId.value];
        scrollToSelectedMenuItem();
      } catch (error) {
        if ((error as Error).message === 'This action is unauthorized.') {
          router.push('/403');
        } else {
          console.error('Failed to fetch Department:', error);
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
          v-if="departmentStore.getPermissions.includes(ResourceActions.Create)"
          key="create-department"
          class="create-menu-item"
          @click="router.push('/settings/department-management/create')"
        >
          <template #icon>
            <PlusOutlined />
          </template>
          <span>Create Department</span>
        </a-menu-item>
        <a-menu-item
          v-for="department in departmentData"
          :key="String(department.id)"
          @click="clickTab(String(department.id))"
        >
          <span>{{ department.departmentName }}</span>
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
      <!-- renders the DepartmentInformationView -->
      <RouterView v-slot="{ Component }">
        <component
          :is="Component"
          @department-deleted="selectedDepartmentId = ''"
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
