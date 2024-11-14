<template>
  <div>
    <div v-if="!loading" class="container" :class="{ blur: selectedGroup }">
      <div v-for="plugin in groupedPlugins" :key="plugin.id" class="plugins">
        <PluginComponent
          v-if="!plugin.isGroup"
          :id="plugin.id"
          :plugin-name="plugin.pluginName"
          :display-name="plugin.displayName"
          :url="plugin.url"
          :is-loading="loading"
          :is-editing="isEditing"
          :edit-key="plugin.editKey"
          :is-deleted="false"
        ></PluginComponent>

        <div v-else @click="openGroupPopup(plugin)">
          <a-badge :count="plugin.plugins.length">
            <a-card
              class="grouped-card"
              :body-style="{
            display: 'flex',
            flexDirection: 'row',
            alignItems: 'center',
            padding: '15px',
          }"
            >
              <!-- Display the favicon image. -->
              <a-avatar :src="plugin.faviconUrl" shape="square" class="avatar"></a-avatar>
              <div class="textContainer">
                <h3>{{ plugin.displayName + ' Plugins'}}</h3>
              </div>
            </a-card>
          </a-badge>
        </div>
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

    <!-- Popup to display plugins in the group -->
    <transition name="fade-popup">
      <div v-if="selectedGroup" class="popup">
        <a-card class="group-popup">
          <h3>Plugins in {{ selectedGroup.pluginName }}</h3>
          <div class="plugin-grid">
            <PluginComponent
              v-for="plugin in selectedGroup.plugins"
              :key="plugin.id"
              :id="plugin.id"
              :plugin-name="plugin.pluginName"
              :display-name="plugin.displayName"
              :url="plugin.url"
              :is-loading="loading"
              :is-editing="isEditing"
              :edit-key="plugin.editKey"
              :is-deleted="false"
            ></PluginComponent>
          </div>
          <a-button @click="closeGroupPopup" style="margin-top: 25px;">Close</a-button>
        </a-card>
      </div>
    </transition>
  </div>
</template>

<script setup lang="ts">
import { computed, inject, onMounted, ref, toRaw, watch } from 'vue';
import type { ComputedRef } from 'vue';
import { PluginComponent } from '@/components/Plugin';
import { AddPluginCard } from '@/views/ProjectView/ProjectPlugins/AddPlugin';
import {
  pluginStoreSymbol,
  projectEditStoreSymbol,
  projectsStoreSymbol,
} from '@/store/injectionSymbols';
import { useEditing } from '@/utils/hooks/useEditing';
import type { PluginEditModel, PluginModel } from '@/models/Plugin';
import { createFaviconURL, cutAfterTLD } from '@/components/Plugin/editURL';

const { isEditing } = useEditing();

const pluginStore = inject(pluginStoreSymbol)!;
const projectsStore = inject(projectsStoreSymbol);
const projectEditStore = inject(projectEditStoreSymbol);

const plugins = ref<PluginEditModel[]>([]);
const loading = computed(
  () => pluginStore.getIsLoading || projectsStore?.getIsLoading,
);

// groups plugin of same kind when they are more than 3
const groupThreshold = 3; // limit for grouping
const groupedPlugins = computed(() => {
  const groups = {};
  plugins.value.forEach((plugin) => {
    const type = plugin.pluginName;
    if (!groups[type]) {
      groups[type] = [];
    }
    groups[type].push(plugin);
  });

  const result = [];

  Object.keys(groups).forEach((type) => {
    const group = groups[type];
    if (group.length >= groupThreshold) {
      const firstPluginUrl = group[0].url;
      result.push({
        id: `group-${type}`, // ID of the group
        pluginName: type, // name of the plugin
        displayName: type, // type of plugin
        plugins: group, // list of plugins in the group
        isGroup: true, // flags that it's a group
        faviconUrl: createFaviconURL(cutAfterTLD(firstPluginUrl))
      });
    } else {
      result.push(...group);
    }
  });
  return result;
});

// selected group for popup
const selectedGroup = ref(null);

function openGroupPopup(pluginGroup) {
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

function handleOutsideClick(event) {
  const popupElement = document.querySelector('.popup');
  if (popupElement && !popupElement.contains(event.target)) {
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
  box-shadow: rgba(100, 100, 111, 0.2) 0 7px 29px 0 !important;
  display: flex;
  flex-direction: column;
  transition: 0.1s ease-in-out;
}
.grouped-card {
  width: max-content;
  min-width: 200px;
  max-width: 300px;
  height: 100px;
  box-shadow: rgba(100, 100, 111, 0.2) 0 7px 29px 0 !important;
  display: flex;
  flex-direction: column;
  transition: 0.1s ease-in-out;

  &:hover {
    cursor: pointer;
    transform: scale(1.01);
  }
}
.grouped-card .avatar {
  margin: 10px;
  width: 40px;
  height: auto;
  aspect-ratio: 1 / 1;
  object-fit: cover;
}
.grouped-card:hover {
  transform: scale(1.01);
}
.popup {
  position: absolute;
  width: 80%;
  margin-top: 10px;
  background: white;
  border-radius: 8px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
  opacity: 1;
  transition: opacity 0.3s ease-in-out;
}
.group-popup {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
}

.blur {
  filter: blur(5px);
  pointer-events: none;
}

.fade-popup-enter-active, .fade-popup-leave-active {
  transition: opacity 0.3s ease-in-out, transform 0.3s ease-in-out;
}
.fade-popup-enter-from, .fade-popup-leave-to {
  opacity: 0;
  transform: scale(0.95);
}

.textContainer {
  font-family: Manrope, serif;
  display: flex;
  flex-direction: column;
  justify-content: center;
  white-space: nowrap;
  overflow: hidden;

  & > * {
    margin: 10px;
  }

  & p {
    color: #6d6e6f;
    overflow: hidden;
    text-overflow: ellipsis;
  }
}
.plugin-grid {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 10px;
}
</style>
