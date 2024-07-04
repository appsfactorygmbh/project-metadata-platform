<template>
  <div>
    <div v-if="!loading" class="container">
      <PluginComponent
        v-for="plugin in plugins"
        ref="itemRefs"
        :key="plugin.displayName"
        class="plugins"
        :plugin-name="plugin.pluginName"
        :display-name="plugin.displayName"
        :url="plugin.url"
        :id="plugin.id"
        :is-loading="loading"
        :is-editing="isEditing"
        @hide="() => deletePlugin(plugin.displayName)"
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
  import {
    onBeforeMount,
    computed,
    toRaw,
    inject,
    onMounted,
    defineExpose,
  } from 'vue';
  import PluginComponent from '@/components/Plugin/PluginComponent.vue';
  import { pluginStoreSymbol } from '@/store/injectionSymbols';
  import type { PluginModel } from '@/models/Plugin';
  import type { ComputedRef } from 'vue';
  import { useEditing } from '@/utils/hooks/useEditing';

  //Placeholder
  const deletePlugin = (pluginName: string) => {
    console.log(pluginName);
  };

  const { isEditing } = useEditing();

  const itemRefs = ref<InstanceType<typeof PluginComponent>[]>([]);
  const showPlugins = () => {
    for (let i = 0; i < itemRefs.value.length; i++) {
      itemRefs.value[i].resetHide();
    }
  };

  const getUpdatedPlugins = (): PluginModel[] => {
    let allPlugins: PluginModel[] = [];
    for (let i = 0; i < itemRefs.value.length; i++) {
      allPlugins.push(itemRefs.value[i].getUpdatedPluginData());
    }
    return allPlugins;
  };

  const pluginStore = inject(pluginStoreSymbol)!;

  const props = defineProps({
    projectID: {
      type: Number,
      required: true,
    },
  });

  let plugins: ComputedRef<PluginModel[]>;
  const loading = computed(() => pluginStore.getIsLoading);

  function setPlugins(newPlugins: PluginModel[]) {
    plugins = computed(() => toRaw(newPlugins));
  }

  onBeforeMount(async () => {
    pluginStore.setLoading(true);
    await pluginStore.fetchPlugins(props.projectID);
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

  defineExpose({
    showPlugins,
    getUpdatedPlugins,
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
