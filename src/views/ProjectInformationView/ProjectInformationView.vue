<script lang="ts" setup>
  import { ProjectInformation } from '@/components/ProjectInformation';
  import { projectsStoreSymbol } from '@/store/injectionSymbols';
  import { pluginStoreSymbol } from '@/store/injectionSymbols';
  import { inject, onMounted } from 'vue';
  import PluginView from '@/views/PluginView/PluginView.vue';
  import ProjectEditButtons from '@/components/ProjectEditButtons/ProjectEditButtons.vue';
  import { useEditing } from '@/utils/hooks/useEditing';
  import type { PluginModel } from '@/models/Plugin';
  import type { DetailedProjectModel } from '@/models/Project';
  import type { UpdateProjectModel } from '@/models/Project';

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
  const pluginStore = inject(pluginStoreSymbol)!;

  onMounted(async () => {
    await projectStore.fetchProject(props.projectId);
  });

  const pluginModel = defineModel<PluginModel[] | null>({
    required: true,
    type: Array,
  });

  const cancelEdit = () => {
    console.log('plugins:       ', pluginModel.value);
    pluginModel.value = pluginStore.getPlugins;
    stopEditing();
  };
  const saveEdit = async () => {
    console.log('plugins:       ', pluginModel.value);
    const updateProjectInformation: DetailedProjectModel | null =
      projectStore.getProject || null;
    const updatedProject: UpdateProjectModel = {
      projectName: updateProjectInformation?.projectName,
      businessUnit: updateProjectInformation?.businessUnit,
      teamNumber: updateProjectInformation?.teamNumber,
      department: updateProjectInformation?.department,
      clientName: updateProjectInformation?.clientName,
      pluginList: pluginModel.value,
    };
    console.log('updated Project', updatedProject);
    const projectID = computed(() => projectStore.getProject?.id);
    if (projectID.value != null) {
      await projectStore.updateProject(updatedProject, projectID.value);
      await pluginStore.fetchPlugins(projectID.value);
    }
    stopEditing();
  };
</script>

<template>
  <ProjectEditButtons v-if="isEditing" @cancel="cancelEdit" @save="saveEdit" />
  <ProjectInformation :pane-width="props.paneWidth" />
  <PluginView v-model="pluginModel" :project-i-d="props.projectId"></PluginView>
</template>
