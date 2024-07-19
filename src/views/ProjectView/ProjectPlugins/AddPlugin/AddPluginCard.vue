<script setup lang="ts">
  import { ref } from 'vue';
  import AddPluginView from './AddPluginView.vue';

  // It controls the visibility of the modal.
  const openModal = ref<boolean>(false);

  // Opens the modal.
  const handleClick = () => {
    openModal.value = true;
    console.log('Open modal');
  };

  const closeModal = () => {
    openModal.value = false;
    console.log('Close modal');
  };
</script>

<template>
  <!-- Placeholder for adding new Plugin -->
  <a-card
    class="card"
    :bordered="false"
    :body-style="{
      display: 'flex',
      flexDirection: 'row',
      alignItems: 'center',
      justifyContent: 'center',
      padding: '15px',
    }"
    @click="handleClick"
  >
    <h1>+</h1>
  </a-card>

  <!-- Opens modal for adding new Plugin when openModal has value "true" -->
  <AddPluginView v-if="openModal" :show-modal="openModal" @close="closeModal" />
</template>

<style scoped lang="scss">
  .card {
    cursor: pointer;
    width: max-content;
    min-width: 200px;
    max-width: 300px;
    border: 1px dashed #d9d9d9;
    display: flex;
    flex-direction: column;
    color: black;
    transition: 0.1s ease-in-out;

    &:hover {
      cursor: pointer;
      transform: scale(1.1);
    }
  }
</style>
