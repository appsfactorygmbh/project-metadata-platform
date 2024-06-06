<script lang="ts" setup>
  // Import ref for reactive variables and utility functions for URL handling.
  import { ref } from 'vue';
  import { cutAfterTLD, createFaviconURL } from './editURL';

  // Define the component's props with pluginName and url as required strings.
  const props = defineProps({
    pluginName: {
      type: String,
      required: true,
    },
    url: {
      type: String,
      required: true,
    },
  });

  // Create a reactive variable for the favicon URL based on the given URL.
  const faviconUrl = ref(createFaviconURL(cutAfterTLD(props.url)));

  // Copies URL to clipboard when card is clicked.
  async function copyToClipboard() {
    try {
      await navigator.clipboard.writeText(props.url);
    } catch (err) {
      console.error('Error when trying to copy: ', err);
    }
  }
</script>

<template>
  <!-- Define the card component, styled as a clickable flex container. -->
  <a-card
    class="card"
    :bordered="false"
    toggle="true"
    :body-style="{
      display: 'flex',
      flexDirection: 'row',
      alignItems: 'center',
      padding: '15px',
    }"
    @click="copyToClipboard"
  >
    <!-- Display the favicon image. -->
    <a-avatar shape="square" :src="faviconUrl" class="avatar"></a-avatar>
    <!-- Container for plugin name and URL text. -->
    <div class="textContainer">
      <h3>{{ pluginName }}</h3>
      <a :href="url">{{ url }}</a>
    </div>
  </a-card>
</template>

<style scoped lang="scss">
  // Import Manrope font family.
  // TODO: install font locally
  @import url('https://fonts.googleapis.com/css2?family=Manrope:wght@200..800&display=swap');

  // Style for the card container.
  .card {
    min-width: 400px;
    max-width: 400px;
    box-shadow: rgba(100, 100, 111, 0.2) 0px 7px 29px 0px !important;
    display: flex;
    flex-direction: column;
    transition: 0.1s ease-in-out;

    &:hover {
      cursor: pointer;
      transform: scale(1.01);
    }
  }

  // Style for the text container.
  .textContainer {
    font-family: Manrope;
    display: flex;
    flex-direction: column;
    justify-content: center;
    margin: 10px;
    white-space: nowrap;
    overflow: hidden;

    & h3 {
      margin: 0px;
    }

    & a {
      color: #6d6e6f;
      overflow: hidden;
      text-overflow: ellipsis;
    }
  }

  // Style for the avatar image.
  .avatar {
    margin: 10px;
    width: 40px;
    height: auto;
    aspect-ratio: 1 / 1;
    object-fit: cover;
  }
</style>
