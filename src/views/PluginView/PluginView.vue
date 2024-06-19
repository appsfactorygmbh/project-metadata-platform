<template>
  <div class="container">
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

  onBeforeMount(async () => {
    await pluginStore?.fetchPlugins(props.projectID);
  });

  const plugins = computed(() => toRaw(pluginStore?.getPlugins));
  const loading = ref(pluginStore?.isLoading);
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
</style>
