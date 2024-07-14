<template>
  <button @click="getMap">GET Map</button>
  <div>
    <div v-if="!loading" class="container">
      <PluginComponent
        v-for="plugin in plugins"
        :id="plugin.id"
        :key="plugin.displayName"
        class="plugins"
        :plugin-name="plugin.pluginName"
        :display-name="plugin.displayName"
        :url="plugin.url"
        :is-loading="loading"
        :is-editing="isEditing"
        :edit-key="plugin.editKey"
        :is-deleted="false"
      ></PluginComponent>
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
  import { computed, inject, onMounted, toRaw, onBeforeMount } from 'vue';
  import type { ComputedRef } from 'vue';
  import PluginComponent from '@/components/Plugin/PluginComponent.vue';
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

  const getMap = () => {
    pluginStore?.setPlugins([
      ...pluginStore.getPlugins,
      {
        pluginName: 'Map',
        displayName: 'Map',
        url: 'https://www.google.com/maps',
        id: 100,
      },
    ]);
    console.log('update: ', projectEditStore?.pluginChanges);
  };

  let plugins: ComputedRef<PluginModel[]>;
  const loading = computed(
    () => pluginStore.getIsLoading || projectsStore?.getIsLoading,
  );

  function setPlugins(newPlugins: PluginModel[]) {
    plugins = computed(() => toRaw(newPlugins));
  }

  watch(isEditing, (newVal) => {
    if (newVal) {
      for (let i = 0; i < plugins.value.length; i++) {
        const index = projectEditStore?.initialAdd(plugins.value[i]);
        plugins.value[i] = {
          ...plugins.value[i],
          editKey: index,
          isDeleted: false,
        };
      }
    }
  });

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

  onMounted(() => {
    plugins = pluginStore.getPlugins;
    if (projectEditStore) {
      const plugins: PluginModel[] = pluginStore.getPlugins;
      for (let i = 0; i < plugins.length; i++) {
        projectEditStore.initialAdd(plugins[i]);
      }
    }
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
