<template>
  <FloatingButton
    v-if="globalPluginsStore.getPermissions.includes(ResourceActions.Create)"
    :button="addButton"
  />

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
    <template #renderItem="{ item }: { item: GlobalPluginModel }">
      <a-list-item class="list-items">
        <a-list-item-meta>
          <template #title>
            <div class="list-item">
              <div class="title">
                {{ item.pluginName }}
              </div>
              <div class="baseUrl">
                {{ item.baseUrl }}
              </div>
              <div v-if="filterType" class="buttons">
                <a-button
                  v-if="item.permissions?.includes(ResourceActions.Edit)"
                  style="margin-right: 1em"
                  title="Edit Plugin"
                  @click="handleEdit(item.id)"
                >
                  <EditOutlined />
                </a-button>
              </div>
              <div v-else class="buttons">
                <a-button
                  v-if="item.permissions?.includes(ResourceActions.Edit)"
                  style="margin-right: 1em"
                  title="Reactivate Plugin"
                  @click="handleReactivate(item.id)"
                >
                  <UndoOutlined />
                </a-button>
                <a-button
                  v-if="item.permissions?.includes(ResourceActions.Delete)"
                  :loading="isButtonLoading(item.id)"
                  :disabled="isButtonLoading(item.id)"
                  title="Delete Plugin"
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
  <RouterView />
  <ConfirmationDialog
    :is-open="isDialogOpen"
    title="Delete confirm"
    message="Are you sure you want to delete this Plugin?"
    @confirm="handleDelete"
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
  import { onBeforeMount } from 'vue';
  import { useRouter } from 'vue-router';
  import { App } from 'ant-design-vue';
  import ConfirmationDialog from '@/components/Modal/ConfirmAction.vue';
  import { useGlobalPluginStore } from '@/store';
  import { useToggle } from '@vueuse/core';
  import type { GlobalPluginModel } from '@/models/GlobalPlugin';
  import { useThemeToken } from '@/utils/hooks';
  import { ResourceActions } from '@/models/utils';

  const token = useThemeToken();
  const { notification } = App.useApp();
  const globalPluginsStore = useGlobalPluginStore();

  const isLoading = computed(
    () => globalPluginsStore?.getIsLoadingGlobalPlugins,
  );
  const isDeleting = computed(
    () => globalPluginsStore?.getIsLoadingDelete || false,
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

  const handleDelete = async () => {
    if (!pluginIdToDelete.value) return;
    try {
      pluginDeleting.value.push(pluginIdToDelete.value);
      await globalPluginsStore?.delete(pluginIdToDelete.value);
      notification.success({
        message: 'Success!',
        description: 'Plugin deleted successfully.',
      });
    } catch (error) {
      notification.error({
        message: 'Error!',
        description: (error as Error).message ?? 'An error occurred.',
      });
      console.error('Error deleting global plugin: ' + error);
    } finally {
      const index: number = pluginDeleting.value?.indexOf(
        pluginIdToDelete.value,
      );
      pluginDeleting.value.splice(index, 1);
      globalPluginsStore.fetchAll();
    }
  };

  const handleReactivate = async (pluginId: GlobalPluginModel['id']) => {
    try {
      await globalPluginsStore?.unarchive(pluginId);
      notification.success({
        message: 'Success!',
        description: 'Plugin reactivated successfully.',
      });
    } catch (error) {
      notification.error({
        message: 'Error!',
        description: (error as Error).message ?? 'An error occurred.',
      });
    }
  };

  /**
   * Handles the cancel action for the delete confirmation dialog.
   * Closes the dialog and shows an information message.
   */
  const handleCancel = () => {
    isDialogOpen.value = false;
    notification.warning({
      message: 'Canceled!',
      description: 'Delete plugin was canceled.',
    });
  };

  /**
   * Links the deleting status witch the comparison betwenn the plugin that
   * is deleted and the corresponding plugin to the button++
   * @param itemId Id of the plugin, that belongs to the button
   * @returns True, if the store is deleting and the item id is one of the deleting plugins
   */
  const isButtonLoading = (itemId: number): boolean => {
    return isDeleting.value && pluginDeleting.value.includes(itemId);
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
    background-color: v-bind('token.colorBgElevated');
    border: none;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    overflow-y: auto;
    max-height: 95%;
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

  .text {
    color: v-bind('token.colorText');
  }

  .baseUrl {
    font-weight: normal;
    margin-left: 10px;
  }
</style>
