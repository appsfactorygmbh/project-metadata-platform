<script lang="ts" setup>
  import { DeleteOutlined } from '@ant-design/icons-vue';
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';
  import { inject, ref } from 'vue';
  import {
    officeLocationRoutingSymbol,
    officeLocationStoreSymbol,
  } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';

  import FloatingButtonGroup from '@/components/Button/FloatingButtonGroup.vue';
  import ConfirmationDialog from '@/components/Modal/ConfirmAction.vue';
  import { useFormStore } from '@/components/Form';
  import { useThemeToken } from '@/utils/hooks';
  import { useBusinessUnitStore } from '@/store';

  const token = useThemeToken();

  const route = useRoute();

  const officeLocationStore = inject(officeLocationStoreSymbol)!;
  const { getOfficeLocation, getIsLoadingOfficeLocation } =
    storeToRefs(officeLocationStore);
  const buStore = useBusinessUnitStore();
  const officeLocation = computed(() => getOfficeLocation.value);
  const isLoading = computed(() => getIsLoadingOfficeLocation.value);
  const { setOfficeLocationId } = inject(officeLocationRoutingSymbol)!;

  const emit = defineEmits(['officeLocationDeleted']);

  const officeLocationNameFormStore = useFormStore(
    'editOfficeLocationNameForm',
  );
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
        tooltip: 'Click here to delete this office location',
        isLink: false,
      },
    ];
    if (!officeLocation.value) tempButtons[0].status = 'deactivated';
    return tempButtons;
  });

  const deleteOfficeLocation = async () => {
    if (!officeLocation.value) return;
    await officeLocationStore.delete(officeLocation.value?.id);
    emit('officeLocationDeleted');
    officeLocationStore.nullOfficeLocation();
    setOfficeLocationId(null);
  };
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
  <div v-if="officeLocation && officeLocation.id" class="panel">
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
        :is-editing-key="'isEditingOfficeLocationName'"
        :form-store="officeLocationNameFormStore"
        :has-edit-keys="true"
      >
        <OfficeLocationNameInputField
          :office-location-id="officeLocation?.id ?? -1"
          :form-store="officeLocationNameFormStore"
          :placeholder="officeLocation?.officeLocationName ?? ''"
          :default="officeLocation?.officeLocationName ?? ''"
        />
      </EditableTextField>
    </a-flex>
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
