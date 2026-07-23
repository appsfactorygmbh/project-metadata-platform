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
    businessUnitRoutingSymbol,
    businessUnitStoreSymbol,
  } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';

  import FloatingButtonGroup from '@/components/Button/FloatingButtonGroup.vue';
  import ConfirmationDialog from '@/components/Modal/ConfirmAction.vue';
  import { useEditing, useThemeToken } from '@/utils/hooks';
  import { App } from 'ant-design-vue';
  import { ResourceActions } from '@/models/utils';
  import type { Rule } from 'ant-design-vue/es/form';
  import type { BusinessUnitModel } from '@/models/BusinessUnit';

  const token = useThemeToken();
  const { notification } = App.useApp();
  const route = useRoute();
  const { isEditing, stopEditing, startEditing } = useEditing();
  const formRef = ref();
  const businessUnitStore = inject(businessUnitStoreSymbol)!;
  const { getBusinessUnit, getIsLoadingBusinessUnit, getLinkedTeams } =
    storeToRefs(businessUnitStore);

  const businessUnit = computed(() => getBusinessUnit.value);
  const linkedTeams = computed(() => getLinkedTeams.value);
  const isLoading = computed(() => getIsLoadingBusinessUnit.value);
  const { setBusinessUnitId } = inject(businessUnitRoutingSymbol)!;

  const emit = defineEmits(['businessUnitDeleted']);

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
    businessUnitName: '',
  });

  watch(
    () => businessUnit.value,
    (newBusinessUnit) => {
      if (!newBusinessUnit) return;
      formData.businessUnitName = newBusinessUnit.businessUnitName ?? '';
    },
  );

  const resetFormData = () => {
    const newBusinessUnit = businessUnit.value;
    if (!newBusinessUnit) return;
    formData.businessUnitName = newBusinessUnit.businessUnitName ?? '';
  };

  watch(
    () => businessUnit.value?.id,
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
      if (!businessUnit.value?.id) return;

      const updateRequest = {
        businessUnitName: formData.businessUnitName,
      };

      await businessUnitStore.update(businessUnit.value.id, updateRequest);

      await businessUnitStore.fetch(businessUnit.value?.id);

      notification.success({
        message: 'Success!',
        description: 'Business Unit updated successfully.',
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
    if (linkedTeams.value.length > 0) {
      notification.error({
        message: 'Error!',
        description: `Business Unit is still linked to these teams: [${linkedTeams.value}]`,
      });
      return;
    }
    isConfirmModalOpen.value = true;
  };
  const closeModal = () => {
    isConfirmModalOpen.value = false;
  };

  //Button for adding new BusinessUnit and deleting BusinessUnit
  const buttons = computed((): FloatButtonModel[] => {
    const tempButtons: FloatButtonModel[] = [
      {
        name: 'DeleteBusinessUnitButton',
        onClick: () => {
          openModal();
        },
        icon: DeleteOutlined,
        type: 'primary',
        specialType: 'danger',
        size: 'large',
        status: 'activated',
        tooltip: 'Click here to delete this business unit',
        isLink: false,
      },
      {
        name: 'EditBusinessUnitButton',
        onClick: () => {
          toggleEditingMode();
        },
        icon: EditOutlined,
        type: 'primary',
        size: 'large',
        status: 'activated',
        tooltip: 'Click here to edit this business unit',
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
      !businessUnit.value ||
      isEditing.value ||
      !businessUnit.value?.permissions?.includes(ResourceActions.Delete)
    )
      tempButtons[0].status = 'deactivated';

    if (
      !businessUnit.value?.id ||
      isEditing.value ||
      !businessUnit.value?.permissions?.includes(ResourceActions.Edit)
    )
      tempButtons[1].status = 'deactivated';

    if (!isEditing.value) {
      tempButtons[2].status = 'deactivated';
      tempButtons[3].status = 'deactivated';
    }
    if (!businessUnit.value?.permissions?.includes(ResourceActions.Edit)) {
      tempButtons[3].status = 'deactivated';
    }
    return tempButtons;
  });

  const deleteBusinessUnit = async () => {
    if (!businessUnit.value) return;
    try {
      await businessUnitStore.delete(businessUnit.value?.id);
      notification.success({
        message: 'Success!',
        description: 'Business Unit deleted successfully.',
      });
      emit('businessUnitDeleted');
      businessUnitStore.nullBusinessUnit();
      setBusinessUnitId(null);
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

  const isUniqueBusinessUnitName = (_rule: Rule, name: string) => {
    const businessUnits: BusinessUnitModel[] =
      businessUnitStore.getBusinessUnits;
    const currentBusinessUnit: BusinessUnitModel | undefined =
      businessUnitStore.getBusinessUnit;
    if (!currentBusinessUnit) {
      return Promise.reject(new Error('Current business unit undefined'));
    }
    if (
      businessUnits?.every(
        (businessUnit) =>
          businessUnit.businessUnitName !== name ||
          name === currentBusinessUnit.businessUnitName,
      )
    ) {
      return Promise.resolve();
    }
    return Promise.reject(
      new Error('This business unit name is already in use.'),
    );
  };

  const businessUnitNameRules: Rule[] = [
    {
      required: true,
      message: 'Please insert an unique business unit name.',
      validator: isUniqueBusinessUnitName,
      trigger: 'change',
      type: 'string',
    },
  ];
</script>
<template>
  <ConfirmationDialog
    :is-open="isConfirmModalOpen"
    title="Delete confirm"
    message="Are you sure you want to delete this business unit?"
    @confirm="deleteBusinessUnit"
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
  <div v-if="businessUnit && businessUnit.id" class="panel">
    <a-form ref="formRef" :model="formData" layout="vertical">
      <a-flex
        class="userInfoBox"
        :body-style="{
          height: 'fit-content',
        }"
      >
        <EditableTextField
          class="textField businessUnitName"
          :value="businessUnit?.businessUnitName ?? ''"
          :is-loading="isLoading"
          :label="'Business Unit\xa0Name'"
          :is-editing-key="'isEditing'"
          :has-edit-keys="false"
        >
          <InformationInputField
            v-model:value="formData.businessUnitName"
            :placeholder="businessUnit?.businessUnitName ?? ''"
            attribute-name="businessUnitName"
            :rules="businessUnitNameRules"
          />
        </EditableTextField>
      </a-flex>
    </a-form>
  </div>
  <a-skeleton
    v-else-if="isLoading"
    :description="`No Business Unit Found for Id ${route.query.businessUnitId}`"
  ></a-skeleton>
  <a-empty
    v-else-if="route.query.businessUnitId"
    :description="`No Business Unit Found for Id ${route.query.businessUnitId}`"
  ></a-empty>
  <a-empty v-else description="No Business Unit Selected"></a-empty>
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
