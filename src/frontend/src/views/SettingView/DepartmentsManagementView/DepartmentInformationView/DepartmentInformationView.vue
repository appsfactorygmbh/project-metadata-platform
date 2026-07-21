<script lang="ts" setup>
  import {
    CloseOutlined,
    DeleteOutlined,
    EditOutlined,
    SaveOutlined,
  } from '@ant-design/icons-vue';
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';
  import { inject, ref } from 'vue';
  import {
    departmentRoutingSymbol,
    departmentStoreSymbol,
  } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';

  import FloatingButtonGroup from '@/components/Button/FloatingButtonGroup.vue';
  import ConfirmationDialog from '@/components/Modal/ConfirmAction.vue';
  import { useEditing, useThemeToken } from '@/utils/hooks';
  import { ResourceActions } from '@/models/utils';
  import message from 'ant-design-vue/es/message';
  import type { Rule } from 'ant-design-vue/es/form';
  import type { DepartmentModel } from '@/models/Department';

  const token = useThemeToken();

  const route = useRoute();
  const { isEditing, stopEditing, startEditing } = useEditing();
  const formRef = ref();
  const departmentStore = inject(departmentStoreSymbol)!;
  const { getDepartment, getIsLoadingDepartment } =
    storeToRefs(departmentStore);
  const department = computed(() => getDepartment.value);
  const isLoading = computed(() => getIsLoadingDepartment.value);
  const { setDepartmentId } = inject(departmentRoutingSymbol)!;

  const emit = defineEmits(['departmentDeleted']);

  onBeforeUnmount(() => {
    if (isEditing.value) {
      stopEditing();
    }
  });
  const toggleEditingMode = async () => {
    if (isEditing.value) {
      await stopEditing();
    } else {
      await startEditing();
    }
  };

  const formData = reactive({
    departmentName: '',
  });

  watch(
    () => department.value,
    (newDepartment) => {
      if (!newDepartment) return;
      formData.departmentName = newDepartment.departmentName ?? '';
    },
  );

  const resetFormData = () => {
    const newDepartment = department.value;
    if (!newDepartment) return;
    formData.departmentName = newDepartment.departmentName ?? '';
  };

  watch(
    () => department.value?.id,
    (newId, oldId) => {
      if (!newId || newId === oldId) return;
      stopEditing();
      resetFormData();
    },
    { immediate: true },
  );
  watch(isEditing, (newVal) => {
    if (!newVal) {
      resetFormData();
    }
  });

  const handleBulkSave = async () => {
    try {
      await formRef.value.validate();
      if (!department.value?.id) return;

      const updateRequest = {
        departmentName: formData.departmentName,
      };

      await departmentStore.update(department.value.id, updateRequest);

      await departmentStore.fetch(department.value?.id);

      message.success('Department updated successfully');
      await stopEditing();
    } catch (error) {
      console.error('Validation or API error:', error);
      message.error('Failed to update department. Please check your inputs.');
    }
  };

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
      {
        name: 'EditDepartmentButton',
        onClick: () => {
          toggleEditingMode();
        },
        icon: EditOutlined,
        type: 'primary',
        size: 'large',
        status: 'activated',
        tooltip: 'Click here to edit this department',
        isLink: false,
      },
      {
        name: 'CancelButton',
        onClick: () => {
          openCancelModal();
        },
        icon: CloseOutlined,
        status: 'activated',
        type: 'primary',
        size: 'large',
        specialType: 'danger',
        tooltip: 'Click to cancel editing',
      },
      {
        name: 'SafeEditButton',
        onClick: () => {
          handleBulkSave();
        },
        icon: SaveOutlined,
        status: 'activated',
        type: 'primary',
        size: 'large',
        specialType: 'success',
        tooltip: 'Click to save changes',
      },
    ];
    if (
      !department.value ||
      isEditing.value ||
      !departmentStore.getPermissions.includes(ResourceActions.Delete)
    )
      tempButtons[0].status = 'deactivated';

    if (
      !department.value?.id ||
      isEditing.value ||
      !department.value?.permissions?.includes(ResourceActions.Edit)
    )
      tempButtons[1].status = 'deactivated';

    if (!isEditing.value) {
      tempButtons[2].status = 'deactivated';
      tempButtons[3].status = 'deactivated';
    }
    if (!department.value?.permissions?.includes(ResourceActions.Edit)) {
      tempButtons[3].status = 'deactivated';
    }
    return tempButtons;
  });

  const deleteDepartment = async () => {
    if (!department.value) return;
    await departmentStore.delete(department.value?.id);
    emit('departmentDeleted');
    departmentStore.nullDepartment();
    setDepartmentId(null);
  };

  const isCancelModalOpen = ref(false);
  const openCancelModal = () => {
    isCancelModalOpen.value = true;
  };

  const isUniqueDepartmentName = (_rule: Rule, name: string) => {
    const departments: DepartmentModel[] = departmentStore.getDepartments;
    const currentDepartment: DepartmentModel | undefined =
      departmentStore.getDepartment;
    if (!currentDepartment) {
      return Promise.reject(new Error('Current department undefined'));
    }
    if (
      departments?.every(
        (department) =>
          department.departmentName !== name ||
          name === currentDepartment.departmentName,
      )
    ) {
      return Promise.resolve();
    }
    return Promise.reject(new Error('This department name is already in use.'));
  };

  const departmentNameRules: Rule[] = [
    {
      required: true,
      message: 'Please insert an unique department name.',
      validator: isUniqueDepartmentName,
      trigger: 'change',
      type: 'string',
    },
  ];
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
  <ConfirmAction
    :is-open="isCancelModalOpen"
    title="Cancel Editing"
    message="Are you sure you want to cancel all changes?"
    @confirm="stopEditing"
    @cancel="isCancelModalOpen = false"
    @update:is-open="(value) => (isCancelModalOpen = value)"
  />
  <div v-if="department && department.id" class="panel">
    <a-form ref="formRef" :model="formData" layout="vertical">
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
          :is-editing-key="'isEditing'"
          :has-edit-keys="false"
        >
          <InformationInputField
            v-model:value="formData.departmentName"
            attribute-name="departmentName"
            :placeholder="department?.departmentName ?? ''"
            :rules="departmentNameRules"
          />
        </EditableTextField>
      </a-flex>
    </a-form>
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
