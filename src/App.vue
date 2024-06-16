<script setup lang="ts">
  import { ref, reactive } from 'vue';
  import { useElementSize } from '@vueuse/core';
  import {Splitpanes, Pane} from 'splitpanes' //external framework that implements the sliders
  import 'splitpanes/dist/splitpanes.css'
  import ProjectView from './components/projectView/projectView.vue'

  const tablePane = ref(null);
  const projectPane = ref(null);  
  const dimensions = reactive(useElementSize(projectPane));
</script>

<template>
 
  <div class="container">
    <!-- divide the UI into pane 1 & pane 2-->
    <splitpanes class="default-theme">
      <pane size="99" min-size="20">
        <div ref="tablePane">
        </div>
      </pane>
      
      <pane size="1" min-size="1">
        <div ref="projectPane">
          <ProjectView 
            :pane-width="dimensions.width"
            :pane-height="dimensions.height"
          /> 
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