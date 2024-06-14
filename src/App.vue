<script setup lang="ts">
  import { ref } from 'vue';
  import ProjectView from './components/projectView/projectView.vue'
  import {Splitpanes, Pane} from 'splitpanes' //external framework that implements the sliders
  import 'splitpanes/dist/splitpanes.css'
  const tablePane = ref(null);
</script>

<template>
 
  <div class="container">
    <!-- divide the UI into pane 1 & pane 2-->
    <splitpanes class="default-theme">
      <pane size="25" min-size="20">
        <div ref="tablePane">
        </div>
      </pane>
      
      <pane size="75" min-size="7">
        <div ref="projectPane">
          <ProjectView /> 
        </div>
      </pane>

    </splitpanes>
  </div>    
</template>

<style scoped>
    .splitpanes {
      width: 100vw;
      height: 100vh;
    }
</style>