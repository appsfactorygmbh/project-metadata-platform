<template>
  <div class="container">
    <div v-if="!loading">
      <PluginComponent
        v-for="plugin in plugins"
        :key="plugin.displayName"
        class="plugins"
        :plugin-name="plugin.pluginName"
        :display-name="plugin.displayName"
        :url="plugin.url"
        :is-loading="loading"
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
  import { onBeforeMount, computed, toRaw, inject } from 'vue';
  import PluginComponent from '../../components/Plugin/PluginComponent.vue';
  import { pluginStoreSymbol } from '@/store/Plugin/injectionsSymbols';

  const pluginStore = inject(pluginStoreSymbol);

  const props = defineProps({
    projectID: {
      type: Number,
      required: true,
    },
  });

  const plugins = computed(() => toRaw(pluginStore?.getPlugins));
  const loading = computed(() => pluginStore?.getIsLoading);

  onBeforeMount(async () => {
    pluginStore?.setLoading(true);
    await pluginStore?.fetchPlugins(props.projectID);
  });
</script>

<style scoped lang="css">
  /* Styling for the container */
  .container {
    width: 100%;
    height: auto;
    display: flex;
    justify-content: left;
    align-items: center;
    flex-direction: row;
    flex-wrap: wrap;
  }
  /* Styling for each plugin in container */
  .plugins {
    margin: 10px;
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
