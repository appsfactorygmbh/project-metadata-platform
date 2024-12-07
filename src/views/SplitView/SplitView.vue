<script setup lang="ts">
  import { Pane, Splitpanes } from 'splitpanes'; //external framework for splitpanes
  import 'splitpanes/dist/splitpanes.css'; //default css for splitpanes
  import { onBeforeMount, reactive, ref } from 'vue';
  import { useElementSize } from '@vueuse/core';
  import { ProjectSearchView } from '@/views/ProjectSearchView';
  import { MenuButtons } from '@/components/MenuButtons';
  import { CreateProjectView } from '@/views/CreateProject';
  import ProjectView from '../ProjectView/ProjectView.vue';
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';
  import { RightOutlined } from '@ant-design/icons-vue';
  import { useEditing } from '@/utils/hooks';

  const { isEditing } = useEditing();
  const tablePane = ref(null);
  const dimensions = reactive(useElementSize(tablePane));

  const splitButton: FloatButtonModel = {
    name: 'SplitButton',
    onClick: () => {},
    icon: RightOutlined,
    size: 'middle',
    status: 'activated',
    tooltip: 'Click here to expand the table',
  };

  const leftPaneWidth = ref();
  const rightPaneWidth = ref();

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
        <FloatingButton :button="splitButton" class="button" />
        <MenuButtons />
        <CreateProjectView v-if="!isEditing" />
      </pane>
    </splitpanes>
  </div>
</template>

<style scoped>
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

  .button {
    position: absolute;
    top: 2.5em;
    left: 1em;
  }
</style>
