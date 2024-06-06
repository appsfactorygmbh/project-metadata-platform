<script lang="ts">
  // Importiere die Plugin-Komponente aus der Plugin.vue-Datei
  import Plugin from './Plugin.vue';
  // Importiere axios, eine Bibliothek für HTTP-Anfragen (get)
  import axios from 'axios';
  // Importiere funktionen aus vue
  import {defineComponent, ref, onMounted} from "vue";

  // Definiere ein Interface für Plugin-Objekte
  interface Plugin {
    name: string;
    url: string;
  }
  // Definiere PluginView-Komponente
   export default defineComponent({
    name: 'PluginView',
    components: {
      Plugin
    },
     // Der Prop 'type' ist ein string und ist erforderlich
    props:{
      type: {
        type: String,
        required: true
    }
    // TODO: fetch Data from API
  },
     // Ausführen der Setup funktion, definiert die reaktive Logik der  Komponente
    setup(){

      // Erstelle eine reaktive Referenz für ein Array von Plugin-Objekten
    const plugins = ref<Plugin[]>([]);

    // Lifecycle-Hook, wird ausgeführt, wenn die Komponete gemountet(vollständig in die Seite integriert) wird
    onMounted(async () =>{
      try{
        // Sende eine GET_Anfrage an die API, um Plugin-Daten abzurufen
        const response = await axios.get('/api/plugins');

        plugins.value = response.data;
      } catch (error) {
        //Fehlerbehebung
        console.error('Error fetching plugins:', error);
      }
    });
    // Rückgabe der reaktiven Referenz, damit sie im Template verwendet werden kann
    return {
      plugins
    };
    }
  });

</script>


<template>
  <div class="container">
    <h1>Plugins</h1>
    <!-- Verwende die Plugin-Komponente für jedes Plugin im Array -->
      <Plugin v-for="plugin in plugins"
              :plugin-name="plugin.name"
              :url="plugin.url"
      ></Plugin>
    </div>

</template>

<style scoped lang="css">
  .container{
    width: 100vw;
    height: 100vh;
    display: flex;
    justify-content: center;
    align-items: center;
    flex-direction: column;
  }


</style>