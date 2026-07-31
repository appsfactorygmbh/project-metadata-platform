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
    companyRoutingSymbol,
    companyStoreSymbol,
  } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';

  import FloatingButtonGroup from '@/components/Button/FloatingButtonGroup.vue';
  import ConfirmationDialog from '@/components/Modal/ConfirmAction.vue';
  import { useEditing, useThemeToken } from '@/utils/hooks';

  import { ResourceActions } from '@/models/utils';
  import type { Rule } from 'ant-design-vue/es/form';
  import type { CompanyModel } from '@/models/Company';
  import { App } from 'ant-design-vue';

  const { notification } = App.useApp();

  const token = useThemeToken();

  const route = useRoute();
  const { isEditing, stopEditing, startEditing } = useEditing();
  const formRef = ref();
  const companyStore = inject(companyStoreSymbol)!;
  const { getCompany, getIsLoadingCompany, getLinkedProjects } =
    storeToRefs(companyStore);
  const company = computed(() => getCompany.value);
  const linkedProjects = computed(() => getLinkedProjects.value);
  const isLoading = computed(() => getIsLoadingCompany.value);
  const { setCompanyId } = inject(companyRoutingSymbol)!;

  const emit = defineEmits(['companyDeleted']);

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
    companyName: '',
  });

  watch(
    () => company.value,
    (newCompany) => {
      if (!newCompany) return;
      formData.companyName = newCompany.companyName ?? '';
    },
  );

  const resetFormData = () => {
    const newCompany = company.value;
    if (!newCompany) return;
    formData.companyName = newCompany.companyName ?? '';
  };

  watch(
    () => company.value?.id,
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
      if (!company.value?.id) return;

      const updateRequest = {
        companyName: formData.companyName,
      };

      await companyStore.update(company.value.id, updateRequest);

      await companyStore.fetch(company.value?.id);

      notification.success({
        message: 'Success!',
        description: 'Company updated successfully.',
      });
      await stopEditing();
    } catch (error) {
      console.error('Validation or API error:', error);
      notification.error({
        message: 'Error!',
        description: (error as Error).message ?? 'An error occurred',
      });
      return;
    }
  };

  const isConfirmModalOpen = ref<boolean>(false);
  const openModal = () => {
    if (linkedProjects.value.length > 0) {
      notification.error({
        message: 'Error!',
        description: `Company is still linked to these projects: [${linkedProjects.value}]`,
      });

      return;
    }
    isConfirmModalOpen.value = true;
  };
  const closeModal = () => {
    isConfirmModalOpen.value = false;
  };

  //Button for adding new Company and deleting Company
  const buttons = computed((): FloatButtonModel[] => {
    const tempButtons: FloatButtonModel[] = [
      {
        name: 'DeleteCompanyButton',
        onClick: () => {
          openModal();
        },
        icon: DeleteOutlined,
        type: 'primary',
        specialType: 'danger',
        size: 'large',
        status: 'activated',
        tooltip: 'Click here to delete this company',
        isLink: false,
      },
      {
        name: 'EditCompanyButton',
        onClick: () => {
          toggleEditingMode();
        },
        icon: EditOutlined,
        type: 'primary',
        size: 'large',
        status: 'activated',
        tooltip: 'Click here to edit this company',
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
      !company.value ||
      isEditing.value ||
      !company.value?.permissions?.includes(ResourceActions.Delete)
    )
      tempButtons[0].status = 'deactivated';

    if (
      !company.value?.id ||
      isEditing.value ||
      !company.value?.permissions?.includes(ResourceActions.Edit)
    )
      tempButtons[1].status = 'deactivated';

    if (!isEditing.value) {
      tempButtons[2].status = 'deactivated';
      tempButtons[3].status = 'deactivated';
    }
    if (!company.value?.permissions?.includes(ResourceActions.Edit)) {
      tempButtons[3].status = 'deactivated';
    }
    return tempButtons;
  });

  const deleteCompany = async () => {
    if (!company.value) return;
    try {
      notification.success({
        message: 'Success!',
        description: 'Company deleted successfully.',
      });
      await companyStore.delete(company.value?.id);
      emit('companyDeleted');
      companyStore.nullCompany();
      setCompanyId(null);
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

  const isUniqueCompanyName = (_rule: Rule, name: string) => {
    const companies: CompanyModel[] = companyStore.getCompanies;
    const currentCompany: CompanyModel | undefined = companyStore.getCompany;
    if (!currentCompany) {
      return Promise.reject(new Error('Current company undefined'));
    }
    if (
      companies?.every(
        (company) =>
          company.companyName !== name || name === currentCompany.companyName,
      )
    ) {
      return Promise.resolve();
    }
    return Promise.reject(new Error('This company name is already in use.'));
  };

  const companyNameRules: Rule[] = [
    {
      required: true,
      message: 'Please insert an unique company name.',
      validator: isUniqueCompanyName,
      trigger: 'change',
      type: 'string',
    },
  ];
</script>
<template>
  <ConfirmationDialog
    :is-open="isConfirmModalOpen"
    title="Delete confirm"
    message="Are you sure you want to delete this company?"
    @confirm="deleteCompany"
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
  <div v-if="company && company.id" class="panel">
    <a-form ref="formRef" :model="formData" layout="vertical">
      <a-flex
        class="userInfoBox"
        :body-style="{
          height: 'fit-content',
        }"
      >
        <EditableTextField
          class="textField companyName"
          :value="company?.companyName ?? ''"
          :is-loading="isLoading"
          :label="'Company\xa0Name'"
          :is-editing-key="'isEditing'"
          :has-edit-keys="false"
        >
          <InformationInputField
            v-model:value="formData.companyName"
            :placeholder="company?.companyName ?? ''"
            attribute-name="companyName"
            :rules="companyNameRules"
          />
        </EditableTextField>
      </a-flex>
    </a-form>
  </div>
  <a-skeleton
    v-else-if="isLoading"
    :description="`No Company Found for Id ${route.query.companyId}`"
  ></a-skeleton>
  <a-empty
    v-else-if="route.query.companyId"
    :description="`No Company Found for Id ${route.query.companyId}`"
  ></a-empty>
  <a-empty v-else description="No Company Selected"></a-empty>
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
