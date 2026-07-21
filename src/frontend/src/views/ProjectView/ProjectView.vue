<script lang="ts" setup>
  import { ProjectPlugins } from './ProjectPlugins';
  import { LocalLogView } from './LocalLogView';
  import { ProjectInformation } from './ProjectInformation';
  import type { UpdateProjectModel } from '@/models/Project';
  import ProjectEditButtons from '@/components/ProjectEditButtons/ProjectEditButtons.vue';
  import { useEditing } from '@/utils/hooks/useEditing';
  import {
    projectEditStoreSymbol,
    localLogStoreSymbol,
    projectRoutingSymbol,
  } from '@/store/injectionSymbols';
  import { inject, ref, watch } from 'vue';
  import { message } from 'ant-design-vue';
  import { usePluginStore, useProjectStore } from '@/store';
  import type { PluginModel } from '@/models/Plugin';
  import _ from 'lodash';
  import { AddPluginView } from '@/views/ProjectView/ProjectPlugins/AddPlugin';
  import { useThemeToken } from '@/utils/hooks';
  import { ResourceActions } from '@/models/utils';

  const token = useThemeToken();

  const localLogStore = inject(localLogStoreSymbol);
  const projectEditStore = inject(projectEditStoreSymbol);

  const projectRouting = inject(projectRoutingSymbol)!;

  const hasActiveProject = computed(() => {
    return !!projectRouting?.routerProjectId?.value;
  });

  const pluginStore = usePluginStore();
  const projectStore = useProjectStore();
  const rerenderPlugins = ref(1);
  const isModalOpen = ref(false);
  const isArchiveModalOpen = ref(false);
  const openModal = () => {
    isModalOpen.value = true;
  };
  const { isEditing, stopEditing } = useEditing();

  const reloadEditStore = () => {
    if (pluginStore) {
      pluginStore.getPlugins.forEach((plugin) => {
        projectEditStore?.initialAdd(plugin);
      });
    }
  };

  watch(
    () => isEditing.value,
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
    rerenderPlugins.value++;
  };

  const isEmpty = ref(false);
  const setIsEmpty = (value: boolean) => {
    isEmpty.value = value;
  };

  watch(
    () => projectStore.getProjects,
    () => {
      if (projectStore.getProjects.length === 0) {
        setIsEmpty(true);
      } else {
        setIsEmpty(false);
      }
    },
  );

  const isAdding = computed(() => projectStore.getIsLoadingUpdate);

  // Watcher to see if fetch was successful
  watch(isAdding, (newVal) => {
    if (!newVal) {
      if (projectStore.getUpdatedSuccessfully) {
        projectEditStore?.resetPluginChanges();
        message.success('Project updated successfully.', 2);
        projectStore.fetch(projectStore.getProject?.id ?? 0);
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

    if (!projectStore.getProject) {
      console.error(
        'Error when trying to get ProjectInformation. getProject is undefined',
      );
      return;
    }

    const updateProjectInformation =
      projectEditStore.getProjectInformationChanges;

    // Puts the unarchived plugins and the archived plugins together
    const updatedPluginList = computed(() => {
      const tempPluginList: PluginModel[] = [];
      projectEditStore.getPluginChanges.forEach((plugin) => {
        tempPluginList.push({
          id: plugin.id,
          pluginName: plugin.pluginName,
          displayName: plugin.displayName,
          url: plugin.url,
        });
      });

      const archivedPlugins = _.differenceBy(
        pluginStore.getPlugins,
        pluginStore.getUnarchivedPlugins,
        'id',
      );

      archivedPlugins.forEach((plugin) => {
        tempPluginList.push({
          id: plugin.id,
          pluginName: plugin.pluginName,
          displayName: plugin.displayName,
          url: plugin.url,
        });
      });
      return tempPluginList;
    });

    const updatedProject: UpdateProjectModel = {
      projectName: updateProjectInformation?.projectName,
      teamId: updateProjectInformation?.teamId,
      clientName: updateProjectInformation?.clientName,
      pluginList: updatedPluginList.value,
      isArchived: projectStore.getProject.isArchived,
      offerId: updateProjectInformation?.offerId,
      companyId: updateProjectInformation?.companyId,
      companyState: updateProjectInformation?.companyState,
      ismsLevel: updateProjectInformation?.ismsLevel,
      isEoC: updateProjectInformation?.isEoC,
      notes: updateProjectInformation?.notes,
    };
    console.log(updatedProject);

    const projectID = computed(() => projectStore.getProject?.id);
    if (projectID.value) {
      await projectStore.update(projectID.value, updatedProject);
      await projectStore.fetchAll();
      await projectStore.fetch(projectID.value);
      await pluginStore.fetchUnarchived(projectID.value);
      await pluginStore.fetch(projectID.value);
      await localLogStore?.fetch(projectID.value);
    }
    closeAddPluginModal();
  };

  // Blur effect
  const isBlurred = ref(false);

  function setBlur(state: boolean) {
    isBlurred.value = state;
  }

  const openAddPluginModal = ref<boolean>(false);

  const closeAddPluginModal = () => {
    openAddPluginModal.value = false;
  };

  const getNextActiveProjectId = (currentid?: number): number | undefined => {
    const projects = projectStore.getProjects;
    const nextProject = projects.find(
      (project) => !project.isArchived && project.id != currentid,
    );
    if (!nextProject) return undefined;
    return nextProject.id;
  };

  const handleArchive = () => {
    isArchiveModalOpen.value = true;
  };

  const confirmArchive = async () => {
    const projectID = projectStore?.getProject?.id;
    const detailedProject = projectStore?.getProject;

    if (!detailedProject) {
      throw new Error('No project found to update');
    }

    const projectData: UpdateProjectModel = {
      projectName: detailedProject.projectName,
      clientName: detailedProject.clientName,
      offerId: detailedProject.offerId,

      companyId: detailedProject.company.id,
      teamId: detailedProject.team ? detailedProject.team.id : null,

      companyState: detailedProject.companyState,
      ismsLevel: detailedProject.ismsLevel,
      isEoC: detailedProject.isEoC,
      notes: detailedProject.notes,
      isArchived: detailedProject.isArchived,

      pluginList: null,
    };
    projectData.pluginList = pluginStore?.getPlugins;

    if (projectID) {
      try {
        await projectStore.archive(projectID);
        await projectStore.fetchAll();
      } finally {
        isArchiveModalOpen.value = false;
        isModalOpen.value = false;
        projectEditStore?.resetPluginChanges();
        stopEditing();
        await localLogStore?.fetch(projectID);
        const newProjectId = getNextActiveProjectId(projectID);
        if (!newProjectId) projectRouting.setProjectId(undefined);
        projectRouting.setProjectId(newProjectId);
      }
    }
  };
</script>

<template>
  <div v-if="hasActiveProject && !isEmpty">
    <ProjectEditButtons
      v-if="isEditing"
      :can-edit="
        projectStore.getProject?.permissions?.includes(ResourceActions.Edit)
      "
      @cancel="openModal"
      @archive="handleArchive"
      @save="saveEdit"
    />
    <ProjectInformation />
    <ProjectPlugins
      :key="rerenderPlugins"
      class="pluginView"
      @set-blur="setBlur"
    />
    <AddPluginView
      v-if="openAddPluginModal"
      :show-modal="openAddPluginModal"
      @added-plugin="async () => await saveEdit()"
    />
    <LocalLogView class="LocalLog" :class="{ blur: isBlurred }" />
    <ConfirmAction
      :is-open="isModalOpen"
      title="Cancel Editing"
      message="Are you sure you want to cancel all changes?"
      @confirm="cancelEdit"
      @cancel="isModalOpen = false"
      @update:is-open="(value) => (isModalOpen = value)"
    />
    <ConfirmAction
      :is-open="isArchiveModalOpen"
      title="Archive Project"
      message="Are you sure you want to archive this project?"
      @confirm="confirmArchive"
      @cancel="isArchiveModalOpen = false"
      @update:is-open="(value) => (isArchiveModalOpen = value)"
    />
  </div>
  <a-flex v-else justify="center" align="center" class="empty-state-container">
    <a-empty
      :description="isEmpty ? 'No project found' : 'No project selected'"
    />
  </a-flex>
</template>

<style scoped>
  .pluginView {
    display: flex;
    justify-content: center;
    padding-top: 10vh;
  }

  .LocalLog {
    margin: 2em 3.5em 2em 3em;
  }

  .empty-state-container {
    height: 80vh;
    color: v-bind('token.colorText');
  }

  .addPlugin {
    margin-bottom: 60px;
  }
</style>
