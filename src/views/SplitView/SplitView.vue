<script setup lang="ts">
  import { Splitpanes, Pane } from 'splitpanes'; //external framework for splitpanes
  import 'splitpanes/dist/splitpanes.css'; //default css for splitpanes
  import { ref, reactive } from 'vue';
  import { useElementSize } from '@vueuse/core';
  import { ProjectSearchView } from '@/views/ProjectSearchView';
  import { MenuButtons } from '@/components/MenuButtons';
  import { CreateProjectView } from '@/views/CreateProject';
  import { ProjectInformationView } from '@/views/ProjectInformationView';
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';
  import { RightOutlined } from '@ant-design/icons-vue';
  import { useEditing } from '@/utils/hooks/useEditing';

  const { isEditing } = useEditing();

  const tablePane = ref(null);
  const dimensions = reactive(useElementSize(tablePane));

  const splitButton: FloatButtonModel = {
    name: 'SplitButton',
    onClick: () => {},
    icon: RightOutlined,
    status: 'activated',
    tooltip: 'Click here to expand the table',
  };
</script>

<template>
  <div class="container">
    <splitpanes class="default-theme">
      <!--
        size: sets default proportion to 1:4
        min-size: sets smallest possible size to 20% and 1%
      -->
      <pane ref="tablePane" size="70" min-size="20">
        <ProjectSearchView
          :pane-width="dimensions.width"
          :pane-height="dimensions.height"
        />
      </pane>

      <pane size="32" min-size="32" class="rightPane">
        <ProjectInformationView :model-value="null" />
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

  .rightPane {
    position: relative;
  }

  .button {
    position: absolute;
    top: 2.5em;
    left: 1em;
  }
</style>
