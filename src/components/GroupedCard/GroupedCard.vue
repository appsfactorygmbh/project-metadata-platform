<template>
  <div @click="openGroupPopup">
    <a-badge :count="pluginCount">
      <a-card
        class="grouped-card"
        :body-style="{
          display: 'flex',
          flexDirection: 'row',
          alignItems: 'center',
          padding: '15px',
        }"
      >
        <a-avatar :src="faviconUrl" shape="square" class="avatar"></a-avatar>
        <div class="textContainer">
          <h3>{{ displayName + ' Plugins' }}</h3>
        </div>
      </a-card>
    </a-badge>
  </div>
</template>

<script setup lang="ts">
  import { defineEmits, defineProps } from 'vue';

  const { pluginCount, displayName, faviconUrl } = defineProps({
    pluginCount: {
      type: Number,
      required: true,
    },
    displayName: {
      type: String,
      required: true,
    },
    faviconUrl: {
      type: String,
      required: true,
    },
  });

  const emit = defineEmits(['open']);

  function openGroupPopup() {
    emit('open');
  }
</script>

<style scoped>
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
  }
</style>
