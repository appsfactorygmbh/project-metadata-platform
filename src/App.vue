<script setup lang="ts">
  import HelloWorld from './components/HelloWorld.vue';
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
  .logo {
    height: 6em;
    padding: 1.5em;
    will-change: filter;
    transition: filter 300ms;
  }
  .logo:hover {
    filter: drop-shadow(0 0 2em #646cffaa);
  }
  .logo.vue:hover {
    filter: drop-shadow(0 0 2em #42b883aa);
  }
</style>
