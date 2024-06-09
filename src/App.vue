<script setup lang="ts">
import { ref, reactive } from "vue";
import { useElementSize } from "@vueuse/core";
import {Splitpanes, Pane} from 'splitpanes' //externes Framework, dass die Schieberegler implementiert
import 'splitpanes/dist/splitpanes.css'
import Table from "./components/Table/tableComponent.vue"

const tablePane = ref(null)
const dimensions = reactive(
  useElementSize(
    tablePane
  ),
)

</script>

<template>
  <div class="container">
    <splitpanes class="default-theme">
      <pane size="25" min-size="20">
        <div ref="tablePane"><Table :paneWidth=dimensions.width :paneHeight=dimensions.height /></div>
      </pane>
      <pane size="75" min-size="1">
        <div id="pane2"></div>
      </pane>
    </splitpanes>
  </div>
</template>
<style scoped>
  .splitpanes {
  height: 96vh;
}
</style>
