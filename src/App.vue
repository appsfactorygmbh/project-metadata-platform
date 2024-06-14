<script setup lang="ts">
  import { ref, reactive } from 'vue';
  import { useElementSize } from '@vueuse/core';
  import { Splitpanes, Pane } from 'splitpanes'; //external framework for splitpanes
  import 'splitpanes/dist/splitpanes.css';
  import Table from './components/Table/tableComponent.vue';

  const tablePane = ref(null);
  const dimensions = reactive(useElementSize(tablePane));
</script>

<template>
  <div class="container">
    <!--
        size: sets default proportion to 1:4
        min-size: sets smalles possible size to 20% and 1%
      -->
    <splitpanes class="default-theme">
      <pane size="99" min-size="20">
        <div ref="tablePane">
          <Table
            :pane-width="dimensions.width"
            :pane-height="dimensions.height"
          />
        </div>
      </pane>
      <pane size="1" min-size="1">
        <div></div>
      </pane>
    </splitpanes>
  </div>
</template>
<style scoped>
  .splitpanes {
    height: 100vh;
  }
</style>
