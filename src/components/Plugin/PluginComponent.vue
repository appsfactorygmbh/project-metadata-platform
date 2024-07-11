<script lang="ts" setup>
  // Import ref for reactive variables and utility functions for URL handling.
  import { ref, watch, onMounted } from 'vue';
  import { cutAfterTLD, createFaviconURL } from './editURL';
  import { DeleteOutlined } from '@ant-design/icons-vue';
  import { useProjectEditStore } from '@/store';

  // Define the component's props with pluginName and url as required strings.
  const props = defineProps({
    id: {
      type: Number,
      required: true,
    },
    pluginName: {
      type: String,
      required: true,
    },
    url: {
      type: String,
      required: true,
    },
    displayName: {
      type: String,
      required: true,
    },
    isLoading: {
      type: Boolean,
      required: false,
    },
    isEditing: {
      type: Boolean,
      required: true,
    },
  });

  const initDisplayName = props.displayName;
  const initUrl = props.url;

  const projectEditStore = useProjectEditStore();

  const displayNameInput = ref<string>(props.displayName);
  const urlInput = ref<string>(props.url);

  const toggleSkeleton = ref<boolean>(props.isLoading);

  onMounted(() => {
    if (projectEditStore) {
      projectEditStore.initialAdd({
        pluginName: props.pluginName,
        displayName: props.displayName,
        url: props.url,
        id: props.id,
      });
    }
  });

  watch(
    () => props.isEditing,
    (newVal) => {
      if (!newVal) {
        displayNameInput.value = initDisplayName;
        urlInput.value = initUrl;
        hide.value = false;
      }
    },
  );

  watch(
    () => props.isLoading,
    (newVal) => {
      toggleSkeleton.value = newVal;
    },
  );
  // Create a reactive variable for the favicon URL based on the given URL.
  const faviconUrl = ref(createFaviconURL(cutAfterTLD(props.url)));

  // Copies URL to clipboard when card is clicked.
  async function handleClick() {
    try {
      await navigator.clipboard.writeText(props.url);
    } catch (err) {
      console.error('Error when trying to copy: ', err);
    }
    window.open(props.url, '_blank');
  }

  const hide = ref<boolean>(false);

  const hidePlugin = () => {
    hide.value = true;
    projectEditStore?.addDeletedPlugin({
      pluginName: props.pluginName,
      displayName: displayNameInput.value,
      url: urlInput.value,
      id: props.id,
    });
  };

  const updateDisplayName = () => {
    projectEditStore?.updatePluginChanges(props.id.toString() + props.url, {
      pluginName: props.pluginName,
      displayName: displayNameInput.value,
      url: urlInput.value,
      id: props.id,
    });
  };
</script>

<template>
  <template v-if="isEditing">
    <a-card
      class="cardNoHover"
      :loading="toggleSkeleton"
      :bordered="false"
      toggle="true"
      :body-style="{
        display: 'flex',
        flexDirection: 'row',
        alignItems: 'center',
        padding: '15px',
      }"
      :style="hide ? 'display: none' : ''"
    >
      <!-- Container for plugin name and URL text. -->
      <div class="textContainerInput">
        <h3 style="text-align: center">{{ pluginName }}</h3>
        <a-input
          v-model:value="displayNameInput"
          class="inputField"
          @input="updateDisplayName"
        ></a-input>
        <a-input
          v-model:value="urlInput"
          class="inputField"
          @input="updateDisplayName"
        ></a-input>
      </div>
      <DeleteOutlined class="circleBackground" @click="hidePlugin" />
    </a-card>
  </template>

  <template v-else>
    <!-- Define the card component, styled as a clickable flex container. -->
    <a-card
      class="card"
      :loading="toggleSkeleton"
      :bordered="false"
      toggle="true"
      :body-style="{
        display: 'flex',
        flexDirection: 'row',
        alignItems: 'center',
        padding: '15px',
      }"
      @click="handleClick"
    >
      <!-- Display the favicon image. -->
      <a-avatar shape="square" :src="faviconUrl" class="avatar"></a-avatar>
      <!-- Container for plugin name and URL text. -->
      <div class="textContainer">
        <h3>{{ pluginName }}</h3>
        <p>{{ displayName }}</p>
      </div>
    </a-card>
  </template>
</template>

<style scoped lang="scss">
  // Import manrope font family.
  @font-face {
    font-family: 'Manrope';
    src: url('../../../public/fonts/manrope/Manrope-VariableFont_wght.woff2')
      format('woff');
  }

  // Style for the card container.

  .circleBackground {
    padding: 3.5%;
    border-radius: 100%;
    background-color: white;
    position: absolute;
    top: -3%;
    right: -3%;
    box-shadow: rgba(100, 100, 111, 0.2) 0 7px 29px 0;
    &:hover {
      transition: 0.1s ease-in-out;
      cursor: pointer;
      transform: scale(1.1);
    }
  }

  .cardNoHover {
    width: max-content;
    min-width: 200px;
    max-width: 300px;
    box-shadow: rgba(100, 100, 111, 0.2) 0 7px 29px 0 !important;
    display: flex;
    flex-direction: column;
  }

  .card {
    width: max-content;
    min-width: 200px;
    max-width: 300px;
    box-shadow: rgba(100, 100, 111, 0.2) 0 7px 29px 0 !important;
    display: flex;
    flex-direction: column;
    transition: 0.1s ease-in-out;

    &:hover {
      cursor: pointer;
      transform: scale(1.01);
    }
  }

  .textContainerInput {
    font-family: Manrope;
    display: flex;
    flex-direction: column;
    justify-content: center;
    margin: 10px;
    white-space: nowrap;
    overflow: hidden;

    & > * {
      margin: 5px 0 5px 0;
    }

    & p {
      color: #6d6e6f;
      overflow: hidden;
      text-overflow: ellipsis;
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

    & > * {
      margin: 0;
    }

    & p {
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
