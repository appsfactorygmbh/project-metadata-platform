<template>
  <div>
    <div v-if="!loading" class="container" :class="{ blur: selectedGroup }">
      <div v-for="plugin in groupedPlugins" :key="plugin.id" class="plugins">
        <PluginComponent
          v-if="!plugin.isGroup"
          :id="Number(plugin.id)"
          :plugin-name="plugin.pluginName"
          :display-name="plugin.displayName"
          :url="plugin.url"
          :is-loading="loading"
          :is-editing="isEditing"
          :edit-key="plugin.editKey"
          :is-deleted="false"
        ></PluginComponent>

        <GroupedCard
          v-else
          :plugin-count="plugin.plugins.length"
          :display-name="plugin.displayName"
          :favicon-url="plugin.faviconUrl"
          @open="openGroupPopup(plugin)"
        />
      </div>
      <AddPluginCard v-if="isEditing"></AddPluginCard>
    </div>

    <!-- Placeholder for loading skeleton -->
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

    <transition name="fade-popup">
      <Popup
        v-if="selectedGroup"
        :selected-group="selectedGroup"
        :loading="loading"
        :is-editing="isEditing"
        @close="closeGroupPopup"
      />
    </transition>
  </div>
</template>

<script setup lang="ts">
  import { computed, inject, onMounted, ref, toRaw, watch } from 'vue';
  import type { ComputedRef } from 'vue';
  import { PluginComponent } from '@/components/Plugin';
  import { AddPluginCard } from '@/views/ProjectView/ProjectPlugins/AddPlugin';
  import { projectEditStoreSymbol } from '@/store/injectionSymbols';
  import { useEditing } from '@/utils/hooks/useEditing';
  import type { PluginEditModel, PluginModel } from '@/models/Plugin';
  import { usePluginStore, useProjectStore } from '@/store';
  import { createFaviconURL, cutAfterTLD } from '@/components/Plugin/editURL';
  import GroupedCard from '@/components/GroupedCard/GroupedCard.vue';
  import Popup from '@/components/Popup/PopupComponent.vue';

  const { isEditing } = useEditing();

  const pluginStore = usePluginStore();
  const projectStore = useProjectStore();
  const projectEditStore = inject(projectEditStoreSymbol);

  const plugins = ref<PluginEditModel[]>([]);
  const loading = computed(
    () => pluginStore.getIsLoading || projectStore.getIsLoading,
  );

  interface GroupedPlugin {
    id: string | number;
    pluginName: string;
    displayName: string;
    plugins: PluginEditModel[];
    isGroup: boolean;
    faviconUrl: string;
    url: string;
    editKey: number;
  }

  // groups plugin of same kind when they are more than 3
  const groupThreshold = parseInt(import.meta.env.VITE_GROUP_THRESHOLD) || 3; // limit for grouping
  const groupedPlugins = computed(() => {
    const groups: Record<string, PluginEditModel[]> = {};
    plugins.value.forEach((plugin: PluginEditModel) => {
      const pluginName = plugin.pluginName;
      if (!groups[pluginName]) {
        groups[pluginName] = [];
      }
      groups[pluginName].push(plugin);
    });

    const result: GroupedPlugin[] = [];

    Object.keys(groups).forEach((pluginName: string) => {
      const group = groups[pluginName];
      if (group.length >= groupThreshold) {
        const firstPluginUrl = group[0].url;
        result.push({
          id: `group-${pluginName}`, // ID of the group
          pluginName: pluginName, // name of the plugin
          displayName: pluginName, // type of plugin
          plugins: group, // list of plugins in the group
          isGroup: true, // flags that it's a group
          faviconUrl: createFaviconURL(cutAfterTLD(firstPluginUrl)),
        } as GroupedPlugin);
      } else {
        result.push(
          ...group.map((plugin) => ({
            id: plugin.id,
            pluginName: plugin.pluginName,
            displayName: plugin.displayName,
            plugins: [],
            isGroup: false,
            faviconUrl: '',
            url: plugin.url,
            editKey: plugin.editKey,
          })),
        );
      }
    });
    return result;
  });

  // selected group for popup
  const selectedGroup = ref<GroupedPlugin | null>(null);

  function openGroupPopup(pluginGroup: GroupedPlugin) {
    selectedGroup.value = pluginGroup;
    // Delay adding the event listener to prevent immediate closing due to initial click
    setTimeout(() => {
      document.addEventListener('click', handleOutsideClick);
    }, 0);
  }

  function closeGroupPopup() {
    selectedGroup.value = null;
    document.removeEventListener('click', handleOutsideClick);
  }

  function handleOutsideClick(event: Event) {
    const popupElement = document.querySelector('.popup');
    if (
      popupElement &&
      !popupElement.contains(event.target as HTMLInputElement)
    ) {
      closeGroupPopup();
    }
  }

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
        syncEditStore(pluginStore.getUnarchivedPlugins);
      }
    },
  );

  onMounted(async () => {
    const data: ComputedRef<PluginModel[]> = computed(() =>
      projectStore?.getProject?.isArchived
        ? pluginStore.getPlugins
        : pluginStore.getUnarchivedPlugins,
    );
    setPlugins(data.value);

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
    box-shadow: rgba(100, 100, 111, 0.2) 0 7px 29px 0 !important;
    display: flex;
    flex-direction: column;
    transition: 0.1s ease-in-out;
  }
  .blur {
    filter: blur(5px);
    pointer-events: none;
  }
</style>
