<script lang="ts" setup>
  import { ProjectPlugins } from './ProjectPlugins';
  import { LocalLogView } from './LocalLogView';
  import { ProjectInformation } from './ProjectInformation';
  import type { UpdateProjectModel } from '@/models/Project';
  import ProjectEditButtons from '@/components/ProjectEditButtons/ProjectEditButtons.vue';
  import { useEditing } from '@/utils/hooks/useEditing';
  import {
    pluginStoreSymbol,
    projectEditStoreSymbol,
    projectsStoreSymbol,
  } from '@/store/injectionSymbols';
  import { inject, watch } from 'vue';
  import { message } from 'ant-design-vue';

  const pluginStore = inject(pluginStoreSymbol);
  const projectStore = inject(projectsStoreSymbol);
  const projectEditStore = inject(projectEditStoreSymbol);

  const { isEditing, stopEditing } = useEditing();

  const reloadEditStore = () => {
    if (pluginStore) {
      pluginStore?.getPlugins.forEach((plugin) => {
        projectEditStore?.initialAdd(plugin);
      });
    }
  };

  watch(
    () => isEditing,
    (newVal) => {
      if (newVal) {
        projectEditStore?.resetPluginChanges();
      }
    },
  );

  const cancelEdit = () => {
    projectEditStore?.resetPluginChanges();
    reloadEditStore();
    stopEditing();
  };

  const isAdding = computed(() => projectStore?.getIsLoadingUpdate);

  // Watcher to see if fetch was successful
  watch(isAdding, (newVal) => {
    if (!newVal) {
      if (projectStore?.getUpdatedSuccessfully) {
        projectEditStore?.resetPluginChanges();
        message.success('Project updated successfully.', 7);
        projectStore.fetchProject(projectStore.getProject?.id || 0);
        stopEditing();
      } else {
        message.error('Could not update Project.', 5);
      }
    }
  });

  const saveEdit = async () => {
    // Check for empty fields and duplicates
    projectEditStore?.checkForConflicts();

    // If error occurred, display message and return
    if (!projectEditStore?.getCanBeAdded) {
      message.error(
        'Could not update Project. There are empty fields or duplicated plugins.',
        5,
      );
      return;
    }

    if (!projectStore?.getProject) {
      console.error(
        'Error when trying to get ProjectInformation. getProject is undefined',
      );
      return;
    }
    const updateProjectInformation =
      projectEditStore.getProjectInformationChanges;
    const updatedProject: UpdateProjectModel = {
      projectName: updateProjectInformation?.projectName,
      businessUnit: updateProjectInformation?.businessUnit,
      teamNumber: updateProjectInformation?.teamNumber,
      department: updateProjectInformation?.department,
      clientName: updateProjectInformation?.clientName,
      pluginList: projectEditStore?.getPluginChanges,
    };
    const projectID = computed(() => projectStore?.getProject?.id);
    if (projectID.value) {
      await projectStore?.updateProject(updatedProject, projectID.value);
      await projectStore.fetchProjects();
      await projectStore.fetchProject(projectID.value);
      await pluginStore?.fetchPlugins(projectID.value);
    }
  };
</script>

<template>
  <ProjectEditButtons v-if="isEditing" @cancel="cancelEdit" @save="saveEdit" />
  <ProjectInformation />
  <ProjectPlugins class="pluginView" />
  <LocalLogView class="LocalLog" />
</template>

<style scoped>
  .pluginView {
    display: flex;
    justify-content: center;
    padding-top: 1em;
  }
</style>
