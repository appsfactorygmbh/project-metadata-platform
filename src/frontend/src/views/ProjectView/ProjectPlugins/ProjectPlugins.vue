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
          :plugin-permissions="plugin.pluginPermissions"
          :billing-permissions="plugin.billingPermissions"
          :is-add-billing-modal-open="
            isBillingModalOpen && activePluginId === plugin.id
          "
          @open-create-billing="openBillingModal"
        />

        <GroupedCard
          v-else
          :plugin-count="plugin.plugins.length"
          :display-name="plugin.displayName"
          :favicon-url="plugin.faviconUrl"
          @open="openGroupPopup(plugin)"
        />
      </div>
      <AddPluginCard
        v-if="
          !isEditing &&
          pluginStore.getPermissions.includes(ResourceActions.Create)
        "
      />
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
      <a-skeleton active />
    </a-card>
    <AddBillingModal
      v-if="activePluginId !== null"
      :is-open="isBillingModalOpen"
      :plugin-name="getActivePluginName()"
      :plugin-id="activePluginId"
      :project-id="projectStore.getProject?.id ?? 0"
      @update:is-open="isBillingModalOpen = $event"
      @added-billing="handleBillingCreated"
      @cancel="handleBillingCancel"
    />
    <transition name="fade-popup">
      <Popup
        v-if="selectedGroup"
        :selected-group="selectedGroup"
        :loading="loading"
        :is-editing="isEditing"
        :is-billing-modal-open="isBillingModalOpen"
        :active-plugin-id="activePluginId"
        @close="closeGroupPopup"
        @open-create-billing="openBillingModal"
      />
    </transition>
  </div>
</template>

<script setup lang="ts">
  import { computed, ref, watch } from 'vue';
  import { PluginComponent } from '@/components/Plugin';
  import { AddPluginCard } from '@/views/ProjectView/ProjectPlugins/AddPlugin';
  import { useEditing } from '@/utils/hooks/useEditing';
  import type { PluginModel } from '@/models/Plugin';
  import { usePluginStore, useProjectStore } from '@/store';
  import { createFaviconURL, cutAfterTLD } from '@/components/Plugin/editURL';
  import GroupedCard from '@/components/GroupedCard/GroupedCard.vue';
  import Popup from '@/components/Popup/PopupComponent.vue';
  import { ResourceActions } from '@/models/utils';

  const { isEditing } = useEditing();

  const pluginStore = usePluginStore();
  const projectStore = useProjectStore();

  const emit = defineEmits(['setBlur']);

  const plugins = computed<PluginModel[]>(() =>
    projectStore?.getProject?.isArchived
      ? pluginStore.getPlugins
      : pluginStore.getUnarchivedPlugins,
  );
  const loading = computed(
    () => pluginStore.getIsLoading || projectStore.getIsLoading,
  );
  const isBillingModalOpen = ref(false);
  const activePluginId = ref<number | null>(null);

  const openBillingModal = (pluginId: number) => {
    activePluginId.value = pluginId;
    isBillingModalOpen.value = true;
  };
  const handleBillingCancel = () => {
    isBillingModalOpen.value = false;
    activePluginId.value = null;
  };
  const handleBillingCreated = async () => {
    isBillingModalOpen.value = false;
    activePluginId.value = null;
    pluginStore.fetch(projectStore.getProject?.id ?? 0);
    pluginStore.fetchUnarchived(projectStore.getProject?.id ?? 0);
  };

  const getActivePluginName = () => {
    const plugin = plugins.value.find(
      (plugin) => plugin.id == activePluginId.value,
    )!;

    return plugin?.displayName ?? plugin?.url;
  };

  interface GroupedPlugin {
    id: string | number;
    pluginName: string;
    displayName: string;
    plugins: PluginModel[];
    isGroup: boolean;
    faviconUrl: string;
    url: string;
    pluginPermissions?: ResourceActions[];
    billingPermissions?: ResourceActions[];
  }

  // groups plugin of same kind when they are more than 3
  const groupThreshold = parseInt(import.meta.env.VITE_GROUP_THRESHOLD) || 3; // limit for grouping
  const groupedPlugins = computed(() => {
    const groups: Record<string, PluginModel[]> = {};
    plugins.value.forEach((plugin: PluginModel) => {
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
            pluginPermissions: plugin.pluginPermissions ?? [],
            billingPermissions: plugin.billingPermissions ?? [],
          })),
        );
      }
    });
    return result;
  });

  // selected group id for popup
  const selectedGroupId = ref<string | number | null>(null);

  const selectedGroup = computed(() => {
    if (!selectedGroupId.value) return null;
    return (
      groupedPlugins.value.find((g) => g.id === selectedGroupId.value) || null
    );
  });

  watch(
    () => selectedGroup.value,
    (newGroup) => {
      if (!newGroup && selectedGroupId.value !== null) {
        closeGroupPopup();
      }
    },
  );
  function openGroupPopup(pluginGroup: GroupedPlugin) {
    selectedGroupId.value = pluginGroup.id;
    emit('setBlur', true);
    setTimeout(() => {
      document.addEventListener('click', handleOutsideClick);
    }, 0);
  }

  function closeGroupPopup() {
    selectedGroupId.value = null;
    emit('setBlur', false);
    document.removeEventListener('click', handleOutsideClick);
  }

  function handleOutsideClick(event: Event) {
    if (isBillingModalOpen.value) {
      return;
    }
    const popupElement = document.querySelector('.popup');
    const path = event.composedPath();
    if (popupElement && !path.includes(popupElement)) {
      closeGroupPopup();
    }
  }
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
