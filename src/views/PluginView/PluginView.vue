<template>
  <div class="container">
    <PluginComponent
      v-for="plugin in plugins"
      :key="plugin.displayName"
      class="plugins"
      :plugin-name="plugin.pluginName"
      :display-name="plugin.displayName"
      :url="plugin.url"
    ></PluginComponent>
  </div>
</template>

<script setup lang="ts">
  import { onBeforeMount, computed, toRaw } from 'vue';
  import PluginComponent from '../../components/Plugin/PluginComponent.vue';
  import { usePluginsStore } from '../../store/Plugin/PluginStore.ts';

  const pluginStore = usePluginsStore();
  const props = defineProps({
    projectID: {
      type: String,
      required: true,
    },
  });

  onBeforeMount(async () => {
    await pluginStore.fetchPlugins(props.projectID);
    console.log('DAaaaaata', toRaw(pluginStore.getPlugins));
  });

  const plugins = computed(() => toRaw(pluginStore.getPlugins));
</script>

<style scoped lang="css">
  /* Styling for the container */
  .container {
    width: 100vw;
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
