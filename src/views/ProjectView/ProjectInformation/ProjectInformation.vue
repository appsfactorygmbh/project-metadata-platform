<script lang="ts" setup>
  import { computed, inject, onMounted, ref, toRaw } from 'vue';
  import {
    localLogStoreSymbol,
    pluginStoreSymbol,
    projectEditStoreSymbol,
    projectRoutingSymbol,
    projectsStoreSymbol,
  } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';
  import type {
    DetailedProjectModel,
    UpdateProjectModel,
  } from '@/models/Project';
  import type { ComputedRef } from 'vue';
  import {
    DeleteOutlined,
    EditOutlined,
    UndoOutlined,
    InboxOutlined
  } from '@ant-design/icons-vue';
  import { useEditing } from '@/utils/hooks/useEditing';
  import type { EditProjectModel } from '@/models/Project/EditProjectModel';
  import ConfirmAction from '@/components/Modal/ConfirmAction.vue';
  import ProjectButton from '@/components/Button/ProjectButton/ProjectButton.vue';

  const localLogStore = inject(localLogStoreSymbol);
  const projectsStore = inject(projectsStoreSymbol)!;
  const projectEditStore = inject(projectEditStoreSymbol)!;
  const pluginStore = inject(pluginStoreSymbol)!;
  const projectRouting = inject(projectRoutingSymbol)!;

  const editingClass = computed(() => ({
    'editing-mode': isEditing.value,
  }));

  const nonEditingClass = computed(() => ({
    'non-editing-mode': !isEditing.value,
  }));

  const { getIsLoadingProject } = storeToRefs(projectsStore);
  const { getIsLoading } = storeToRefs(projectsStore);
  const isLoading = computed(
    () => getIsLoadingProject.value || getIsLoading.value,
  );

  const { isEditing, stopEditing, startEditing } = useEditing();

  onMounted(async () => {
    const project = projectsStore.getProject;
    if (project) addData(project);

    const data: ComputedRef<DetailedProjectModel | null> = computed(
      () => projectsStore.getProject,
    );

    watch(
      () => data.value,
      (newProject, oldProject) => {
        if (!newProject) return;
        if (newProject.id !== oldProject?.id) {
          addData(toRaw(newProject));
        }
      },
    );
  });

  // set watcher for isEditing. if isEditing is false reset the input status fields
  watch(
    () => isEditing.value,
    (newVal) => {
      if (!newVal) {
        projectEditStore.resetPluginChanges();
        BUInputStatus.value = '';
        teamNumberInputStatus.value = '';
        departmentInputStatus.value = '';
        clientNameInputStatus.value = '';
        addData(projectsStore.getProject!);
      }
    },
  );

  const toggleEditingMode = async () => {
    if (isEditing.value) {
      await stopEditing();
    } else {
      await startEditing();
    }
  };

  const projectData = {
    projectName: ref<string>(''),
    businessUnit: ref<string>(''),
    teamNumber: ref<number>(0),
    department: ref<string>(''),
    clientName: ref<string>(''),
    isArchived: ref<boolean>(false),
  };

  const BUInputStatus = ref<'' | 'error' | 'warning' | undefined>('');
  const teamNumberInputStatus = ref<'' | 'error' | 'warning' | undefined>('');
  const departmentInputStatus = ref<'' | 'error' | 'warning' | undefined>('');
  const clientNameInputStatus = ref<'' | 'error' | 'warning' | undefined>('');

  const BUInput = ref(projectData.businessUnit);
  const teamNumberInput = ref(projectData.teamNumber);
  const departmentInput = ref(projectData.department);
  const clientNameInput = ref(projectData.clientName);

  //Function to update the project information
  function updateProjectInformation(): void {
    const updatedProject: EditProjectModel = {
      projectName: projectData.projectName.value,
      businessUnit: BUInput.value,
      teamNumber: teamNumberInput.value,
      department: departmentInput.value,
      clientName: clientNameInput.value,
    };
    projectEditStore.updateProjectInformationChanges(updatedProject);
  }

  //Function to load the data from projectViewService to projectView
  function addData(loadedData: DetailedProjectModel) {
    if (projectsStore.getProject)
      projectEditStore.setProjectInformation(projectsStore.getProject);
    projectData.projectName.value = loadedData.projectName;
    projectData.businessUnit.value = loadedData.businessUnit;
    projectData.teamNumber.value = loadedData.teamNumber;
    projectData.department.value = loadedData.department;
    projectData.clientName.value = loadedData.clientName;
  }

  const isArchiveModalOpen = ref(false);
  const isDeleteModalOpen = ref(false);

  const handleArchive = () => {
    isArchiveModalOpen.value = true;
  };

  const handleDelete = async () => {
    isDeleteModalOpen.value = true;
  };

  const confirmDelete = async () => {
    const project = projectsStore.getProject;

    if (!project?.id) {
      isDeleteModalOpen.value = false;
      return;
    }

    try {
      const response = await projectsStore.deleteProject(project.id);

      if (response?.ok) {
        const nextProject = projectsStore.getProject;

        if (nextProject) {
          projectRouting.setProjectId(nextProject.id);
        }
      }
    } catch (error) {
      console.error('Error deleting project:', error);
    } finally {
      isDeleteModalOpen.value = false;
    }
  };

  const getNextActiveProjectId = (currentProjectId: number): number => {
    const projects = projectsStore.getProjects;
    const nextProject = projects.find((project) => !project.isArchived);
    if (!nextProject) return currentProjectId;
    return nextProject.id;
  };

  const confirmArchive = async () => {
    const projectID = projectsStore?.getProject?.id;
    const projectData = projectsStore?.getProject as UpdateProjectModel;
    projectData.pluginList = pluginStore?.getPlugins;

    if (projectID) {
      try {
        await projectsStore.archiveProject(projectData, projectID);
        if (projectsStore.getUpdatedSuccessfully) {
          await projectsStore.fetchProjects();
        }
      } finally {
        isArchiveModalOpen.value = false;
        const newProjectId = getNextActiveProjectId(projectID);
        projectRouting.setProjectId(newProjectId);
      }
    }
  };

  const reactivateProject = async () => {
    const currentProject = projectsStore.getProject! as UpdateProjectModel;
    const projectId = projectsStore.getProject?.id;
    currentProject.pluginList = pluginStore.getPlugins;

    await projectsStore.activateProject(currentProject, projectId!);
    await projectsStore.fetchProjects();
    await localLogStore?.fetchLocalLog(projectId!);
  };
</script>

<template>
  <div class="pane">
    <div class="main">
      <!-- create box for the project name -->
      <div class="projectNameContainer">
        <h1 v-if="!isLoading" class="projectName">
          {{ projectData.projectName.value }}
        </h1>
        <a-skeleton v-else active :paragraph="false" style="max-width: 20em" />

        <!-- Edit Button -->
        <ProjectButton
          v-if="!projectsStore.getProject?.isArchived"
          @click="toggleEditingMode"
        >
          <template #icon>
            <EditOutlined class="icon" />
          </template>
        </ProjectButton>

        <!-- Reactivate Button -->
        <a-tooltip
          v-if="projectsStore.getProject?.isArchived"
          position="left"
          title="Click here to reactivate the project"
          style="padding-left: 0; padding-right: 0"
        >
          <ProjectButton @click="reactivateProject">
            <template #icon>
              <UndoOutlined class="icon" />
            </template>
          </ProjectButton>
        </a-tooltip>

        <!-- Delete Button -->
        <a-tooltip
          v-if="projectsStore.getProject?.isArchived"
          position="right"
          title="Click here to delete the project"
          style="padding-left: 0; padding-right: 0"
        >
          <ProjectButton @click="handleDelete">
            <template #icon>
              <DeleteOutlined class="icon" />
            </template>
          </ProjectButton>

          <ConfirmAction
            :is-open="isDeleteModalOpen"
            title="Delete Project"
            message="Are you sure you want to delete this project permanently?"
            @confirm="confirmDelete"
            @cancel="isDeleteModalOpen = false"
            @update:is-open="(value) => (isDeleteModalOpen = value)"
          />
        </a-tooltip>

        <!-- Archive Button -->
        <a-tooltip
          v-if="!projectsStore.getProject?.isArchived"
          position="left"
          title="Click here to archive the project"
          style="padding-left: 0; padding-right: 0"
        >
          <ProjectButton @click="handleArchive">
            <template #icon>
              <InboxOutlined class="icon"/>
            </template>
          </ProjectButton>
        </a-tooltip>

        <ConfirmAction
          :is-open="isArchiveModalOpen"
          title="Archive Project"
          message="Are you sure you want to archive this project?"
          @confirm="confirmArchive"
          @cancel="isArchiveModalOpen = false"
          @update:is-open="(value) => (isArchiveModalOpen = value)"
        />
      </div>

      <!-- create box for project description (BU, Team Nr, Department, Client Name) -->
      <a-flex
        class="projectInformationBox"
        :body-style="{
          height: 'fit-content',
        }"
      >
        <a-card
          :body-style="{
            display: 'flex',
            padding: '5px',
            alignItems: 'center',
          }"
          class="infoCard"
          :class="[editingClass, nonEditingClass]"
        >
          <label class="label">Business&nbsp;Unit:</label>
          <template v-if="!isLoading">
            <p v-if="!isEditing" class="projectInfo">
              {{ projectData.businessUnit.value }}
            </p>
            <a-input
              v-else
              v-model:value="BUInput"
              class="inputField"
              :status="BUInputStatus"
              @input="updateProjectInformation"
              @change="
                () => {
                  if (!BUInput) {
                    BUInputStatus = 'error';
                    projectEditStore.addEmptyProjectInformationField('BU');
                  } else {
                    BUInputStatus = '';
                    projectEditStore.removeEmptyProjectInformationField('BU');
                  }
                }
              "
            />
          </template>
          <a-skeleton
            v-else
            active
            :paragraph="false"
            style="padding-left: 1em"
          />
        </a-card>

        <a-card
          :body-style="{
            display: 'flex',
            padding: '5px',
            alignItems: 'center',
          }"
          class="infoCard"
          :class="[editingClass, nonEditingClass]"
        >
          <label class="label">Team&nbsp;Number:</label>
          <template v-if="!isLoading">
            <p v-if="!isEditing" class="projectInfo">
              {{ projectData.teamNumber.value }}
            </p>
            <a-input
              v-else
              v-model:value="teamNumberInput"
              class="inputField"
              :status="teamNumberInputStatus"
              @input="updateProjectInformation"
              @change="
                () => {
                  if (!teamNumberInput || isNaN(teamNumberInput)) {
                    teamNumberInputStatus = 'error';
                    projectEditStore.addEmptyProjectInformationField(
                      'teamNumber',
                    );
                  } else {
                    teamNumberInputStatus = '';
                    projectEditStore.removeEmptyProjectInformationField(
                      'teamNumber',
                    );
                  }
                }
              "
            />
          </template>
          <a-skeleton
            v-else
            active
            :paragraph="false"
            style="padding-left: 1em"
          />
        </a-card>

        <a-card
          :body-style="{
            display: 'flex',
            padding: '5px',
            alignItems: 'center',
          }"
          class="infoCard"
          :class="[editingClass, nonEditingClass]"
        >
          <label class="label">Department:</label>
          <template v-if="!isLoading">
            <p v-if="!isEditing" class="projectInfo">
              {{ projectData.department.value }}
            </p>
            <a-input
              v-else
              v-model:value="departmentInput"
              class="inputField"
              :status="departmentInputStatus"
              @input="updateProjectInformation"
              @change="
                () => {
                  if (!departmentInput) {
                    departmentInputStatus = 'error';
                    projectEditStore.addEmptyProjectInformationField(
                      'department',
                    );
                  } else {
                    departmentInputStatus = '';
                    projectEditStore.removeEmptyProjectInformationField(
                      'department',
                    );
                  }
                }
              "
            />
          </template>
          <a-skeleton
            v-else
            active
            :paragraph="false"
            style="padding-left: 1em"
          />
        </a-card>

        <a-card
          :body-style="{
            display: 'flex',
            padding: '5px',
            alignItems: 'center',
          }"
          class="infoCard"
          :class="[editingClass, nonEditingClass]"
        >
          <label class="label">Client&nbsp;Name:</label>
          <template v-if="!isLoading">
            <p v-if="!isEditing" class="projectInfo">
              {{ projectData.clientName.value }}
            </p>
            <a-input
              v-else
              v-model:value="clientNameInput"
              class="inputField"
              :status="clientNameInputStatus"
              @input="updateProjectInformation"
              @change="
                () => {
                  if (!clientNameInput) {
                    clientNameInputStatus = 'error';
                    projectEditStore.addEmptyProjectInformationField(
                      'clientName',
                    );
                  } else {
                    clientNameInputStatus = '';
                    projectEditStore.removeEmptyProjectInformationField(
                      'clientName',
                    );
                  }
                }
              "
            />
          </template>
          <a-skeleton
            v-else
            active
            :paragraph="false"
            style="padding-left: 1em"
          />
        </a-card>
      </a-flex>
    </div>
  </div>
</template>

<style scoped lang="scss">
  /* Style for the middle section */
  .main {
    width: 100%;
    max-height: 80vh;
    height: max-content;
    padding-right: 5em;
    padding-left: 5em;

    display: flex;
    flex-direction: column;
    align-items: center;
  }

  /* Style for the right panel */
  .pane {
    display: flex;
    flex-direction: row;
  }

  /* Style for the Project name input box */
  .projectNameInput {
    font-size: 2.8em;
    width: 80%;
    height: 2.8em;
    text-align: center;
    border: none;
    border-bottom: 2px solid #a5a4a4;
    color: black;
    background-color: rgb(250, 250, 250);
  }

  /* Style for the Project title box */
  .projectNameContainer {
    width: 100%;
    height: 5%;
    margin: 10px;
    border-radius: 10px;
    text-align: center;
    align-items: center;
    flex-direction: row;
    display: flex;
    justify-content: center;
  }

  .projectName {
    font-size: 2.5em;
    font-weight: bold;
    color: #000;
    margin: 10px;
  }

  .projectInformationBox {
    width: 100%;
    height: auto;
    justify-content: start;
    flex-direction: row;
    flex-wrap: wrap;
    padding: 1em 0;
    border-radius: 10px;
    container-type: inline-size;
    background: white;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  }

  @container (max-width: 53vw) {
    .infoCard.editing-mode {
      width: 100% !important;
    }
  }

  @container (max-width: 45vw) {
    .infoCard.non-editing-mode {
      width: 100% !important;
    }
  }

  .infoCard {
    border: none;
    width: 50%;
    display: table;
    padding: 0 1em 0 1em;
    max-width: 100%;
  }

  .button {
    margin-bottom: 10px;
    height: 40px;
    width: 40px;
    border: none;
  }

  .icon {
    color: black;
    font-size: 1.5em;
  }

  .label {
    font-size: 1.4em;
    font-weight: bold;
    margin: 0;
  }

  .projectInfo {
    font-size: 1.4em;
    margin: 0 auto 0 0.5em;
    white-space: nowrap;
  }
  .inputField {
    margin-left: 1em;
    max-width: 100%;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
    flex: 1 !important; /* Set to important to override inline style */
    padding-left: 1em !important; /* Set to important to override inline style */
  }
</style>
