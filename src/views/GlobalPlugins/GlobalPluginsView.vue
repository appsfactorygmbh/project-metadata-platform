<template>
  <FloatingButton :button="addButton" />

  <a-tooltip
    placement="left"
    title="Click here to toggle between active and archived global plugins"
  >
    <a-button class="archiveButton" @click="toggleShowFilter">
      <template #icon>
        <InboxOutlined v-if="filterType" />
        <BulbOutlined v-else />
      </template>
    </a-button>
  </a-tooltip>

  <a-list
    class="plugin-list"
    item-layout="horizontal"
    :data-source="[
      ...(globalPluginsStore?.getGlobalPlugins.filter(
        (item) => !item.isArchived == filterType,
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
              <div v-if="filterType" class="buttons">
                <a-button
                  style="margin-right: 1em"
                  title="Edit Plugin"
                  @click="handleEdit(item.id)"
                >
                  <EditOutlined />
                </a-button>
                <a-button
                  :loading="isButtonLoading(item.id)"
                  :disabled="isButtonLoading(item.id)"
                  title="Archive Plugin"
                  @click="showDialog(item.id, 'archive')"
                >
                  <InboxOutlined />
                </a-button>
              </div>
              <div v-else class="buttons">
                <a-button
                  style="margin-right: 1em"
                  title="Reactivate Plugin"
                  @click="handleReactivate(item)"
                >
                  <UndoOutlined />
                </a-button>
                <a-button
                  :loading="isButtonLoading(item.id)"
                  :disabled="isButtonLoading(item.id)"
                  title="Delete Plugin"
                  @click="showDialog(item.id, 'delete')"
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
    :title="confirmAction.title"
    :message="confirmAction.message"
    @confirm="handleConfirm"
    @cancel="handleCancel"
    @update:is-open="isDialogOpen = $event"
  />
</template>

<script lang="ts" setup>
  import {
    BulbOutlined,
    DeleteOutlined,
    EditOutlined,
    InboxOutlined,
    PlusOutlined,
    UndoOutlined,
  } from '@ant-design/icons-vue';
  import type { FloatButtonModel } from '@/components/Button';
  import { globalPluginStoreSymbol } from '@/store/injectionSymbols';
  import { inject, onBeforeMount } from 'vue';
  import { useRouter } from 'vue-router';
  import { message } from 'ant-design-vue';
  import ConfirmationDialog from '@/components/Modal/ConfirmAction.vue';
  import { useToggle } from '@vueuse/core';
  import type { GlobalPluginModel } from '@/models/Plugin';

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
    await globalPluginsStore?.fetchGlobalPlugins();
  });

  const router = useRouter();

  const addButton: FloatButtonModel = {
    name: 'CreatePluginButton',
    onClick: () => {
      router.push('/settings/global-plugins/create');
    },
    icon: PlusOutlined,
    size: 'large',
    type: 'primary',
    status: 'activated',
    tooltip: 'Click here to create a new global plugin',
  };

  // filterType is true for active and false for archived
  const [filterType, toggleFilterType] = useToggle<true, false>(true, {
    truthyValue: true,
    falsyValue: false,
  });
  const toggleShowFilter = () => {
    toggleFilterType();
  };

  const handleEdit = (pluginId: number) => {
    router.push({ path: '/settings/global-plugins/edit', query: { pluginId } });
  };

  //stores the plugins, that get deleted at the time
  const pluginDeleting = ref<Array<number>>([]);

  type ConfirmActionModel = {
    type: string;
    title: string;
    message: string;
  };

  // Dialog state and functions
  const isDialogOpen = ref(false);
  const pluginIdToDelete = ref<number | null>(null);
  const confirmAction = ref<ConfirmActionModel>({
    type: '',
    title: '',
    message: '',
  });

  /**
   * Shows the confirmation dialog for deleting a plugin.
   * @param {number} pluginId - The ID of the plugin to be deleted.
   * @param {string} action - The action to be performed on the plugin.
   */
  const showDialog = (pluginId: number, action: string) => {
    toggleConfirmAction(action);
    pluginIdToDelete.value = pluginId;
    isDialogOpen.value = true;
  };

  const toggleConfirmAction = (action: string) => {
    confirmAction.value = {
      type: action,
      title: action === 'archive' ? 'Archive Plugin' : 'Delete Plugin',
      message:
        action === 'archive'
          ? 'Are you sure you want to archive this plugin?'
          : 'Are you sure you want to delete this plugin?',
    };
  };

  /**
   * Adds the plugin to the deleting plugins, deletes the plugin and removes it again
   * @param pluginId Id of the plugin that should be deleted
   */
  const handleArchive = async (pluginId: number) => {
    pluginDeleting.value.push(pluginId);

    const plugin = globalPluginsStore!.getGlobalPlugins.find(
      (plugin) => plugin.id === pluginId,
    );
    await globalPluginsStore?.archiveGlobalPlugin(plugin as GlobalPluginModel);

    const index: number = pluginDeleting.value?.indexOf(pluginId);
    pluginDeleting.value.splice(index, 1);
  };

  const handleDelete = async (pluginId: number) => {
    pluginDeleting.value.push(pluginId);

    await globalPluginsStore?.deleteGlobalPlugin(pluginId);

    const index: number = pluginDeleting.value?.indexOf(pluginId);
    pluginDeleting.value.splice(index, 1);
  };

  const handleReactivate = async (plugin: GlobalPluginModel) => {
    await globalPluginsStore?.reactivateGlobalPlugin(plugin);
    message.success('The plugin has been reactivated', 2);
  };

  /**
   * Handles the confirmation action for deleting a plugin.
   * If the plugin ID is valid, it deletes the plugin, updates the state,
   * and shows a success message.
   */
  const handleConfirm = async () => {
    if (pluginIdToDelete.value !== null) {
      let confirmMessage;
      if (confirmAction.value.type === 'archive') {
        await handleArchive(pluginIdToDelete.value);
        confirmMessage = 'The plugin has been archived';
      } else {
        await handleDelete(pluginIdToDelete.value);
        confirmMessage = 'The plugin has been deleted';
      }

      isDialogOpen.value = false;
      message.success(confirmMessage, 2);
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
  .archiveButton {
    width: 5%;
    position: absolute;
    top: 1.3em;
    right: 1.7em;
  }

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
