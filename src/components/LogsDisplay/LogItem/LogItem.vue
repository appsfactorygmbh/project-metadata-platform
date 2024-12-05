<script setup lang="ts">
  import { reactive, ref } from 'vue';
  import { useElementSize } from '@vueuse/core';
  const props = defineProps({
    timeStamp: {
      type: String,
      required: true,
    },
    logMessage: {
      type: String,
      required: true,
    },
    isLast: {
      type: Boolean,
      required: true,
    },
  });
  const timeStampDate = new Date(props.timeStamp);
  const timeStampSize = ref(null);
  const size = reactive(useElementSize(timeStampSize));

  const minWidth = computed(() => {
    if (size.width < 340) return '90px';
    else return '162px';
  });
</script>

<template>
  <div ref="timeStampSize" class="container">
    <div class="text timeStamp" :style="{ minWidth }">
      {{ timeStampDate.toLocaleString('de-DE') }}
    </div>
    <div class="line-container">
      <div class="circle" />
      <div v-if="!isLast" class="line" />
    </div>
    <p class="text">{{ props.logMessage }}</p>
  </div>
</template>

<style scoped>
  .line-container {
    display: flex;
    flex-direction: column;
    align-items: center;
    margin: 0 1em;
    transform: translateY(7px);
  }

  .circle {
    width: 10px;
    height: 10px;
    background-color: white;
    border: 3px solid #6d6e6f;
    border-radius: 50%;
  }
  .line {
    width: 3px;
    min-height: 30px;
    background-color: rgba(5, 5, 5, 0.06);
    flex-grow: 1;
  }
  .container {
    display: flex;
    flex-direction: row;
    height: max-content;
  }
  .text {
    margin: 0;
    margin-bottom: 20px;
  }
  .hide {
    display: none;
  }
  .timeStamp {
    display: flex;
    flex-wrap: wrap;
    justify-content: flex-end;
    height: 0em;
    margin: 0;
  }

  .timeStamp p {
    width: 70px;
    margin: 0;
  }
</style>
