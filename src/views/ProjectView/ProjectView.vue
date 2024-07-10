<script lang="ts" setup>
  import { ProjectPlugins } from './ProjectPlugins';
  import { ProjectInformation } from './ProjectInformation';
  import type {
    DetailedProjectModel,
    UpdateProjectModel,
  } from '@/models/Project';
  import type { PluginModel } from '@/models/Plugin';
  import ProjectEditButtons from '@/components/ProjectEditButtons/ProjectEditButtons.vue';
  import { useEditing } from '@/utils/hooks/useEditing';
  import {
    pluginStoreSymbol,
    projectsStoreSymbol,
  } from '@/store/injectionSymbols';
  import { inject } from 'vue';

  const pluginStore = inject(pluginStoreSymbol);
  const projectStore = inject(projectsStoreSymbol);

  const { isEditing, stopEditing } = useEditing();

  const cancelEdit = () => {
    pluginModel.value = pluginStore?.getPlugins || [];
    stopEditing();
  };
  const saveEdit = async () => {
    console.log('plugins:       ', pluginModel.value);
    const updateProjectInformation: DetailedProjectModel | null =
      projectStore?.getProject || null;
    const updatedProject: UpdateProjectModel = {
      projectName: updateProjectInformation?.projectName,
      businessUnit: updateProjectInformation?.businessUnit,
      teamNumber: updateProjectInformation?.teamNumber,
      department: updateProjectInformation?.department,
      clientName: updateProjectInformation?.clientName,
      pluginList: pluginModel.value,
    };
    console.log('updated Project', updatedProject);
    const projectID = computed(() => projectStore?.getProject?.id);
    if (projectID.value != null) {
      await projectStore?.updateProject(updatedProject, projectID.value);
      await pluginStore?.fetchPlugins(projectID.value);
    }
    stopEditing();
  };

  const pluginModel = defineModel<PluginModel[] | undefined>({
    required: true,
    type: Array,
  });
</script>

<template>
  <ProjectEditButtons v-if="isEditing" @cancel="cancelEdit" @save="saveEdit" />
  <ProjectInformation />
  <ProjectPlugins v-model.lazy="pluginModel" class="pluginView" />
</template>

<style scoped>
  .pluginView {
    display: flex;
    justify-content: center;
    padding-top: 1em;
  }
</style>
