<script lang="ts" setup>
  import {
    DeleteOutlined,
    RobotOutlined,
    RedoOutlined,
  } from '@ant-design/icons-vue';
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';
  import { inject, ref } from 'vue';
  import {
    apiTokenRoutingSymbol,
    apiTokenStoreSymbol,
  } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';

  import FloatingButtonGroup from '@/components/Button/FloatingButtonGroup.vue';
  import ConfirmationDialog from '@/components/Modal/ConfirmAction.vue';
  import { useThemeToken } from '@/utils/hooks';

  const token = useThemeToken();

  const route = useRoute();
  const apiTokenStore = inject(apiTokenStoreSymbol)!;
  const { setApiTokenId, routerApiTokenId } = inject(apiTokenRoutingSymbol)!;
  const {
    getIsLoadingApiToken,
    getIsLoading,
    getApiToken,
    getHasTokenValue,
    getTokenValue,
  } = storeToRefs(apiTokenStore);
  const apiToken = computed(() => getApiToken.value);

  const isLoading = computed(
    () => getIsLoadingApiToken.value || getIsLoading.value,
  );

  const isTokenModalVisible = ref(false);

  watch(getHasTokenValue, (newValue) => {
    if (newValue) {
      isTokenModalVisible.value = true;
    }
  });

  const isConfirmDeleteModalOpen = ref<boolean>(false);
  const openConfirmDeleteModal = () => {
    isConfirmDeleteModalOpen.value = true;
  };
  const closeConfirmDeleteModal = () => {
    isConfirmDeleteModalOpen.value = false;
  };

  const isConfirmRegenerateModalOpen = ref<boolean>(false);
  const openConfirmRegenerateModal = () => {
    isConfirmRegenerateModalOpen.value = true;
  };
  const closeConfirmRegenerateModal = () => {
    isConfirmRegenerateModalOpen.value = false;
  };

  onMounted(async () => {});

  //Button for adding new User and deleting User
  const buttons = computed((): FloatButtonModel[] => {
    const tempButtons: FloatButtonModel[] = [
      {
        name: 'DeleteApiTokenButton',
        onClick: () => {
          openConfirmDeleteModal();
        },
        icon: DeleteOutlined,
        type: 'primary',
        specialType: 'danger',
        size: 'large',
        status: 'activated',
        tooltip: 'Click here to delete this API-token',
        isLink: false,
      },
      {
        name: 'RegenerateApiTokenButton',
        onClick: () => {
          openConfirmRegenerateModal();
        },
        icon: RedoOutlined,
        type: 'primary',
        size: 'large',
        status: 'activated',
        tooltip: 'Click here to regnerate this API-token value',
        isLink: false,
      },
    ];
    if (!routerApiTokenId.value) {
      tempButtons[0].status = 'deactivated';
      tempButtons[1].status = 'deactivated';
    }

    return tempButtons;
  });

  const deleteApiToken = async () => {
    if (!apiToken.value) return;
    await apiTokenStore.delete(apiToken.value?.id);
    setApiTokenId(null);
    apiTokenStore.setApiToken(null);
  };
  const regenerateApiToken = async () => {
    if (!apiToken.value) return;
    await apiTokenStore.regenerate(apiToken.value?.id);
    await apiTokenStore.fetchApiToken(apiToken.value?.id);
  };
</script>
<template>
  <ConfirmationDialog
    :is-open="isConfirmDeleteModalOpen"
    title="Delete confirm"
    message="Are you sure you want to delete this token?"
    @confirm="deleteApiToken"
    @cancel="closeConfirmDeleteModal"
    @update:is-open="isConfirmDeleteModalOpen = $event"
  />
  <ConfirmationDialog
    :is-open="isConfirmRegenerateModalOpen"
    title="Regenerate confirm"
    message="Are you sure you want to regenerate this token?"
    @confirm="regenerateApiToken"
    @cancel="closeConfirmRegenerateModal"
    @update:is-open="isConfirmRegenerateModalOpen = $event"
  />
  <a-modal
    v-model:open="isTokenModalVisible"
    title="New Token Value generated!"
    :closable="false"
    :mask-closable="false"
  >
    <p>Copy your token now. You won't be able to see it again</p>
    <a-typography-paragraph class="token-value" copyable type="secondary">{{
      getTokenValue
    }}</a-typography-paragraph>
    <template #footer>
      <a-button
        type="primary"
        @click="
          {
            isTokenModalVisible = false;
            apiTokenStore.setTokenValue(null);
          }
        "
        >Ok</a-button
      >
    </template>
  </a-modal>
  <div v-if="apiToken?.id" class="panel">
    <a-flex class="avatar">
      <a-avatar :size="150">
        <template #icon>
          <RobotOutlined />
        </template>
      </a-avatar>
    </a-flex>

    <a-flex
      class="userInfoBox"
      :body-style="{
        height: 'fit-content',
      }"
    >
      <EditableTextField
        class="textField name"
        :value="apiToken?.name ?? ''"
        :is-loading="isLoading"
        :label="'Name'"
        :has-edit-keys="false"
      >
      </EditableTextField>
      <EditableTagList
        v-if="(apiToken?.scopes ?? []).length > 0"
        class="textField scopes"
        :values="apiToken?.scopes ?? []"
        :is-loading="isLoading"
        :label="'Scopes'"
        :has-edit-keys="false"
      >
      </EditableTagList>
      <EditableTextField
        class="textField expiration"
        :value="apiToken?.expirationDate.toLocaleDateString() ?? ''"
        :is-loading="isLoading"
        :label="'Expiration Date'"
        :has-edit-keys="false"
      >
      </EditableTextField>
    </a-flex>
  </div>
  <a-empty
    v-else-if="route.query.userId"
    :description="`No Token Found for Id ${route.query.apiTokenId}`"
  ></a-empty>
  <a-empty v-else description="No API-Token Selected"></a-empty>
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

  .token-value {
    font-family: monospace;
    word-break: break-all;
  }
</style>
