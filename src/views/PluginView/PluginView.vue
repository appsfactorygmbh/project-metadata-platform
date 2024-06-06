<script lang="ts">
  // Importiere die Plugin-Komponente aus der Plugin.vue-Datei
  import Plugin from '../../components/Plugin/Plugin.vue';
  // Importiere funktionen aus vue
  import {defineComponent, ref} from "vue";

   export default defineComponent({
   name: 'PluginView',
   components: {
     Plugin
   },
    setup() {
     const plugins = ref([
       {
         name: 'Vue Plugin',
         url: 'https://vuejs.org/'
       },
       {
         name: 'React Plugin',
         url: 'https://reactjs.org/'
       }
     ]);

     return {
       plugins
     };
   }
 });
</script>

  <template>
    <div class="container">
    <!-- Verwende die Plugin-Komponente für jedes Plugin im Array -->
    <Plugin class="plugins" v-for="plugin in plugins"
                 :plugin-name="plugin.name"
                 :url="plugin.url"
                 :key="plugin.name"
    ></Plugin>
    </div>

    </template>

    <style scoped lang="css">
  .container{
    width: 100vw;
    height: auto;
    display: flex;
    justify-content: center;
    align-items: center;
    flex-direction: row;
    flex-wrap: wrap;
  }
  .plugins{

    margin-top: 10px;
  }


  </style>