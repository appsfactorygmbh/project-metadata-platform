<template>
  <FloatingButton :button="button" />

  <a-list
    class="plugin-list"
    item-layout="horizontal"
    :data-source="[
      ...(pluginStore?.getGlobalPlugins.filter((item) => !item.archieved) ||
        []),
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
                <a-button style="margin-right: 1em">
                  <EditOutlined />
                </a-button>
                <a-button
                  :loading="isButtonLoading(item.id)"
                  :disabled="isButtonLoading(item.id)"
                  @click="handleDelete(item.id)"
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
    v-if="fetchError()"
    class="error-alert"
    message="Failed to delete global plugin"
    type="error"
    show-icon
  />
</template>
<script lang="ts" setup>
  import {
    EditOutlined,
    DeleteOutlined,
    PlusOutlined,
  } from '@ant-design/icons-vue';
  import type { FloatButtonModel } from '@/components/Button';
  import { pluginStoreSymbol } from '@/store/injectionSymbols';
  import { inject, onBeforeMount } from 'vue';
  import { usePluginsStore } from '@/store';

  const props = defineProps({
    isTest: {
      type: Boolean,
      default: false,
    },
  });

  const pluginStore = props.isTest
    ? usePluginsStore()
    : inject(pluginStoreSymbol);

  console.log(pluginStore);

  const isLoading = computed(() => pluginStore?.getIsLoading);
  const isDeleting = computed(() => pluginStore?.getIsLoadingDelete || false);
  const removedSuccessfully = computed(
    () => pluginStore?.getRemovedSuccessfully || false,
  );

  onBeforeMount(async () => {
    await pluginStore?.fetchGlobalPlugins();
  });

  const button: FloatButtonModel = {
    name: 'CreatePluginButton',
    onClick: () => {},
    icon: PlusOutlined,
    status: 'activated',
    tooltip: 'Click here to create a new global plugin',
  };

  //stores the plugins, that get deleted at the time
  const pluginDeleting = ref<Array<number>>([]);

  /**
   * Adds the plugin to the deleting plugins, deletes the plugin and removes it again
   * @param pluginId Id of the plugin that should be deleted
   */
  const handleDelete = async (pluginId: number) => {
    pluginDeleting.value.push(pluginId);
    await pluginStore?.deleteGlobalPlugin(pluginId);
    const index: number = pluginDeleting.value?.indexOf(pluginId);
    pluginDeleting.value.splice(index, 1);
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

  const fetchError = (): boolean => {
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
