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
                <a-button>
                  <DeleteOutlined />
                </a-button>
              </div>
            </div>
          </template>
        </a-list-item-meta>
      </a-list-item>
    </template>
  </a-list>
</template>
<script lang="ts" setup>
  import {
    EditOutlined,
    DeleteOutlined,
    PlusOutlined,
  } from '@ant-design/icons-vue';
  import type { FloatButtonModel } from '@/components/Button';
  import { pluginStoreSymbol } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';
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
    : inject(pluginStoreSymbol)!;

  const { getIsLoading } = storeToRefs(pluginStore);
  const isLoading = computed(() => getIsLoading.value);

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
</style>
