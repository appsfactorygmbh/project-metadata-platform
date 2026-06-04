<script lang="ts" setup>
  import { DeleteOutlined } from '@ant-design/icons-vue';
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';
  import { inject, ref } from 'vue';
  import {
    businessUnitRoutingSymbol,
    businessUnitStoreSymbol,
  } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';

  import FloatingButtonGroup from '@/components/Button/FloatingButtonGroup.vue';
  import ConfirmationDialog from '@/components/Modal/ConfirmAction.vue';
  import { useFormStore } from '@/components/Form';
  import { useThemeToken } from '@/utils/hooks';
  import { message } from 'ant-design-vue';
  import { useBusinessUnitStore } from '@/store';

  const token = useThemeToken();

  const route = useRoute();

  const businessUnitStore = inject(businessUnitStoreSymbol)!;
  const { getBusinessUnit, getIsLoadingBusinessUnit, getLinkedTeams } =
    storeToRefs(businessUnitStore);
  const buStore = useBusinessUnitStore();
  const businessUnit = computed(() => getBusinessUnit.value);
  const linkedTeams = computed(() => getLinkedTeams.value);
  const isLoading = computed(() => getIsLoadingBusinessUnit.value);
  const { setBusinessUnitId } = inject(businessUnitRoutingSymbol)!;

  const emit = defineEmits(['businessUnitDeleted']);

  const businessUnitNameFormStore = useFormStore('editBusinessUnitNameForm');
  onMounted(async () => {
    buStore.fetchAll();
  });

  const isConfirmModalOpen = ref<boolean>(false);
  const openModal = () => {
    if (linkedTeams.value.length > 0) {
      message.error(
        `BusinessUnit is still linked to these teams: [${linkedTeams.value}]`,
        5,
      );
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
    ];
    if (!businessUnit.value) tempButtons[0].status = 'deactivated';
    return tempButtons;
  });

  const deleteBusinessUnit = async () => {
    if (!businessUnit.value) return;
    await businessUnitStore.delete(businessUnit.value?.id);
    emit('businessUnitDeleted');
    businessUnitStore.nullBusinessUnit();
    setBusinessUnitId(null);
  };
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
  <div v-if="businessUnit && businessUnit.id" class="panel">
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
        :is-editing-key="'isEditingBusinessUnitName'"
        :form-store="businessUnitNameFormStore"
        :has-edit-keys="true"
      >
        <BusinessUnitNameInputField
          :business-unit-id="businessUnit?.id ?? -1"
          :form-store="businessUnitNameFormStore"
          :placeholder="businessUnit?.businessUnitName ?? ''"
          :default="businessUnit?.businessUnitName ?? ''"
        />
      </EditableTextField>
    </a-flex>
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
