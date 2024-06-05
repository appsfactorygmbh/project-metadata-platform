<script lang="ts" xmlns:https="http://www.w3.org/1999/xhtml">
export default {
  name: "Plugin",
  props: {
    pluginName: {
      type: String,
      required: true,
    },
    url: {
      type: String,
      required: true,
    },
  },
  data() {
    return {
      faviconUrl: "",
    };
  },
  methods: {
    createFaviconURL(tld: string) {
      this.faviconUrl = `https://www.google.com/s2/favicons?domain=${tld}&sz=128`;
    },
    cutAfterTLD(url: string): string {
      let match = url.match(/^https?:\/\/[^\/]+/);
      if (match) {
        return match[0];
      }
      return url;
    },
    async copyToClipboard() {
      try {
        await navigator.clipboard.writeText(this.url);
      } catch (err) {
        console.error("Fehler beim Kopieren: ", err);
      }
    },
  },
  created() {
    this.createFaviconURL(this.cutAfterTLD(this.url));
  },
};
</script>

<template>
  <a-card
    class="card"
    :bordered="false"
    toggle="true"
    bodyStyle="display: flex; flex-direction: row; align-items: center; padding: 15px"
    @click="copyToClipboard"
  >
    <a-avatar :src="faviconUrl" class="avatar"></a-avatar>
    <div class="textContainer">
      <h3>{{ pluginName }}</h3>
      <a v-bind:href="url">{{ url }}</a>
    </div>
  </a-card>
</template>

<style lang="scss">
@import url("https://fonts.googleapis.com/css2?family=Manrope:wght@200..800&display=swap");
.card {
  width: max-content;
  max-width: 100%;
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
}
</style>
