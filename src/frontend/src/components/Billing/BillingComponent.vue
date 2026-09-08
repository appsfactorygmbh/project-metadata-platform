<script setup lang="ts">
  import { ref, computed, reactive } from 'vue';
  import {
    CreditCardOutlined,
    EditOutlined,
    DeleteOutlined,
  } from '@ant-design/icons-vue';
  import { App } from 'ant-design-vue';
  import { useThemeToken } from '@/utils/hooks';
  import { ResourceActions } from '@/models/utils';
  import { useBillingStore } from '@/store/BillingStore';
  import ConfirmationDialog from '@/components/Modal/ConfirmAction.vue';
  import { Currencies, TimeFrame } from '@/api/generated';
  import type { Rule } from 'ant-design-vue/es/form';

  const token = useThemeToken();
  const { notification } = App.useApp();
  const formRef = ref();
  const billingStore = useBillingStore();
  export interface SelectOption {
    id: number | string | null;
    name: string;
  }
  const props = defineProps({
    projectId: { type: Number, required: true },
    pluginId: { type: Number, required: true },
  });

  const emit = defineEmits(['billingStateUpdated']);
  const isOpen = ref(false);
  const isLoading = ref(false);
  const localIsEditing = ref(false);
  interface BillingFormData {
    contractIds: string[];
    currency: Currencies;
    budgetLimit: number;
    hostingFee: number;
    targetMargin: number;
    timeFrame: TimeFrame;
    date: Date | undefined;
    notes: string | undefined;
  }
  const formData = reactive<BillingFormData>({
    contractIds: [],
    currency: Currencies.Eur,
    budgetLimit: 0,
    hostingFee: 0,
    targetMargin: 0,
    timeFrame: TimeFrame.Never,
    date: undefined as Date | undefined,
    notes: '',
  });

  const resetFormData = () => {
    const newBilling = billingData.value;
    if (!newBilling) return;
    formData.contractIds = newBilling.contractIds ?? [];
    formData.currency = newBilling.currency;
    formData.budgetLimit = newBilling.budgetLimit ?? 0;
    formData.hostingFee = newBilling.hostingFee ?? 0;
    formData.targetMargin = newBilling.targetMargin ?? 0;
    formData.timeFrame = newBilling.timeFrame ?? TimeFrame.Never;
    formData.date = newBilling.date ?? undefined;
    formData.notes = newBilling.notes ?? '';
  };

  const getPopupContainer = (triggerNode: HTMLElement): HTMLElement => {
    return (triggerNode.parentNode as HTMLElement) || document.body;
  };
  const getInnerPopupContainer = (triggerNode: HTMLElement) => {
    const popoverContent = triggerNode.closest(
      '.popover-content',
    ) as HTMLElement;

    return popoverContent || document.body;
  };

  const billingData = computed(() => billingStore.getBilling);

  const toggleEdit = async () => {
    resetFormData();
    localIsEditing.value = !localIsEditing.value;
  };

  const handleUpdate = async () => {
    try {
      await formRef.value.validate();
      if (
        billingData.value?.projectId == null ||
        billingData.value?.pluginId == null
      ) {
        return;
      }
      const updateRequest = {
        contractIds: formData.contractIds,
        currency: formData.currency,
        budgetLimit: formData.budgetLimit,
        hostingFee: formData.hostingFee,
        targetMargin: formData.targetMargin,
        timeFrame: formData.timeFrame,
        date: formData.date,
        notes: formData.notes,
      };

      await billingStore.update(
        billingData.value?.projectId,
        billingData.value?.pluginId,
        updateRequest,
      );
      await toggleEdit();

      notification.success({
        message: 'Success!',
        description: 'GlobalBilling updated successfully.',
      });
      await billingStore.fetch(
        billingData.value?.projectId,
        billingData.value?.pluginId,
      );
    } catch (error) {
      console.error('Validation or API error:', error);
      notification.error({
        message: 'Error!',
        description: (error as Error).message ?? 'An error occurred.',
      });
    }
  };

  const handleOpen = async () => {
    isOpen.value = true;

    try {
      isLoading.value = true;
      await billingStore.fetch(props.projectId, props.pluginId);
    } catch (error) {
      notification.error({
        message: 'Error',
        description: (error as Error).message,
      });
    } finally {
      isLoading.value = false;
    }
  };

  const handleOpenChange = async (open: boolean) => {
    if (!open && isConfirmModalOpen.value) {
      return;
    }
    isOpen.value = open;
    if (!open) {
      resetFormData();
      localIsEditing.value = false;
    }
  };
  const isConfirmModalOpen = ref<boolean>(false);
  const openModal = () => {
    isConfirmModalOpen.value = true;
  };
  const closeModal = () => {
    isConfirmModalOpen.value = false;
  };
  const deletebilling = async () => {
    if (!billingData.value) return;
    try {
      await billingStore.remove(props.projectId, props.pluginId);
      emit('billingStateUpdated');
      notification.success({
        message: 'Success!',
        description: 'Billing Information removed successfully.',
      });
      isOpen.value = false;
    } catch (error) {
      notification.error({
        message: 'Error!',
        description: (error as Error).message ?? 'An error occurred.',
      });
    }
  };

  const isDateNeeded = () => {
    if (formData.timeFrame == TimeFrame.Date && formData.date == null) {
      return Promise.reject(new Error('Date is needed'));
    } else {
      return Promise.resolve();
    }
  };

  const dateRules: Rule[] = [
    {
      required: true,
      message: 'Please add a valid date.',
      validator: isDateNeeded,
      trigger: ['change', 'blur'],
    },
  ];

  const getCurrencyName = (currencyCode: string) => {
    try {
      return (
        new Intl.DisplayNames([], { type: 'currency' }).of(currencyCode) ||
        currencyCode
      );
    } catch {
      return currencyCode;
    }
  };

  const formatCurrencyValue = (
    value: number | null | undefined,
    currencyCode?: string | null,
  ) => {
    if (value == null) return '';
    if (!currencyCode) return value.toString();

    try {
      return new Intl.NumberFormat([], {
        style: 'currency',
        currency: currencyCode,
      }).format(value);
    } catch {
      return value.toString();
    }
  };

  const currencyOptions = computed(() => {
    const options: SelectOption[] = Object.entries(Currencies).map(
      ([value]) => ({
        id: value,
        name: getCurrencyName(value),
      }),
    );
    return options;
  });

  const getTimeFrameName = (value: TimeFrame | null | undefined) => {
    if (!value) return '';
    const key = Object.keys(TimeFrame).find(
      (key) => TimeFrame[key as keyof typeof TimeFrame] === value,
    );

    return key || value;
  };
</script>

<template>
  <ConfirmationDialog
    :is-open="isConfirmModalOpen"
    :z-index="1050"
    title="Removal confirm"
    message="Are you sure you want to remove this billing information?"
    @confirm="deletebilling"
    @cancel="closeModal"
    @update:is-open="isConfirmModalOpen = $event"
  />
  <a-popover
    v-bind="$attrs"
    :open="isOpen"
    trigger="click"
    placement="bottomLeft"
    :get-popup-container="getPopupContainer"
    @update:open="handleOpenChange"
  >
    <template #title>
      <div
        style="
          display: flex;
          justify-content: space-between;
          align-items: center;
          width: 100%;
          padding-bottom: 4px;
        "
      >
        <span :style="{ color: token.colorText, fontWeight: 'bold' }">
          {{ 'Billing Information' }}
        </span>

        <div
          v-if="!isLoading"
          style="display: flex; gap: 8px; margin-left: 20px"
        >
          <a-button
            v-if="billingData?.permissions?.includes(ResourceActions.Edit)"
            size="small"
            :type="localIsEditing ? 'default' : 'primary'"
            @click.stop="toggleEdit"
          >
            <template v-if="!localIsEditing" #icon>
              <EditOutlined />
            </template>
            {{ localIsEditing ? 'Cancel' : 'Edit' }}
          </a-button>

          <a-button
            v-if="
              !localIsEditing &&
              billingData?.permissions?.includes(ResourceActions.Delete)
            "
            size="small"
            danger
            @click.stop="openModal"
          >
            <template #icon>
              <DeleteOutlined />
            </template>
            Delete
          </a-button>
          <a-button
            v-if="localIsEditing"
            size="small"
            :type="'primary'"
            @click.stop="handleUpdate"
          >
            {{ 'Save' }}
          </a-button>
        </div>
      </div>
    </template>
    <template #content>
      <div class="popover-content" :class="{ 'is-editing': localIsEditing }">
        <a-spin v-if="isLoading" class="spinner" />
        <div
          v-else-if="
            billingData && billingData.pluginId && billingData.projectId
          "
          class="panel"
        >
          <a-form ref="formRef" :model="formData" layout="vertical">
            <a-flex
              class="billing-form"
              :body-style="{
                height: 'fit-content',
              }"
            >
              <EditableTextField
                v-if="billingData?.contractIds.length > 0 || localIsEditing"
                class="textField contractId"
                :value="billingData?.contractIds.join(', ') ?? ''"
                :is-loading="isLoading"
                :label="'Contract Id'"
                :is-editing-local="localIsEditing"
                :has-edit-keys="false"
              >
                <InformationListInputField
                  v-model:value="formData.contractIds"
                  attribute-name="Contract Ids"
                  mode="tags"
                  :placeholder="billingData?.contractIds.join(', ')"
                  :options="[]"
                />
              </EditableTextField>
              <EditableTextField
                class="textField budgetLimit"
                :value="
                  formatCurrencyValue(
                    billingData.budgetLimit,
                    billingData.currency,
                  )
                "
                :is-loading="isLoading"
                :label="'Budget limit'"
                :is-editing-local="localIsEditing"
                :has-edit-keys="false"
              >
                <NumericInformationInputField
                  v-model:value="formData.budgetLimit"
                  attribute-name="budgetLimit"
                  :placeholder="billingData?.budgetLimit?.toString() ?? ''"
                  :max="999999999"
                  :min="0"
                />
              </EditableTextField>
              <EditableTextField
                class="textField hostingFee"
                :value="
                  formatCurrencyValue(
                    billingData.hostingFee,
                    billingData.currency,
                  )
                "
                :is-loading="isLoading"
                :label="'Hosting Fee'"
                :is-editing-local="localIsEditing"
                :has-edit-keys="false"
              >
                <NumericInformationInputField
                  v-model:value="formData.hostingFee"
                  attribute-name="hostingFee"
                  :placeholder="billingData?.hostingFee?.toString() ?? ''"
                  :max="999999999"
                  :min="0"
                />
              </EditableTextField>
              <EditableTextField
                class="textField currency"
                :value="
                  getCurrencyName(billingData.currency) +
                  ' (' +
                  billingData.currency +
                  ')'
                "
                :is-loading="isLoading"
                :label="'Currency'"
                :is-editing-local="localIsEditing"
                :has-edit-keys="false"
              >
                <InformationSearchSelectField
                  v-model:value="formData.currency"
                  attribute-name="currency"
                  :get-popup-container="getInnerPopupContainer"
                  :placeholder="billingData.currency ?? ''"
                  :options="currencyOptions"
                />
              </EditableTextField>
              <EditableTextField
                class="textField targetMargin"
                :value="
                  billingData?.targetMargin != null
                    ? billingData?.targetMargin + '%'
                    : ''
                "
                :is-loading="isLoading"
                :label="'Target Margin'"
                :is-editing-local="localIsEditing"
                :has-edit-keys="false"
              >
                <NumericInformationInputField
                  v-model:value="formData.targetMargin"
                  attribute-name="targetMargin"
                  :placeholder="billingData?.targetMargin?.toString() ?? ''"
                  :precision="0"
                  :max="100"
                  :min="0"
                />
              </EditableTextField>
              <EditableTextField
                v-if="billingData.timeFrame != TimeFrame.Date || localIsEditing"
                class="textField timeFrame"
                :value="getTimeFrameName(billingData?.timeFrame)"
                :is-loading="isLoading"
                :label="'Time Frame'"
                :is-editing-local="localIsEditing"
                :has-edit-keys="false"
              >
                <InformationSearchSelectField
                  v-model:value="formData.timeFrame"
                  :attribute-name="'timeFrame'"
                  :placeholder="billingData.timeFrame ?? ''"
                  :get-popup-container="getInnerPopupContainer"
                  :options="
                    Object.entries(TimeFrame).map(([key, value]) => ({
                      id: value,
                      name: key,
                    }))
                  "
                />
              </EditableTextField>
              <EditableTextField
                v-if="
                  (billingData.timeFrame == TimeFrame.Date &&
                    !localIsEditing) ||
                  (formData.timeFrame == TimeFrame.Date && localIsEditing)
                "
                class="textField date"
                :value="billingData?.date?.toLocaleDateString() ?? ''"
                :is-loading="isLoading"
                :label="'Date'"
                :is-editing-local="localIsEditing"
                :has-edit-keys="false"
              >
                <DateInputField
                  v-model:value="formData.date"
                  :get-popup-container="getInnerPopupContainer"
                  attribute-name="date"
                  :start-date="new Date(Date.now())"
                  :placeholder="
                    billingData?.date?.toLocaleDateString() ?? 'Date'
                  "
                  :rules="dateRules"
                />
              </EditableTextField>
              <EditableTextField
                v-if="(billingData.notes?.length ?? 0) > 0 || localIsEditing"
                class="textField notes"
                :value="billingData?.notes ?? ''"
                :is-loading="isLoading"
                :label="'Notes'"
                :is-editing-local="localIsEditing"
                :has-edit-keys="false"
              >
                <InformationInputTextArea
                  v-model:value="formData.notes"
                  attribute-name="notes"
                  :placeholder="billingData?.notes ?? 'Notes'"
                  :max-length="280"
                />
              </EditableTextField>
            </a-flex>
          </a-form>
        </div>
      </div>
    </template>
    <CreditCardOutlined
      class="action-badge billing-badge"
      :class="{ 'force-visible': isOpen || isConfirmModalOpen }"
      @click.prevent.stop="handleOpen"
    />
  </a-popover>
</template>

<style scoped lang="scss">
  .action-badge {
    position: absolute;
    bottom: -8px;
    left: 8px;
    transform: translateX(-50%);
    z-index: 10;
    width: 24px;
    height: 24px;
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: 50%;
    box-shadow: v-bind('token.boxShadowSecondary');
    color: v-bind('token.colorTextLightSolid');

    font-size: 12px;
    cursor: pointer;
    opacity: 0;
    transition: all 0.2s ease-in-out;

    &:hover {
      transform: translateX(-50%) scale(1.1);
    }
    &.force-visible {
      opacity: 1 !important;
    }
  }

  .billing-badge {
    background-color: v-bind('token.colorSuccess');
  }

  .popover-content {
    min-height: 50px;
    display: flex;
    flex-direction: column;
    justify-content: center;
    color: v-bind('token.colorText');

    transition: width 0.2s ease-in-out;
  }

  .popover-content:not(.is-editing) {
    min-width: 250px;
    width: max-content;
    max-width: 450px;
  }

  .popover-content.is-editing {
    width: 320px;
  }

  .spinner {
    margin: 20px auto;
  }

  .billing-form {
    display: flex;
    flex-direction: column;
    gap: 8px;
  }

  .textField {
    font-size: 12px;
    color: v-bind('token.colorTextSecondary');
    margin-bottom: 4px;

    background-color: transparent !important;
  }

  :deep(.label) {
    color: v-bind('token.colorText');
  }

  .form-actions {
    display: flex;
    justify-content: flex-end;
    gap: 8px;
    margin-top: 4px;
  }

  .billing-view {
    display: flex;
    flex-direction: column;
    gap: 6px;
    font-size: 13px;
    color: v-bind('token.colorText');
  }

  .view-row {
    display: flex;
    justify-content: space-between;
    border-bottom: 1px solid v-bind('token.colorBorderSecondary');
    padding-bottom: 2px;
  }
</style>
