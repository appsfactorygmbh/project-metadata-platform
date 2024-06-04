<script lang="ts">
  import Plugin from './Plugin.vue';
  export default {
    name: 'PluginView',
    components: {
      Plugin
    },
    // TODO: fetch Data from API
  }
</script>

<template>
  <div class="container">
    <Plugin plugin-name="GitHub" url="https://github.com/dasisteinetesturl"></Plugin>
  </div>
</template>

<style scoped lang="scss">
  .container{
    width: 100vw;
    height: 100vh;
    display: flex;
    justify-content: center;
    align-items: center;
  }
</style>