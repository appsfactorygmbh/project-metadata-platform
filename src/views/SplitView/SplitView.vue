<script setup lang="ts">
  import { Splitpanes, Pane } from 'splitpanes'; //external framework for splitpanes
  import 'splitpanes/dist/splitpanes.css'; //default css for splitpanes
  import { ref, reactive } from 'vue';
  import { useElementSize } from '@vueuse/core';
  import { ProjectSearchView } from '@/views/ProjectSearchView';
  import { MenuButtons } from '@/components/MenuButtons';
  import CreateProjectView from '@/views/CreateProject/createProjectView.vue';

  const tablePane = ref(null);
  const dimensions = reactive(useElementSize(tablePane));

  import { ProjectInformationView } from '@/views/ProjectInformationView';

  const projectInformationPane = ref(null);
  const infoSize = reactive(useElementSize(projectInformationPane));
</script>

<template>
  <div class="container">
    <splitpanes class="default-theme">
      <!--
        size: sets default proportion to 1:4
        min-size: sets smallest possible size to 20% and 1%
      -->
      <pane ref="tablePane" size="68" min-size="20">
        <ProjectSearchView
          :pane-width="dimensions.width"
          :pane-height="dimensions.height"
        />
      </pane>

      <pane size="32" min-size="32">
        <div ref="projectInformationPane">
          <CreateProjectView></CreateProjectView>
          <ProjectInformationView
            :pane-width="infoSize.width"
            :project-id="100"
          />
          <MenuButtons />
        </div>
      </pane>
    </splitpanes>
  </div>
</template>

<style scoped>
  .splitpanes {
    height: 100vh;
  }
</style>
