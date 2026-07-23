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
    officeLocationRoutingSymbol,
    officeLocationStoreSymbol,
  } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';

  import FloatingButtonGroup from '@/components/Button/FloatingButtonGroup.vue';
  import ConfirmationDialog from '@/components/Modal/ConfirmAction.vue';

  import { useEditing, useThemeToken } from '@/utils/hooks';

  import { ResourceActions } from '@/models/utils';
  import { App } from 'ant-design-vue';
  import type { Rule } from 'ant-design-vue/es/form';
  import type { OfficeLocationModel } from '@/models/OfficeLocation';

  const token = useThemeToken();
  const { notification } = App.useApp();
  const route = useRoute();
  const { isEditing, stopEditing, startEditing } = useEditing();
  const formRef = ref();
  const officeLocationStore = inject(officeLocationStoreSymbol)!;
  const { getOfficeLocation, getIsLoadingOfficeLocation } =
    storeToRefs(officeLocationStore);
  const officeLocation = computed(() => getOfficeLocation.value);
  const isLoading = computed(() => getIsLoadingOfficeLocation.value);
  const { setOfficeLocationId } = inject(officeLocationRoutingSymbol)!;

  const emit = defineEmits(['officeLocationDeleted']);

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
    officeLocationName: '',
  });

  watch(
    () => officeLocation.value,
    (newOfficeLocation) => {
      if (!newOfficeLocation) return;
      formData.officeLocationName = newOfficeLocation.officeLocationName ?? '';
    },
  );

  const resetFormData = () => {
    const newOfficeLocation = officeLocation.value;
    if (!newOfficeLocation) return;
    formData.officeLocationName = newOfficeLocation.officeLocationName ?? '';
  };

  watch(
    () => officeLocation.value?.id,
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
      if (!officeLocation.value?.id) return;

      const updateRequest = {
        officeLocationName: formData.officeLocationName,
      };

      await officeLocationStore.update(officeLocation.value.id, updateRequest);

      await officeLocationStore.fetch(officeLocation.value?.id);

      notification.success({
        message: 'Success!',
        description: 'Office Location updated successfully.',
      });
      await stopEditing();
    } catch (error) {
      console.error('Validation or API error:', error);
      notification.error({
        message: 'Error!',
        description: (error as Error).message ?? 'An error occurred.',
      });
    }
  };

  const isConfirmModalOpen = ref<boolean>(false);
  const openModal = () => {
    isConfirmModalOpen.value = true;
  };
  const closeModal = () => {
    isConfirmModalOpen.value = false;
  };

  //Button for adding new OfficeLocation and deleting OfficeLocation
  const buttons = computed((): FloatButtonModel[] => {
    const tempButtons: FloatButtonModel[] = [
      {
        name: 'DeleteOfficeLocationButton',
        onClick: () => {
          openModal();
        },
        icon: DeleteOutlined,
        type: 'primary',
        specialType: 'danger',
        size: 'large',
        status: 'activated',
        tooltip: 'Click here to delete this officeLocation',
        isLink: false,
      },
      {
        name: 'EditOfficeLocationButton',
        onClick: () => {
          toggleEditingMode();
        },
        icon: EditOutlined,
        type: 'primary',
        size: 'large',
        status: 'activated',
        tooltip: 'Click here to edit this officeLocation',
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
      !officeLocation.value ||
      isEditing.value ||
      !officeLocation.value?.permissions?.includes(ResourceActions.Delete)
    )
      tempButtons[0].status = 'deactivated';

    if (
      !officeLocation.value?.id ||
      isEditing.value ||
      !officeLocation.value?.permissions?.includes(ResourceActions.Edit)
    )
      tempButtons[1].status = 'deactivated';

    if (!isEditing.value) {
      tempButtons[2].status = 'deactivated';
      tempButtons[3].status = 'deactivated';
    }
    if (!officeLocation.value?.permissions?.includes(ResourceActions.Edit)) {
      tempButtons[3].status = 'deactivated';
    }
    return tempButtons;
  });

  const deleteOfficeLocation = async () => {
    if (!officeLocation.value) return;
    try {
      await officeLocationStore.delete(officeLocation.value?.id);
      notification.success({
        message: 'Success!',
        description: 'Office Location deleted successfully.',
      });
      emit('officeLocationDeleted');
      officeLocationStore.nullOfficeLocation();
      setOfficeLocationId(null);
    } catch (error) {
      notification.error({
        message: 'Error!',
        description: (error as Error).message ?? 'An error occurred.',
      });
    }
  };

  const isCancelModalOpen = ref(false);
  const openCancelModal = () => {
    isCancelModalOpen.value = true;
  };

  const isUniqueOfficeLocationName = (_rule: Rule, name: string) => {
    const officeLocations: OfficeLocationModel[] =
      officeLocationStore.getOfficeLocations;
    const currentOfficeLocation: OfficeLocationModel | undefined =
      officeLocationStore.getOfficeLocation;
    if (!currentOfficeLocation) {
      return Promise.reject(new Error('Current officeLocation undefined'));
    }
    if (
      officeLocations?.every(
        (officeLocation) =>
          officeLocation.officeLocationName !== name ||
          name === currentOfficeLocation.officeLocationName,
      )
    ) {
      return Promise.resolve();
    }
    return Promise.reject(
      new Error('This officeLocation name is already in use.'),
    );
  };

  const locationNameRules: Rule[] = [
    {
      required: true,
      message: 'Please insert an unique officeLocation name.',
      validator: isUniqueOfficeLocationName,
      trigger: 'change',
      type: 'string',
    },
  ];
</script>
<template>
  <ConfirmationDialog
    :is-open="isConfirmModalOpen"
    title="Delete confirm"
    message="Are you sure you want to delete this office location?"
    @confirm="deleteOfficeLocation"
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
  <div v-if="officeLocation && officeLocation.id" class="panel">
    <a-form ref="formRef" :model="formData" layout="vertical">
      <a-flex
        class="userInfoBox"
        :body-style="{
          height: 'fit-content',
        }"
      >
        <EditableTextField
          class="textField officeLocationName"
          :value="officeLocation?.officeLocationName ?? ''"
          :is-loading="isLoading"
          :label="'Office Location\xa0Name'"
          :is-editing-key="'isEditing'"
          :has-edit-keys="false"
        >
          <InformationInputField
            v-model:value="formData.officeLocationName"
            :placeholder="officeLocation?.officeLocationName ?? ''"
            attribute-name="officeLocationName"
            :rules="locationNameRules"
          />
        </EditableTextField>
      </a-flex>
    </a-form>
  </div>
  <a-skeleton
    v-else-if="isLoading"
    :description="`No Office Location Found for Id ${route.query.officeLocationId}`"
  ></a-skeleton>
  <a-empty
    v-else-if="route.query.officeLocationId"
    :description="`No Office Location Found for Id ${route.query.officeLocationId}`"
  ></a-empty>
  <a-empty v-else description="No Office Location Selected"></a-empty>
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
