<script lang="ts">
interface Plugin {
  pluginName: string;
  url: string;
}
import { defineComponent } from "vue";
import Plugin from "./Plugin.vue";
export default defineComponent({
  name: "PluginView",
  components: {
    Plugin,
  },
  props: {
    pluginProps: {
      type: Array as () => Plugin[],
      required: true,
    },
  },
});
</script>
<template>
  <a-row justify="start" :gutter="[15, 15]">
    <div v-for="plugin in pluginProps">
      <a-col class="gutter-row">
        <Plugin :url="plugin.url" :pluginName="plugin.pluginName"></Plugin>
      </a-col>
    </div>
  </a-row>
</template>
