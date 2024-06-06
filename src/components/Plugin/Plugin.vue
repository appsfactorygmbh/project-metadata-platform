<script lang="ts" setup>
  import { ref, onMounted } from 'vue';

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

  const faviconUrl = ref<any>('');

  function createFaviconURL(tld: string) {
    faviconUrl.value = `https://www.google.com/s2/favicons?domain=${tld}&sz=128`;
  }

  function cutAfterTLD(url: string): string {
    const regex = /^(https?:\/\/)?([^\/]+(\.[a-z]{2,}))/i;
    const match = url.match(regex);
    if (match) {
      return match[0];
    }
    return url;
  }

  async function copyToClipboard() {
    try {
      await navigator.clipboard.writeText(props.url);
    } catch (err) {
      console.error('Fehler beim Kopieren: ', err);
    }
  }

  onMounted(() => {
    createFaviconURL(cutAfterTLD(props.url));
  });
</script>

<template>
  <a-card
    class="card"
    :bordered="false"
    toggle="true"
    :body-style = "{display: 'flex', flexDirection: 'row', alignItems: 'center', padding: '15px'}"
    @click="copyToClipboard"
  >
    <a-avatar :src="faviconUrl" class="avatar"></a-avatar>
    <div class="textContainer">
      <h3>{{ pluginName }}</h3>
      <a :href="url">{{ url }}</a>
    </div>
  </a-card>
</template>

<style scoped lang="scss">
  @import url('https://fonts.googleapis.com/css2?family=Manrope:wght@200..800&display=swap');
  .card {
    width: max-content;
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
  .avatar {
    margin: 10px;
    min-width: 30px;
    max-width: 30px;
    height: auto;
    aspect-ratio: 1 / 1; // Für ein quadratisches Bild
    object-fit: cover;
  }
</style>
