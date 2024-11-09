<template>
  <FloatingButton :button="addButton" />

  <a-list
    class="plugin-list"
    item-layout="horizontal"
    :data-source="[
      ...(globalPluginsStore?.getGlobalPlugins.filter(
        (item) => !item.isArchived,
      ) || []),
    ]"
    :loading="isLoading"
    bordered
  >
    <template #renderItem="{ item }">
      <a-list-item class="list-items">
        <a-list-item-meta>
          <template #title>
            <div class="list-item">
              <div class="title">
                {{ item.name }}
              </div>
              <div class="buttons">
                <a-button
                  style="margin-right: 1em"
                  @click="handleEdit(item.id)"
                >
                  <EditOutlined />
                </a-button>
                <a-button
                  :loading="isButtonLoading(item.id)"
                  :disabled="isButtonLoading(item.id)"
                  @click="showDialog(item.id)"
                >
                  <DeleteOutlined />
                </a-button>
              </div>
            </div>
          </template>
        </a-list-item-meta>
      </a-list-item>
    </template>
  </a-list>
  <a-alert
    v-if="deleteError()"
    class="error-alert"
    message="Failed to delete global plugin"
    type="error"
    show-icon
  />
  <RouterView />
  <ConfirmationDialog
    :is-open="isDialogOpen"
    title="Delete confirm"
    message="Are you sure you want to delete the plugin?"
    @confirm="handleConfirm"
    @cancel="handleCancel"
    @update:is-open="isDialogOpen = $event"
  />
</template>

<script lang="ts" setup>
  import {
    DeleteOutlined,
    EditOutlined,
    PlusOutlined,
  } from '@ant-design/icons-vue';
  import type { FloatButtonModel } from '@/components/Button';
  import { globalPluginStoreSymbol } from '@/store/injectionSymbols';
  import { inject, onBeforeMount } from 'vue';
  import { useRouter } from 'vue-router';
  import { message } from 'ant-design-vue';
  import ConfirmationDialog from '@/components/Modal/ConfirmAction.vue';

  const globalPluginsStore = inject(globalPluginStoreSymbol);

  const isLoading = computed(
    () => globalPluginsStore?.getIsLoadingGlobalPlugins,
  );
  const isDeleting = computed(
    () => globalPluginsStore?.getIsLoadingDelete || false,
  );
  const removedSuccessfully = computed(
    () => globalPluginsStore?.getRemovedSuccessfully || false,
  );

  onBeforeMount(async () => {
    await globalPluginsStore?.fetchAll();
  });

  const router = useRouter();

  const addButton: FloatButtonModel = {
    name: 'CreatePluginButton',
    onClick: () => {
      router.push('/settings/global-plugins/create');
    },
    icon: PlusOutlined,
    status: 'activated',
    tooltip: 'Click here to create a new global plugin',
  };

  const handleEdit = (pluginId: number) => {
    router.push({ path: '/settings/global-plugins/edit', query: { pluginId } });
  };

  //stores the plugins, that get deleted at the time
  const pluginDeleting = ref<Array<number>>([]);

  // Dialog state and functions
  const isDialogOpen = ref(false);
  const pluginIdToDelete = ref<number | null>(null);

  /**
   * Shows the confirmation dialog for deleting a plugin.
   * @param {number} pluginId - The ID of the plugin to be deleted.
   */
  const showDialog = (pluginId: number) => {
    pluginIdToDelete.value = pluginId;
    isDialogOpen.value = true;
  };

  /**
   * Adds the plugin to the deleting plugins, deletes the plugin and removes it again
   * @param pluginId Id of the plugin that should be deleted
   */
  const handleDelete = async (pluginId: number) => {
    pluginDeleting.value.push(pluginId);
    await globalPluginsStore?.delete(pluginId);
    const index: number = pluginDeleting.value?.indexOf(pluginId);
    pluginDeleting.value.splice(index, 1);
  };

  /**
   * Handles the confirmation action for deleting a plugin.
   * If the plugin ID is valid, it deletes the plugin, updates the state,
   * and shows a success message.
   */
  const handleConfirm = async () => {
    if (pluginIdToDelete.value !== null) {
      await handleDelete(pluginIdToDelete.value);
      isDialogOpen.value = false;
      message.success('The Plugin has been deleted', 2);
    }
  };
  /**
   * Handles the cancel action for the delete confirmation dialog.
   * Closes the dialog and shows an information message.
   */
  const handleCancel = () => {
    isDialogOpen.value = false;
    message.info('Delete plugin was canceled', 2);
  };

  /**
   * Links the deleting status witch the comparison betwenn the plugin that
   * is deleted and the corresponding plugin to the button
   * @param itemId Id of the plugin, that belongs to the button
   * @returns True, if the store is deleting and the item id is one of the deleting plugins
   */
  const isButtonLoading = (itemId: number): boolean => {
    return isDeleting.value && pluginDeleting.value.includes(itemId);
  };

  const deleteError = (): boolean => {
    return !isDeleting.value && !removedSuccessfully.value;
  };
</script>

<style scoped>
  .plugin-list {
    margin: 1em;
    background-color: white;
    border: none;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  }

  .list-items {
    padding-top: 0.5em;
  }

  .list-item {
    display: flex;
  }

  .buttons {
    margin-left: auto;
  }

  .error-alert {
    width: max-content;
    margin: 0 auto;
  }
</style>
