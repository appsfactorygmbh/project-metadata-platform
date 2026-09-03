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
    globalBillingRoutingSymbol,
    globalBillingStoreSymbol,
  } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';

  import FloatingButtonGroup from '@/components/Button/FloatingButtonGroup.vue';
  import ConfirmationDialog from '@/components/Modal/ConfirmAction.vue';
  import { useEditing, useThemeToken } from '@/utils/hooks';
  import { ResourceActions } from '@/models/utils';
  import type { Rule } from 'ant-design-vue/es/form';
  import type { GlobalBillingModel } from '@/models/GlobalBilling';
  import { App } from 'ant-design-vue';
  import router from '@/router';
  import { TimeFrame } from '@/api/generated';
  import type { SelectOption } from '@/components/EditableTextField/InputFields/InformationSearchSelectField.vue';

  const token = useThemeToken();
  const { notification } = App.useApp();
  const route = useRoute();
  const { isEditing, stopEditing, startEditing } = useEditing();
  const formRef = ref();
  const globalBillingStore = inject(globalBillingStoreSymbol)!;
  const { getGlobalBilling, getIsLoadingGlobalBilling } =
    storeToRefs(globalBillingStore);
  const globalBilling = computed(() => getGlobalBilling.value);
  const isLoading = computed(() => getIsLoadingGlobalBilling.value);
  const { setGlobalBillingId } = inject(globalBillingRoutingSymbol)!;

  const emit = defineEmits(['globalBillingDeleted']);

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
    billingKind: '',
    currency: null as string | null,
    budgetLimit: null as number | null,
    hostingFee: null as number | null,
    targetMargin: null as number | null,
    timeFrame: null as TimeFrame | null,
  });

  watch(
    () => globalBilling.value,
    (newGlobalBilling) => {
      if (!newGlobalBilling) return;
      formData.billingKind = newGlobalBilling.billingKind ?? '';
      formData.currency = newGlobalBilling.currency ?? null;
      formData.budgetLimit = newGlobalBilling.budgetLimit ?? null;
      formData.hostingFee = newGlobalBilling.hostingFee ?? null;
      formData.targetMargin = newGlobalBilling.targetMargin ?? null;
      formData.timeFrame = newGlobalBilling.timeFrame ?? null;
    },
  );

  const resetFormData = () => {
    const newGlobalBilling = globalBilling.value;
    if (!newGlobalBilling) return;
    formData.billingKind = newGlobalBilling.billingKind ?? '';
    formData.currency = newGlobalBilling.currency ?? null;
    formData.budgetLimit = newGlobalBilling.budgetLimit ?? null;
    formData.hostingFee = newGlobalBilling.hostingFee ?? null;
    formData.targetMargin = newGlobalBilling.targetMargin ?? null;
    formData.timeFrame = newGlobalBilling.timeFrame ?? null;
  };

  watch(
    () => globalBilling.value?.id,
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
      if (!globalBilling.value?.id) return;

      const updateRequest = {
        billingKind: formData.billingKind,
        currency: formData.currency,
        budgetLimit: formData.budgetLimit,
        hostingFee: formData.hostingFee,
        targetMargin: formData.targetMargin,
        timeFrame: formData.timeFrame ?? undefined,
      };

      await globalBillingStore.update(globalBilling.value.id, updateRequest);
      await stopEditing();

      notification.success({
        message: 'Success!',
        description: 'GlobalBilling updated successfully.',
      });
      await globalBillingStore.fetch(globalBilling.value?.id);
    } catch (error) {
      console.error('Validation or API error:', error);
      notification.error({
        message: 'Error!',
        description: (error as Error).message ?? 'An error occurred.',
      });
      if (
        !isEditing.value &&
        (error as Error).message === 'This action is unauthorized.'
      ) {
        router.push('/403');
      }
    }
  };

  const isConfirmModalOpen = ref<boolean>(false);
  const openModal = () => {
    isConfirmModalOpen.value = true;
  };
  const closeModal = () => {
    isConfirmModalOpen.value = false;
  };

  //Button for adding new GlobalBilling and deleting GlobalBilling
  const buttons = computed((): FloatButtonModel[] => {
    const tempButtons: FloatButtonModel[] = [
      {
        name: 'DeleteGlobalBillingButton',
        onClick: () => {
          openModal();
        },
        icon: DeleteOutlined,
        type: 'primary',
        specialType: 'danger',
        size: 'large',
        status: 'activated',
        tooltip: 'Click here to delete this Billing Information',
        isLink: false,
      },
      {
        name: 'EditGlobalBillingButton',
        onClick: () => {
          toggleEditingMode();
        },
        icon: EditOutlined,
        type: 'primary',
        size: 'large',
        status: 'activated',
        tooltip: 'Click here to edit this Billing Information',
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
      !globalBilling.value ||
      isEditing.value ||
      !globalBilling.value?.permissions?.includes(ResourceActions.Delete)
    )
      tempButtons[0].status = 'deactivated';

    if (
      !globalBilling.value?.id ||
      isEditing.value ||
      !globalBilling.value?.permissions?.includes(ResourceActions.Edit)
    )
      tempButtons[1].status = 'deactivated';

    if (!isEditing.value) {
      tempButtons[2].status = 'deactivated';
      tempButtons[3].status = 'deactivated';
    }
    if (!globalBilling.value?.permissions?.includes(ResourceActions.Edit)) {
      tempButtons[3].status = 'deactivated';
    }
    return tempButtons;
  });

  const deleteGlobalBilling = async () => {
    if (!globalBilling.value) return;
    try {
      await globalBillingStore.delete(globalBilling.value?.id);
      emit('globalBillingDeleted');
      notification.success({
        message: 'Success!',
        description: 'Global Billing Information deleted successfully.',
      });
      globalBillingStore.nullGlobalBilling();
      setGlobalBillingId(null);
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

  const isUniqueGlobalBillingKind = (_rule: Rule, name: string) => {
    const billing: GlobalBillingModel[] =
      globalBillingStore.getGlobalBillingList;
    const currentGlobalBilling: GlobalBillingModel | undefined =
      globalBillingStore.getGlobalBilling;
    if (!currentGlobalBilling) {
      return Promise.reject(new Error('Current globalBilling undefined'));
    }
    if (
      billing?.every(
        (globalBilling) =>
          globalBilling.billingKind !== name ||
          name === currentGlobalBilling.billingKind,
      )
    ) {
      return Promise.resolve();
    }
    return Promise.reject(
      new Error('This global Billing kind is already in use.'),
    );
  };

  const billingKindRules: Rule[] = [
    {
      required: true,
      message: 'Please insert an unique kind of billing information.',
      validator: isUniqueGlobalBillingKind,
      trigger: 'change',
      type: 'string',
    },
  ];

  const timeFrameOptions = computed(() => {
    const options: SelectOption[] = Object.entries(TimeFrame).map(
      ([key, value]) => ({
        id: value,
        name: key,
      }),
    );
    return options.concat({
      id: null,
      name: 'None',
    });
  });
</script>
<template>
  <ConfirmationDialog
    :is-open="isConfirmModalOpen"
    title="Delete confirm"
    message="Are you sure you want to delete this billing information?"
    @confirm="deleteGlobalBilling"
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
  <div v-if="globalBilling && globalBilling.id" class="panel">
    <a-form ref="formRef" :model="formData" layout="vertical">
      <a-flex
        class="userInfoBox"
        :body-style="{
          height: 'fit-content',
        }"
      >
        <EditableTextField
          class="textField billingKind"
          :value="globalBilling?.billingKind ?? ''"
          :is-loading="isLoading"
          :label="'Kind'"
          :is-editing-key="'isEditing'"
          :has-edit-keys="false"
        >
          <InformationInputField
            v-model:value="formData.billingKind"
            attribute-name="billingKind"
            :placeholder="globalBilling?.billingKind ?? ''"
            :rules="billingKindRules"
          />
        </EditableTextField>
        <EditableTextField
          class="textField budgetlimit"
          :value="globalBilling?.budgetLimit ?? ''"
          :is-loading="isLoading"
          :label="'Budget limit'"
          :is-editing-key="'isEditing'"
          :has-edit-keys="false"
        >
          <NumericInformationInputField
            v-model:value="formData.budgetLimit"
            attribute-name="budgetLimit"
            :placeholder="globalBilling?.budgetLimit?.toString() ?? ''"
          />
        </EditableTextField>
        <EditableTextField
          class="textField hostingFee"
          :value="globalBilling?.hostingFee ?? ''"
          :is-loading="isLoading"
          :label="'Hosting Fee'"
          :is-editing-key="'isEditing'"
          :has-edit-keys="false"
        >
          <NumericInformationInputField
            v-model:value="formData.hostingFee"
            attribute-name="hostingFee"
            :placeholder="globalBilling?.hostingFee?.toString() ?? ''"
          />
        </EditableTextField>
        <EditableTextField
          class="textField currency"
          :value="globalBilling?.currency ?? ''"
          :is-loading="isLoading"
          :label="'Currency'"
          :is-editing-key="'isEditing'"
          :has-edit-keys="false"
        >
          <InformationInputField
            v-model:value="formData.currency"
            attribute-name="currency"
            :placeholder="globalBilling?.currency ?? ''"
          />
        </EditableTextField>
        <EditableTextField
          class="textField targetMargin"
          :value="
            globalBilling?.targetMargin != null
              ? globalBilling?.targetMargin + '%'
              : ''
          "
          :is-loading="isLoading"
          :label="'Target Margin'"
          :is-editing-key="'isEditing'"
          :has-edit-keys="false"
        >
          <NumericInformationInputField
            v-model:value="formData.targetMargin"
            attribute-name="targetMargin"
            :placeholder="globalBilling?.targetMargin?.toString() ?? ''"
            :precision="0"
          />
        </EditableTextField>
        <EditableTextField
          class="textField timeFrame"
          :value="globalBilling?.timeFrame ?? ''"
          :is-loading="isLoading"
          :label="'Time Frame'"
          :is-editing-key="'isEditing'"
          :has-edit-keys="false"
        >
          <InformationSearchSelectField
            v-model:value="formData.timeFrame"
            :attribute-name="'timeFrame'"
            :placeholder="globalBilling.timeFrame ?? ''"
            :options="timeFrameOptions"
          />
        </EditableTextField>
      </a-flex>
    </a-form>
  </div>
  <a-skeleton
    v-else-if="isLoading"
    :description="`No Global Billing Information Found for Id ${route.query.globalBillingId}`"
  ></a-skeleton>
  <a-empty
    v-else-if="route.query.globalBillingId"
    :description="`No GlobalBilling Found for Id ${route.query.globalBillingId}`"
  ></a-empty>
  <a-empty
    v-else
    description="No Global Billing Information Selected"
  ></a-empty>
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
