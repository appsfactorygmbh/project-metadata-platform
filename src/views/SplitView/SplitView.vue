<script setup lang="ts">
  import { Splitpanes, Pane } from 'splitpanes'; //external framework for splitpanes
  import 'splitpanes/dist/splitpanes.css'; //default css for splitpanes
  import { ref, reactive } from 'vue';
  import { useElementSize } from '@vueuse/core';
  import { ProjectInformationView } from '@/views/ProjectInformationView';

  const projectPane = ref(null);
  const dimensions = reactive(useElementSize(projectPane));
</script>

<template>
  <div class="container">
    <splitpanes class="default-theme">
      <!--
        size: sets default proportion to 1:4
        min-size: sets smallest possible size to 20% and 1%
      -->
      <pane size="99" min-size="20">
        <div></div>
      </pane>

      <pane size="1" min-size="1">
        <div ref="projectPane">
          <ProjectInformationView :pane-width="dimensions.width" />
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
