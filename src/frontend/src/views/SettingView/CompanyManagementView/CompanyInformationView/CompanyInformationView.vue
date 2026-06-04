<script lang="ts" setup>
  import { DeleteOutlined } from '@ant-design/icons-vue';
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';
  import { inject, ref } from 'vue';
  import {
    companyRoutingSymbol,
    companyStoreSymbol,
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

  const companyStore = inject(companyStoreSymbol)!;
  const { getCompany, getIsLoadingCompany, getLinkedProjects } =
    storeToRefs(companyStore);
  const buStore = useBusinessUnitStore();
  const company = computed(() => getCompany.value);
  const linkedProjects = computed(() => getLinkedProjects.value);
  const isLoading = computed(() => getIsLoadingCompany.value);
  const { setCompanyId } = inject(companyRoutingSymbol)!;

  const emit = defineEmits(['companyDeleted']);

  const companyNameFormStore = useFormStore('editCompanyNameForm');
  onMounted(async () => {
    buStore.fetchAll();
  });

  const isConfirmModalOpen = ref<boolean>(false);
  const openModal = () => {
    if (linkedProjects.value.length > 0) {
      message.error(
        `Company is still linked to these projects: [${linkedProjects.value}]`,
        5,
      );
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
    ];
    if (!company.value) tempButtons[0].status = 'deactivated';
    return tempButtons;
  });

  const deleteCompany = async () => {
    if (!company.value) return;
    await companyStore.delete(company.value?.id);
    emit('companyDeleted');
    companyStore.nullCompany();
    setCompanyId(null);
  };
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
  <div v-if="company && company.id" class="panel">
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
        :is-editing-key="'isEditingCompanyName'"
        :form-store="companyNameFormStore"
        :has-edit-keys="true"
      >
        <CompanyNameInputField
          :company-id="company?.id ?? -1"
          :form-store="companyNameFormStore"
          :placeholder="company?.companyName ?? ''"
          :default="company?.companyName ?? ''"
        />
      </EditableTextField>
    </a-flex>
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
