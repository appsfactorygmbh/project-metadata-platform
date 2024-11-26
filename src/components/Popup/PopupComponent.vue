<template>
  <transition name="fade-popup">
    <div v-if="selectedGroup" class="popup">
      <a-card class="group-popup">
        <h3>Plugins in {{ selectedGroup.pluginName }}</h3>
        <div class="plugin-grid">
          <PluginComponent
            v-for="plugin in selectedGroup.plugins"
            :id="plugin.id"
            :key="plugin.id"
            :display-name="plugin.displayName"
            :url="plugin.url"
            :is-loading="loading"
            :is-editing="isEditing"
            :edit-key="plugin.editKey"
            :is-deleted="false"
            :show-favicon="false"
          ></PluginComponent>
        </div>
        <a-button style="margin-top: 15px" @click="closePopup">Close</a-button>
      </a-card>
    </div>
  </transition>
</template>

<script setup lang="ts">
  import { defineEmits, defineProps } from 'vue';
  import { PluginComponent } from '@/components/Plugin';

  const { selectedGroup, loading, isEditing } = defineProps({
    selectedGroup: {
      type: Object,
      required: true,
    },
    loading: {
      type: Boolean,
      default: false,
    },
    isEditing: {
      type: Boolean,
      default: false,
    },
  });

  const emit = defineEmits(['close']);

  function closePopup() {
    emit('close');
  }
</script>

<style scoped>
  .popup {
    position: absolute;
    max-width: 80%;
    margin-top: 10px;
    margin-bottom: 10px;
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

  .fade-popup-enter-active,
  .fade-popup-leave-active {
    transition:
      opacity 0.3s ease-in-out,
      transform 0.3s ease-in-out;
  }
  .fade-popup-enter-from,
  .fade-popup-leave-to {
    opacity: 0;
    transform: scale(0.95);
  }

  .plugin-grid {
    display: flex;
    flex-wrap: wrap;
    justify-content: center;
    gap: 10px;
  }
</style>
