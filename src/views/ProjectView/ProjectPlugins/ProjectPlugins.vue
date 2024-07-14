<template>
  <button @click="getMap">GET Map</button>
  <button @click="addPlugin">addPLugin</button>
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
  import { ref, computed, inject, onMounted, toRaw, onBeforeMount, watch } from 'vue';
  import type { ComputedRef } from 'vue';
  import PluginComponent from '@/components/Plugin/PluginComponent.vue';
  import { pluginStoreSymbol, projectsStoreSymbol, projectEditStoreSymbol } from '@/store/injectionSymbols';
  import { useEditing } from '@/utils/hooks/useEditing';
  import type { PluginModel, PluginEditModel } from '@/models/Plugin';
  const { isEditing } = useEditing();

  const pluginStore = inject(pluginStoreSymbol)!;
  const projectsStore = inject(projectsStoreSymbol);
  const projectEditStore = inject(projectEditStoreSymbol);

  const getMap = () => {
    console.log('update: ', projectEditStore?.pluginChanges);
  };

  const plugins = ref<PluginModel[]>([]);
  const loading = computed(
    () => pluginStore.getIsLoading || projectsStore?.getIsLoading,
  );

  const addPlugin = () => {
    // Create a new plugin
    const index = projectEditStore?.initialAdd({
      id: 100,
      pluginName: '',
      displayName: '',
      url: '',
    });
    const newPlugin: PluginModel = {
      id: 100,
      pluginName: '',
      displayName: '',
      url: '',
      editKey: index,
      isDeleted: false,
    };
    pluginStore.setPlugins([...plugins.value, newPlugin]);
    //
    // // Add the new plugin to the projectEditStore and get the index
    // const index = projectEditStore?.initialAdd(newPlugin);
    //
    // // Check if index is defined before assigning it to editKey
    // if (index !== undefined) {
    //   // Create a new array that includes the new plugin
    //   const updatedPlugins = [
    //     ...plugins.value,
    //     {
    //       ...newPlugin,
    //       editKey: index,
    //       isDeleted: false,
    //     },
    //   ];
    //
    //   // Assign the new array to plugins.value
    //   plugins.value = updatedPlugins;
    // }
  };

  function setPlugins(newPlugins: PluginModel[]) {
    plugins.value = toRaw(newPlugins);
  }

  watch(
    () => isEditing.value,
    (newVal) => {
      if (!newVal) {
        projectEditStore?.resetChanges();
      } else {
        for (let i = 0; i < plugins.value.length; i++) {
          const index = projectEditStore?.initialAdd(plugins.value[i]);
          plugins.value[i] = {
            ...plugins.value[i],
            editKey: index,
            isDeleted: false,
          };
        }
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
