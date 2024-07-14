<script lang="ts" setup>
  import { ProjectPlugins } from './ProjectPlugins';
  import { ProjectInformation } from './ProjectInformation';
  import type {
    DetailedProjectModel,
    UpdateProjectModel,
  } from '@/models/Project';
  import ProjectEditButtons from '@/components/ProjectEditButtons/ProjectEditButtons.vue';
  import { useEditing } from '@/utils/hooks/useEditing';
  import {
    pluginStoreSymbol,
    projectsStoreSymbol,
    projectEditStoreSymbol,
  } from '@/store/injectionSymbols';
  import { inject, watch } from 'vue';
  import { message } from 'ant-design-vue';

  const pluginStore = inject(pluginStoreSymbol);
  const projectStore = inject(projectsStoreSymbol);
  const projectEditStore = inject(projectEditStoreSymbol);

  const { isEditing, stopEditing } = useEditing();

  const reloadEditStore = () => {
    if (pluginStore) {
      for (let i = 0; i < pluginStore?.getPlugins.length; i++) {
        projectEditStore?.initialAdd(pluginStore?.getPlugins[i]);
      }
    }
  };

  watch(
    () => isEditing,
(newVal) => {
      if (newVal) {
        projectEditStore?.resetChanges();
      }
    },

  )

  const cancelEdit = () => {
    projectEditStore?.resetChanges();
    reloadEditStore();
    stopEditing();
  };

  const isAdding = computed(() => projectStore?.getIsLoadingAdd);

  const saveEdit = async () => {
    if (!projectEditStore?.getCanBeAdded) {
      message.error(
        'Could not update Project. There are empty fields or duplicated plugins.',
        7,
      );
      return;
    }

    watch(isAdding, (newVal) => {
      if (newVal == false) {
        if (projectStore?.getAddedSuccessfully) {
          message.success('Project updated successfully.', 7);
        } else {
          message.error('Could not update Project.', 7);
        }
      }
    });

    const updateProjectInformation: DetailedProjectModel | null =
      projectStore?.getProject || null;
    const updatedProject: UpdateProjectModel = {
      projectName: updateProjectInformation?.projectName,
      businessUnit: updateProjectInformation?.businessUnit,
      teamNumber: updateProjectInformation?.teamNumber,
      department: updateProjectInformation?.department,
      clientName: updateProjectInformation?.clientName,
      pluginList: projectEditStore?.getPluginChanges,
    };
    console.log('updated Project', updatedProject);
    const projectID = computed(() => projectStore?.getProject?.id);
    if (projectID.value) {
      await projectStore?.updateProject(updatedProject, projectID.value);
      await pluginStore?.fetchPlugins(projectID.value);
    }
    projectEditStore?.resetChanges();
    stopEditing();
  };
</script>

<template>
  <ProjectEditButtons v-if="isEditing" @cancel="cancelEdit" @save="saveEdit" />
  <ProjectInformation />
  <ProjectPlugins class="pluginView" />
</template>

<style scoped>
  .pluginView {
    display: flex;
    justify-content: center;
    padding-top: 1em;
  }
</style>
