<script lang="ts" setup>
  import { ProjectInformation } from '@/components/ProjectInformation';
  import { projectsStoreSymbol } from '@/store/injectionSymbols';
  import { inject, onMounted } from 'vue';
  import PluginView from '@/views/PluginView/PluginView.vue';
  import {SaveOutlined, CloseOutlined} from "@ant-design/icons-vue";
  import { useEditing } from "@/utils/hooks/useEditing"

  const props = defineProps({
    paneWidth: {
      type: Number,
      required: true,
    },
    projectId: {
      type: Number,
      required: true,
    },
  });

  const { isEditing } = useEditing();

  const projectStore = inject(projectsStoreSymbol)!;

  onMounted(async () => {
    await projectStore.fetchProject(props.projectId);
  });


</script>

<template>

  <a-float-button-group class="menu" v-if="isEditing">
    <a-float-button>
      <template #icon><SaveOutlined class="icon" /> </template>
    </a-float-button>
    <a-float-button>
      <template #icon><CloseOutlined class="icon" /> </template>
    </a-float-button>
  </a-float-button-group>

  <ProjectInformation :pane-width="props.paneWidth" />
  <PluginView :project-i-d="props.projectId"></PluginView>
</template>
