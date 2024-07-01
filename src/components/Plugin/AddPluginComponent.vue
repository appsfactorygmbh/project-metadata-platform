<script setup lang="ts">
  import { PlusOutlined } from '@ant-design/icons-vue';
</script>

<template>
  <!-- Define the card component, styled as a clickable flex container. -->
  <a-card
    class="card"
    :bordered="false"
    toggle="true"
    :body-style="{
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      height: '100%',
    }"
  >
    <!-- Container for plugin name and URL text. -->
    <PlusOutlined class="icon" />
  </a-card>
</template>

<style scoped lang="scss">
  // Import manrope font family.
  @font-face {
    font-family: 'Manrope';
    src: url('../../../public/fonts/manrope/Manrope-VariableFont_wght.woff2')
      format('woff');
  }

  .card {
    width: max-content;
    min-width: 200px;
    max-width: 300px;
    box-shadow: rgba(100, 100, 111, 0.2) 0px 7px 29px 0px !important;
    display: flex;
    flex-direction: column;
  }

  // Style for the text container.
  .icon {
    transform: scale(1.5);
  }
</style>
