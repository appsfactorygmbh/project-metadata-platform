<template>
  <div>
    <div v-if="!loading" class="container">
      <PluginComponent
        v-for="plugin in plugins"
        :id="plugin.id"
        :key="plugin.id"
        class="plugins"
        :plugin-name="plugin.pluginName"
        :display-name="plugin.displayName"
        :url="plugin.url"
        :is-loading="loading"
        :is-editing="isEditing"
        :edit-key="plugin.editKey"
        :is-deleted="false"
      ></PluginComponent>
      <AddPluginCard v-if="isEditing"></AddPluginCard>
    </div>
    <a-card
      v-else
      class="dummyCard"
      :bordered="false"
      :body-style="{
        display: 'flex',
        flexDirection: 'row',
        alignItems: 'center',
        padding: '15px',
      }"
    >
      <a-skeleton active></a-skeleton>
    </a-card>
  </div>
</template>

<script setup lang="ts">
  import { computed, inject, onMounted, ref, toRaw, watch } from 'vue';
  import type { ComputedRef } from 'vue';
  import { PluginComponent } from '@/components/Plugin';
  import { AddPluginCard } from '@/views/ProjectView/ProjectPlugins/AddPlugin';
  import {
    pluginStoreSymbol,
    projectEditStoreSymbol,
    projectsStoreSymbol,
  } from '@/store/injectionSymbols';
  import { useEditing } from '@/utils/hooks/useEditing';
  import type { PluginEditModel, PluginModel } from '@/models/Plugin';
  const { isEditing } = useEditing();

  const pluginStore = inject(pluginStoreSymbol)!;
  const projectsStore = inject(projectsStoreSymbol);
  const projectEditStore = inject(projectEditStoreSymbol);

  const plugins = ref<PluginEditModel[]>([]);
  const loading = computed(
    () => pluginStore.getIsLoading || projectsStore?.getIsLoading,
  );

  // take the normal Plugins initialAdd them to the projectEditStore and add the editKey to the return value of initial Add to the plugin
  const syncEditStore = (normalPlugins: PluginModel[]) => {
    for (let i = 0; i < normalPlugins.length; i++) {
      const index = projectEditStore?.initialAdd(normalPlugins[i]);
      if (index !== undefined) {
        plugins.value[i] = {
          ...normalPlugins[i],
          editKey: index,
          isDeleted: false,
        };
      }
    }
  };

  function setPlugins(newPlugins: PluginModel[]) {
    const normalPlugins = toRaw(newPlugins);
    projectEditStore?.resetPluginChanges();
    plugins.value = [];
    syncEditStore(normalPlugins);
  }

  watch(
    () => projectEditStore?.getAddedPlugins.length,
    (newVal) => {
      if (newVal && newVal > 0) {
        const newPlugin = projectEditStore?.getLastAddedPlugin;
        if (newPlugin) {
          plugins.value = [...plugins.value, newPlugin];
        }
      }
    },
  );

  watch(
    () => isEditing.value,
    (newVal) => {
      if (!newVal) {
        projectEditStore?.resetPluginChanges();
      } else {
        plugins.value = [];
        projectEditStore?.resetPluginChanges();
        syncEditStore(pluginStore.getPlugins);
      }
    },
  );

  onMounted(async () => {
    setPlugins(pluginStore.getPlugins);

    const data: ComputedRef<PluginModel[]> = computed(
      () => pluginStore.getPlugins,
    );

    watch(
      () => data.value,
      (newProject) => {
        setPlugins(newProject);
      },
    );
  });
</script>

<style scoped lang="css">
  /* Styling for the container */
  .container {
    width: 100%;
    height: auto;
    display: flex;
    justify-content: center;
    align-items: center;
    flex-direction: row;
    flex-wrap: wrap;
    margin-bottom: 10px;
    & > * {
      margin: 10px;
    }
  }
  .dummyCard {
    width: max-content;
    min-width: 200px;
    max-width: 100%;
    box-shadow: rgba(100, 100, 111, 0.2) 0px 7px 29px 0px !important;
    display: flex;
    flex-direction: column;
    transition: 0.1s ease-in-out;
  }
</style>
