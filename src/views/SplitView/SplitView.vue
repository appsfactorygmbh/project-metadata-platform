<script setup lang="ts">
  import { Pane, Splitpanes } from 'splitpanes'; //external framework for splitpanes
  import 'splitpanes/dist/splitpanes.css'; //default css for splitpanes
  import { onBeforeMount, reactive, ref } from 'vue';
  import { useElementSize } from '@vueuse/core';
  import { ProjectSearchView } from '@/views/ProjectSearchView';
  import { MenuButtons } from '@/components/MenuButtons';
  import { CreateProjectView } from '@/views/CreateProject';
  import ProjectView from '../ProjectView/ProjectView.vue';
  import { useEditing } from '@/utils/hooks';
  import { useThemeToken } from '@/utils/hooks';
  import { AddPluginView } from '@/views/ProjectView/ProjectPlugins/AddPlugin';
  import { AppstoreAddOutlined } from '@ant-design/icons-vue';
  import { Tooltip } from 'ant-design-vue';

  const token = useThemeToken();

  const { isEditing } = useEditing();
  const tablePane = ref(null);
  const dimensions = reactive(useElementSize(tablePane));

  const leftPaneWidth = ref<number>(60);
  const rightPaneWidth = ref<number>(40);

  onBeforeMount(() => {
    const paneSizeFromLocalStorage = localStorage.getItem('paneSizes');
    if (paneSizeFromLocalStorage) {
      leftPaneWidth.value = JSON.parse(paneSizeFromLocalStorage)[0].size;
      rightPaneWidth.value = JSON.parse(paneSizeFromLocalStorage)[1].size;
    }
  });

  const onResize = (newSizes: number[]) => {
    localStorage.setItem('paneSizes', JSON.stringify(newSizes));
  };

  const openModal = ref<boolean>(false);

  const handleClick = () => {
    openModal.value = true;
  };

  const closeModal = () => {
    openModal.value = false;
  };
</script>

<template>
  <div class="container">
    <splitpanes class="default-theme" @resized="onResize">
      <!--
        size: sets default proportion to 1:4
        min-size: sets smallest possible size to 20% and 1%
      -->
      <pane
        ref="tablePane"
        :size="leftPaneWidth"
        min-size="20"
        class="leftPane"
      >
        <ProjectSearchView
          :pane-width="dimensions.width"
          :pane-height="dimensions.height"
        />
      </pane>

      <pane :size="rightPaneWidth" min-size="32" class="rightPane">
        <ProjectView />
        <MenuButtons />
        <CreateProjectView v-if="!isEditing" />
        <div v-if="!isEditing" class="floating-button-container">
          <Tooltip title="Click here to add a new plugin " placement="left">
            <a-card
              class="floating-button"
              :bordered="false"
              @click="handleClick"
            >
              <AppstoreAddOutlined />
            </a-card>
          </Tooltip>
        </div>

        <AddPluginView
          v-if="openModal"
          :show-modal="openModal"
          @close="closeModal"
        />
      </pane>
    </splitpanes>
  </div>
</template>

<style scoped>
  :deep(.splitpanes.default-theme .splitpanes__pane) {
    background-color: v-bind('token.colorFill') !important;
  }

  :deep(.splitpanes.default-theme .splitpanes__splitter) {
    background-color: v-bind('token.colorBgElevated') !important;
    border: 0;
  }

  :deep(.splitpanes.default-theme .splitpanes__splitter::before) {
    background-color: v-bind('token.colorFillSecondary') !important;
  }

  :deep(.splitpanes.default-theme .splitpanes__splitter::after) {
    background-color: v-bind('token.colorFillSecondary') !important;
  }

  .splitpanes {
    height: 100vh;
  }

  .leftPane {
    position: relative;
  }

  .rightPane {
    position: relative;
    max-height: 100vh; /* Set a maximum height */
    overflow-y: auto; /* Enable vertical scrolling */
  }
  /* Container für den Floating-Button */
  .floating-button-container {
    position: absolute;
    right: 20px;
    bottom: 100px;
    z-index: 10;
  }

  .floating-button {
    cursor: pointer;
    display: flex;
    align-items: center;
    justify-content: center;
    background-color: #6d6e6f;
    color: white;
    width: 50px;
    height: 50px;
    border-radius: 50%;
    font-size: 24px;
    transition: 0.3s ease-in-out;
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  }

  .floating-button:hover {
    transform: scale(1.1);
  }
</style>
