<script setup lang="ts">
import {Splitpanes, Pane} from 'splitpanes' //externes Framework, dass die Schieberegler implementiert
import 'splitpanes/dist/splitpanes.css'
</script>

<template>
  <div class="container">
    <splitpanes class="default-theme">
      <pane size="25" min-size="25">
        <div></div>
      </pane>
      <pane size="75" min-size="1">
        <div></div>
      </pane>
    </splitpanes>
  </div>
</template>

<style scoped>
.splitpanes {
  height: 96vh;
}
</style>
