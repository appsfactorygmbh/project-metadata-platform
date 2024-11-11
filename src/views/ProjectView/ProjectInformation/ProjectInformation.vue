<script lang="ts" setup>
  import { inject, onMounted, toRaw } from 'vue';
  import {
    projectEditStoreSymbol,
    projectsStoreSymbol,
  } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';
  import type { DetailedProjectModel } from '@/models/Project';
  import type { ComputedRef } from 'vue';
  import { EditOutlined, UndoOutlined } from '@ant-design/icons-vue';
  import { useEditing } from '@/utils/hooks/useEditing';
  import type { EditProjectModel } from '@/models/Project/EditProjectModel';

  const projectsStore = inject(projectsStoreSymbol)!;
  const projectEditStore = inject(projectEditStoreSymbol)!;

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

  // set watcher for isEditing. if is editing is false reset the inputstatus fields
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
    if (isEditing.value === true) {
      await stopEditing();
    } else {
      await startEditing();
    }
  };

  const reactivateProject = async () => {
    const currentProject = projectsStore.getProject!;
    const projectId = currentProject.id;
    await projectsStore.activateProject(currentProject, projectId);
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
    console.log('project updates: ', updatedProject);
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
        <a-button
          v-if="!projectsStore.getProject?.isArchived"
          class="button"
          ghost
          style="margin-left: 10px"
          @click="toggleEditingMode"
        >
          <template #icon><EditOutlined class="icon" /></template>
        </a-button>
        <a-tooltip
          v-else
          position="left"
          title="Click here to reactivate the project"
          style="padding-left: 0; padding-right: 0"
        >
          <a-button
            class="button"
            ghost
            style="margin-left: 10px"
            @click="reactivateProject"
          >
            <template #icon><UndoOutlined class="icon" /></template>
          </a-button>
        </a-tooltip>
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
    font-size: 2.5em;
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
