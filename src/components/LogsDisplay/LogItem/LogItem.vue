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
  const dateTime = computed(() =>
    new Date(props.timeStamp).toLocaleString().split(','),
  );
  const timeStampSize = ref(null);
  const size = reactive(useElementSize(timeStampSize));

  const minWidth = computed(() => {
    if (size.width < 340) return '100px';
    else return '162px';
  });
</script>

<template>
  <div ref="timeStampSize" class="container">
    <div class="text timeStamp" :style="{ minWidth }">
      <p class="date">{{ dateTime[0] }}</p>
      <p class="time">{{ dateTime[1] }}</p>
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
    margin: 0 20px 0 20px;
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
    height: 40px;
  }

  .timeStamp p {
    width: 80px;
    margin: 0;
  }
</style>
