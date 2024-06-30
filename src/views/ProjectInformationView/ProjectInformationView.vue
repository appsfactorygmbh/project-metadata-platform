<script lang="ts" setup>
  import { ProjectInformation } from '@/components/ProjectInformation';
  import { projectsStoreSymbol } from '@/store/injectionSymbols';
  import { inject, onMounted } from 'vue';
  import PluginView from '@/views/PluginView/PluginView.vue';
  import ProjectEditButtons from '@/components/ProjectEditButtons/ProjectEditButtons.vue';
  import { useEditing } from '@/utils/hooks/useEditing';

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

  const { isEditing, stopEditing } = useEditing();

  const projectStore = inject(projectsStoreSymbol)!;

  onMounted(async () => {
    await projectStore.fetchProject(props.projectId);
  });

  const pluginViewRef = ref<InstanceType<typeof PluginView>>();

  const cancelEdit = () => {
    console.log(pluginViewRef.value)
    pluginViewRef.value?.showPlugins();
    stopEditing()
  }
  const saveEdit = () => {
    console.log("saved")
  }

</script>

<template>
  <ProjectEditButtons
    v-if="isEditing"
    @cancel="cancelEdit"
    @save="saveEdit"
  />
  <ProjectInformation :pane-width="props.paneWidth" />
  <PluginView :project-i-d="props.projectId" ref="pluginViewRef"></PluginView>
</template>
