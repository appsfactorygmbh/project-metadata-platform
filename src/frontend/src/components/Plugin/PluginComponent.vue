<script lang="ts" setup>
  // Import ref for reactive variables and utility functions for URL handling.
  import { ref, computed, type PropType } from 'vue';
  import { createFaviconURL, cutAfterTLD } from './editURL';
  import { DeleteOutlined, EditOutlined } from '@ant-design/icons-vue';
  import { useThemeToken } from '@/utils/hooks';
  import { usePluginStore, useProjectStore } from '@/store';
  import { App } from 'ant-design-vue';
  import { ResourceActions } from '@/models/utils';
  import ConfirmAction from '@/components/Modal/ConfirmAction.vue';
  const token = useThemeToken();
  const pluginStore = usePluginStore();
  const projectStore = useProjectStore();
  const { notification } = App.useApp();

  // Define the component's props with pluginName and url as required strings.
  const props = defineProps({
    id: {
      type: Number,
      required: true,
    },
    pluginName: {
      type: String,
      required: false,
      default: '',
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

    showFavicon: {
      type: Boolean,
      default: true,
    },
    permissions: {
      type: Array as PropType<ResourceActions[]>,
      default: () => [],
    },
  });
  const localIsEditing = ref(false);
  const isSaving = ref(false);
  const isDeleteModalOpen = ref(false);
  const displayNameInput = ref<string>(props.displayName);
  const urlInput = ref<string>(props.url);

  const faviconUrl = computed(() => createFaviconURL(cutAfterTLD(props.url)));

  const canEditPlugin = computed(() =>
    props.permissions.includes(ResourceActions.Edit),
  );
  const canDeletePlugin = computed(() =>
    props.permissions.includes(ResourceActions.Delete),
  );

  const toggleEdit = () => {
    displayNameInput.value = props.displayName;
    urlInput.value = props.url;
    localIsEditing.value = true;
  };

  const cancelEdit = () => {
    localIsEditing.value = false;
  };

  const savePlugin = async () => {
    if (!urlInput.value || !displayNameInput.value) {
      notification.error({
        message: 'Error',
        description: 'Fields cannot be empty.',
      });
      return;
    }
    const projectId = projectStore.getProject?.id;
    if (!projectId) return;

    try {
      isSaving.value = true;
      await pluginStore.update(projectId, props.id, {
        displayName: displayNameInput.value,
        url: urlInput.value,
      });
      isSaving.value = false;
      localIsEditing.value = false;
      notification.success({
        message: 'Success',
        description: 'Plugin updated.',
      });
    } catch (error) {
      console.error('Validation or API error:', error);
      notification.error({
        message: 'Error!',
        description: (error as Error).message ?? 'An error occurred.',
      });
    } finally {
      isSaving.value = false;
    }
  };

  const handleDelete = () => {
    isDeleteModalOpen.value = true;
  };

  const confirmDelete = async () => {
    const projectId = projectStore.getProject?.id;
    if (!projectId) {
      isDeleteModalOpen.value = false;
      return;
    }

    try {
      isSaving.value = true;
      await pluginStore.remove(projectId, props.id);
      notification.success({
        message: 'Success',
        description: 'Plugin Removed.',
      });
    } catch (error) {
      console.error('Validation or API error:', error);
      notification.error({
        message: 'Error!',
        description: (error as Error).message ?? 'An error occurred.',
      });
    } finally {
      isSaving.value = false;
      isDeleteModalOpen.value = false;
    }
  };
</script>

<template>
  <div class="plugin-wrapper">

    <template v-if="localIsEditing">
      <a-card class="cardNoHover" :loading="isSaving" :bordered="false">
        <div class="textContainerInput">
          <h3 style="text-align: center">{{ pluginName }}</h3>
          <a-input
            v-model:value="displayNameInput"
            placeholder="Display Name"
          />
          <a-input v-model:value="urlInput" placeholder="URL" />
        </div>
>
        <div class="edit-actions">
          <a-button @click.stop="cancelEdit">Cancel</a-button>
          <a-button type="primary" @click.stop="savePlugin">Save</a-button>
        </div>
      </a-card>
    </template>


    <template v-else>
      <div class="card-container">
        <a
          :href="
            props.url.startsWith('http') ? props.url : 'https://' + props.url
          "
          target="_blank"
        >
          <a-card
            class="card"
            :loading="props.isLoading || isSaving"
            :bordered="false"
            :body-style="{
              display: 'flex',
              flexDirection: 'row',
              alignItems: 'center',
              padding: '15px',
            }"
          >
            <a-avatar
              v-if="showFavicon"
              shape="square"
              :src="faviconUrl"
              class="avatar"
            />
            <div class="textContainer">
              <h3>{{ pluginName }}</h3>
              <p>{{ displayName }}</p>
            </div>
          </a-card>
        </a>
        <a-tooltip title="Click here to edit this plugin">
          <EditOutlined
            v-if="canEditPlugin && !isSaving && !props.isLoading"
            class="action-badge edit-badge"
            @click.prevent.stop="toggleEdit"
          />
        </a-tooltip>
        <a-tooltip title="Click here to remove this plugin">
          <DeleteOutlined
            v-if="canDeletePlugin && !isSaving && !props.isLoading"
            class="action-badge delete-badge"
            @click.prevent.stop="handleDelete"
          />
        </a-tooltip>
      </div>
    </template>
    <ConfirmAction
      :is-open="isDeleteModalOpen"
      title="Delete Plugin"
      message="Are you sure you want to remove this plugin?"
      @confirm="confirmDelete"
      @cancel="isDeleteModalOpen = false"
      @update:is-open="(value) => (isDeleteModalOpen = value)"
    />
  </div>
</template>

<style scoped lang="scss">
  // Import manrope font family.
  @font-face {
    font-family: 'Manrope';
    src: url('/fonts/manrope/Manrope-VariableFont_wght.woff2') format('woff');
  }

  .plugin-wrapper {
    position: relative;
    display: inline-block;
  }

  .card-container {
    position: relative;
    display: inline-block;


    &:hover .action-badge {
      opacity: 1;
    }
  }


  .action-badge {
    position: absolute;
    top: -8px;
    z-index: 10;
    width: 24px;
    height: 24px;
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: 50%;
    box-shadow: v-bind('token.boxShadowSecondary');
    color: white;
    font-size: 12px;
    cursor: pointer;
    opacity: 0;
    transition: all 0.2s ease-in-out;

    &:hover {
      transform: scale(1.1);
    }
  }

  .edit-badge {
    left: -8px;
    background-color: #8c8c8c;
  }


  .delete-badge {
    right: -8px;
    background-color: color-mix(in srgb, #6d6e6f, #ff002e 60%);
  }


  .edit-actions {
    display: flex;
    justify-content: flex-end;
    gap: 8px;
    margin-top: 15px;
    padding: 0 10px; 
  }

  .cardNoHover {
    width: max-content;
    min-width: 200px;
    max-width: 300px;
    box-shadow: v-bind('token.boxShadowSecondary');
    display: flex;
    background-color: v-bind('token.colorBgElevated');
  }

  .card {
    width: max-content;
    max-width: 300px;
    min-width: 200px;
    box-shadow: v-bind('token.boxShadowSecondary');
    display: flex;
    flex-direction: column;
    transition: 0.1s ease-in-out;
    background-color: v-bind('token.colorBgElevated');

    &:hover {
      cursor: pointer;
      transform: scale(1.01);
    }
  }

  .textContainerInput,
  .textContainer {
    font-family: Manrope, serif;
    display: flex;
    flex-direction: column;
    justify-content: center;
    margin: 10px;
    white-space: nowrap;
    overflow: hidden;

    & > * {
      margin: 5px 0;
    }

    & p {
      color: #6d6e6f;
      overflow: hidden;
      text-overflow: ellipsis;
      margin: 0;
    }
  }

  .avatar {
    margin: 10px;
    width: 40px;
    height: auto;
    aspect-ratio: 1 / 1;
    object-fit: cover;
  }
</style>
