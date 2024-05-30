<script setup lang="ts">
import { ref } from 'vue'

defineProps<{ msg: string }>()

const count = ref(0)
const weatherForecasts = ref<any[]>([])

async function getForecasts() {
  count.value++;
  try {
    const response = await fetch(import.meta.env.VITE_BACKEND_URL + '/WeatherForecast?count=' + count.value, {
      headers: {
        'Accept': 'text/plain',
        'Access-Control-Allow-Origin': '*',
        'cors': 'no-cors'
      },
    })
    weatherForecasts.value = await response.json()
  } catch (error) {
    console.error('Failed to fetch weather forecasts:', error)
  }
}
</script>

<template>
  <h1>{{ msg }}</h1>

  <div class="card">
    <button type="button" @click="getForecasts()">count is {{ count }}</button>
    <p>
      Edit
      <code>components/HelloWorld.vue</code> to test HMR
    </p>
  </div>

  <div>
    <h2>Weather Forecasts</h2>
    <ul>
      <li v-for="forecast in weatherForecasts" :key="forecast.date">
        {{ forecast.date }} - {{ forecast.temperatureC }}°C - {{ forecast.summary }}
      </li>
    </ul>
  </div>
</template>

<style scoped>
.read-the-docs {
  color: #888;
}
</style>