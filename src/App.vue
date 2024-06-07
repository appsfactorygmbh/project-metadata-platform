<script setup lang="ts">
import {Splitpanes, Pane} from 'splitpanes' //external framework for splitpanes
import 'splitpanes/dist/splitpanes.css' //default css for splitpanes
</script>

<template>
  <div class="container">

    <splitpanes class="default-theme">
      <!--
        size: sets default proportion to 1:4
        min-size: sets smalles possible size to 20% and 1%
      -->
      <pane id="pane1" size="25" min-size="20">
        <div>

        </div>
      </pane>
      <pane id="pane2" size="75" min-size="1">
        <div>

        </div>
      </pane>

    </splitpanes>
  </div>
</template>

<style scoped>
.splitpanes {
  height: 96vh;
}
</style>
