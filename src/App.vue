<script setup lang="ts">
  import ProjectView from './components/projectView.vue'
  import {Splitpanes, Pane} from 'splitpanes' //external framework that implements the sliders
  import 'splitpanes/dist/splitpanes.css'

</script>

<template>
 
  <div class="container">
    <!-- divide the UI into pane 1 & pane 2-->
    <splitpanes class="default-theme">
      <pane id="pane1" size="25">
        <div class="main"></div>
      </pane>
      
      <pane id="pane2" size="75">
        <ProjectView /> 
      </pane>

    </splitpanes>
  </div>    

</template>

<style scoped>
  
  body {
      color: #EBF0F6;
      display: flex;
    }
    /* set default color font for titles*/
    * {
      color: rgb(0, 0, 0);
    }

    #pane1 {
     min-width: 25vw;
    }

    #pane2 {
      padding-right: 30px;
      min-width: 5vw;
      color: #EBF0F6;
      display: flex;
    }

    .container {
      width: 100vw;
      height: 100vh;
      display: flex;
      align-items: center;
    }

</style>