<script setup lang="ts">
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
  console.log(props.isLast);
</script>

<template>
  <div class="container">
    <p class="text timeStamp">{{ props.timeStamp }}</p>
    <div class="line-container">
      <div class="circle"></div>
      <div v-if="!isLast" class="line"></div>
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
    border: 3px solid #176fff;
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
  }
  .text {
    margin: 0;
    margin-bottom: 20px;
  }
  .hide {
    display: none;
  }
  .timeStamp {
    min-width: 150px;
    text-align: end;
  }
</style>
