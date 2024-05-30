<script setup lang="ts">
import { ref } from 'vue'
import {Splitpanes, Pane} from 'splitpanes' //externes Framework, dass die Schieberegler implementiert
import 'splitpanes/dist/splitpanes.css'
</script>

<template>
  <div class="container">
    <splitpanes class="default-theme">
      <pane id="pane1" size="25">
        <div></div>
      </pane>
      <pane id="pane2" size="75">
        <div></div>
      </pane>
    </splitpanes>
  </div>
</template>

<style scoped>
.container {
  width: 100vw;
  height: 100vh;
}
#pane1 {
  min-width: 25vw;
}
#pane2 {
  min-width: 5vw;
}
</style>