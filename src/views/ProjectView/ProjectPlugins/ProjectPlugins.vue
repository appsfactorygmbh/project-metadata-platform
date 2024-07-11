<template>
  <div>
    <div v-if="!loading" class="container">
      <PluginComponent
        v-for="plugin in pluginStore.getCachePlugins"
        :id="plugin.id"
        :key="plugin.displayName"
        class="plugins"
        :plugin-name="plugin.pluginName"
        :display-name="plugin.displayName"
        :url="plugin.url"
        :is-loading="loading"
        :is-editing="isEditing"
      ></PluginComponent>
      <AddPluginComponent
        v-if="isEditing"
        style="height: 100%"
      ></AddPluginComponent>
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
  import { computed, toRaw, inject, onMounted } from 'vue';
  import PluginComponent from '@/components/Plugin/PluginComponent.vue';
  import {
    pluginStoreSymbol,
    projectsStoreSymbol,
  } from '@/store/injectionSymbols';
  import { useEditing } from '@/utils/hooks/useEditing';
  const { isEditing } = useEditing();

  const pluginStore = inject(pluginStoreSymbol)!;
  const projectsStore = inject(projectsStoreSymbol);

  const loading = computed(
    () => pluginStore.getIsLoading || projectsStore?.getIsLoading,
  );

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
