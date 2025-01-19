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
  import { theme } from 'ant-design-vue';

  const { useToken } = theme;
  const { token } = useToken();

  console.log(token.value.colorBgBase);

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
      </pane>
    </splitpanes>
  </div>
</template>

<style scoped>
  :deep(.splitpanes.default-theme .splitpanes__pane) {
    background-color: v-bind('token.colorBgContainer');
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
</style>
