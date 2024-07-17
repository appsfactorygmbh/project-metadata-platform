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
  import { ref, computed, inject, onMounted, toRaw, watch } from 'vue';
  import type { ComputedRef } from 'vue';
  import PluginComponent from '@/components/Plugin/PluginComponent.vue';
  import AddPluginCard from '@/views/PluginView/AddPlugin/AddPluginCard.vue';
  import {
    pluginStoreSymbol,
    projectsStoreSymbol,
    projectEditStoreSymbol,
  } from '@/store/injectionSymbols';
  import { useEditing } from '@/utils/hooks/useEditing';
  import type { PluginModel, PluginEditModel } from '@/models/Plugin';
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
    projectEditStore?.resetChanges();
    plugins.value = [];
    syncEditStore(normalPlugins);
  }

  watch(
    () => isEditing.value,
    (newVal) => {
      if (!newVal) {
        projectEditStore?.resetChanges();
      } else {
        plugins.value = [];
        projectEditStore?.resetChanges();
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
