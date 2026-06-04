<script lang="ts" setup>
  import { DeleteOutlined } from '@ant-design/icons-vue';
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';
  import { inject, ref } from 'vue';
  import {
    departmentRoutingSymbol,
    departmentStoreSymbol,
  } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';

  import FloatingButtonGroup from '@/components/Button/FloatingButtonGroup.vue';
  import ConfirmationDialog from '@/components/Modal/ConfirmAction.vue';
  import { useFormStore } from '@/components/Form';
  import { useThemeToken } from '@/utils/hooks';
  import { useBusinessUnitStore } from '@/store';

  const token = useThemeToken();

  const route = useRoute();

  const departmentStore = inject(departmentStoreSymbol)!;
  const { getDepartment, getIsLoadingDepartment } =
    storeToRefs(departmentStore);
  const buStore = useBusinessUnitStore();
  const department = computed(() => getDepartment.value);
  const isLoading = computed(() => getIsLoadingDepartment.value);
  const { setDepartmentId } = inject(departmentRoutingSymbol)!;

  const emit = defineEmits(['departmentDeleted']);

  const departmentNameFormStore = useFormStore('editDepartmentNameForm');
  onMounted(async () => {
    buStore.fetchAll();
  });

  const isConfirmModalOpen = ref<boolean>(false);
  const openModal = () => {
    isConfirmModalOpen.value = true;
  };
  const closeModal = () => {
    isConfirmModalOpen.value = false;
  };

  //Button for adding new Department and deleting Department
  const buttons = computed((): FloatButtonModel[] => {
    const tempButtons: FloatButtonModel[] = [
      {
        name: 'DeleteDepartmentButton',
        onClick: () => {
          openModal();
        },
        icon: DeleteOutlined,
        type: 'primary',
        specialType: 'danger',
        size: 'large',
        status: 'activated',
        tooltip: 'Click here to delete this department',
        isLink: false,
      },
    ];
    if (!department.value) tempButtons[0].status = 'deactivated';
    return tempButtons;
  });

  const deleteDepartment = async () => {
    if (!department.value) return;
    await departmentStore.delete(department.value?.id);
    emit('departmentDeleted');
    departmentStore.nullDepartment();
    setDepartmentId(null);
  };
</script>
<template>
  <ConfirmationDialog
    :is-open="isConfirmModalOpen"
    title="Delete confirm"
    message="Are you sure you want to delete this department?"
    @confirm="deleteDepartment"
    @cancel="closeModal"
    @update:is-open="isConfirmModalOpen = $event"
  />
  <div v-if="department && department.id" class="panel">
    <a-flex
      class="userInfoBox"
      :body-style="{
        height: 'fit-content',
      }"
    >
      <EditableTextField
        class="textField departmentName"
        :value="department?.departmentName ?? ''"
        :is-loading="isLoading"
        :label="'Department Name'"
        :is-editing-key="'isEditingDepartmentName'"
        :form-store="departmentNameFormStore"
        :has-edit-keys="true"
      >
        <DepartmentNameInputField
          :department-id="department?.id ?? -1"
          :form-store="departmentNameFormStore"
          :placeholder="department?.departmentName ?? ''"
          :default="department?.departmentName ?? ''"
        />
      </EditableTextField>
    </a-flex>
  </div>
  <a-skeleton
    v-else-if="isLoading"
    :description="`No Department Found for Id ${route.query.departmentId}`"
  ></a-skeleton>
  <a-empty
    v-else-if="route.query.departmentId"
    :description="`No Department Found for Id ${route.query.departmentId}`"
  ></a-empty>
  <a-empty v-else description="No Department Selected"></a-empty>
  <FloatingButtonGroup :buttons="buttons" class="floating-buttons" />
  <RouterView />
</template>

<style scoped>
  .panel {
    position: relative;
    /* Make sure the panel is a positioning context */
    min-width: 150px;
    max-height: 100vh;
    overflow-y: auto;
  }

  .ant-float-btn-group {
    height: max-content !important;
    width: max-content !important;
    position: absolute;
    right: 20px;
    bottom: 40px;
  }

  .userInfoBox {
    padding: 1em 3em;
    margin: 2em 1em;
    border-radius: 10px;
    background-color: v-bind('token.colorBgElevated');
    min-width: 450px;
    height: auto;
    flex-direction: column;
    flex-wrap: wrap;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  }

  :deep(.ant-card) {
    margin: 0.5em 0;
    background-color: v-bind('token.colorBgElevated');
  }

  .avatar {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
  }
</style>
