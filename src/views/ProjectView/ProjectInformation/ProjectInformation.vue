<script lang="ts" setup>
  import { inject, onMounted, toRaw, reactive } from 'vue';
  import { projectsStoreSymbol, projectEditStoreSymbol } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';
  import type { DetailedProjectModel } from '@/models/Project';
  import type { ComputedRef } from 'vue';
  import { EditOutlined } from '@ant-design/icons-vue';
  import { useEditing } from '@/utils/hooks/useEditing';

  const projectsStore = inject(projectsStoreSymbol)!;
  const projectEditStore = inject(projectEditStoreSymbol)!;

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

  const toggleEditingMode = () => {
    if (isEditing.value === true) {
      stopEditing();
    } else {
      startEditing();
    }
  };

  const projectData: DetailedProjectModel = reactive({
    id: 0,
    projectName: '',
    businessUnit: '',
    teamNumber: 0,
    department: '',
    clientName: '',
  });

  const BUInputStatus = ref<'' | 'error' | 'warning' | undefined>('');
  const teamNumberInputStatus = ref<'' | 'error' | 'warning' | undefined>('');
  const departmentInputStatus = ref<'' | 'error' | 'warning' | undefined>('');
  const clientNameInputStatus = ref<'' | 'error' | 'warning' | undefined>('');

  const BUIInput = ref(projectData.businessUnit)
  const teamNumberInput = ref(projectData.teamNumber)
  const departmentInput = ref(projectData.department)
  const clientNameInput = ref(projectData.clientName)


  //Function to update the project information
  function updateProjectInformation() {
    const updatedProject: DetailedProjectModel = {
      id: projectData.id,
      projectName: projectData.projectName,
      businessUnit: projectData.businessUnit,
      teamNumber: projectData.teamNumber,
      department: projectData.department,
      clientName: projectData.clientName,
    };
    projectEditStore.updateProjectInformationChanges(updatedProject);
  }

  //Function to load the data from projectViewService to projectView
  function addData(loadedData: DetailedProjectModel) {
    projectEditStore.setInitialProjectInformation(loadedData)
    projectData.id = loadedData.id;
    projectData.projectName = loadedData.projectName;
    projectData.businessUnit = loadedData.businessUnit;
    projectData.teamNumber = loadedData.teamNumber;
    projectData.department = loadedData.department;
    projectData.clientName = loadedData.clientName;
  }

</script>

<template>
  <div class="pane">
    <div class="main">
      <!-- create box for the project name -->
      <div class="projectNameContainer">
        <h1 v-if="!isLoading" class="projectName">
          {{ projectData.projectName }}
        </h1>
        <a-skeleton v-else active :paragraph="false" style="max-width: 20em" />
        <a-button
          class="button"
          ghost
          style="margin-left: 10px"
          @click="toggleEditingMode"
        >
          <template #icon><EditOutlined class="icon" /></template>
        </a-button>
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
          }"
          class="infoCard"
        >
          <label class="label">Business&nbsp;Unit:</label>
          <template v-if="!isLoading">
            <p v-if="!isEditing" class="projectInfo">
              {{ projectData.businessUnit }}
            </p>
            <a-input
              v-else
              v-model:value="BUIInput"
              class="inputField"
              :status = "BUInputStatus"
              @input="updateProjectInformation"
              @change="() => {if(!projectData.businessUnit) BUInputStatus = 'error'; else BUInputStatus = '';}"
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
          }"
          class="infoCard"
        >
          <label class="label">Team&nbsp;Number:</label>
          <template v-if="!isLoading">
            <p v-if="!isEditing" class="projectInfo">
              {{ projectData.teamNumber }}
            </p>
            <a-form-item v-else>
              <a-input
                v-model:value="teamNumberInput"
                class="inputField"
                :status = "teamNumberInputStatus"
                @input="updateProjectInformation"
                @change="() => {if(!projectData.teamNumber) teamNumberInputStatus = 'error'; else teamNumberInputStatus = '';}"
              />
            </a-form-item>
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
          }"
          class="infoCard"
        >
          <label class="label">Department:</label>
          <template v-if="!isLoading">
            <p v-if="!isEditing" class="projectInfo">
              {{ projectData.department }}
            </p>
            <a-input
              v-else
              v-model:value="departmentInput"
              class="inputField"
              :status = "departmentInputStatus"
              @change="() => {if(!projectData.department) departmentInputStatus = 'error'; else departmentInputStatus = '';}"
              @input="updateProjectInformation"
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
          }"
          class="infoCard"
        >
          <label class="label">Client&nbsp;Name:</label>
          <template v-if="!isLoading">
            <p v-if="!isEditing" class="projectInfo">
              {{ projectData.clientName }}
            </p>
            <a-input
              v-else
              v-model:value="clientNameInput"
              class="inputField"
              :status = "clientNameInputStatus"
              @input="updateProjectInformation"
              @change="() => {if(!projectData.clientName) clientNameInputStatus = 'error'; else clientNameInputStatus = '';}"
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
<script lang="ts" setup>
  import { inject, onMounted, toRaw, reactive } from 'vue';
  import { projectsStoreSymbol } from '@/store/injectionSymbols';
  import { useProjectStore } from '@/store';
  import { storeToRefs } from 'pinia';
  import type { DetailedProjectModel } from '@/models/Project';
  import type { ComputedRef } from 'vue';
  import { EditOutlined } from '@ant-design/icons-vue';
  import { useEditing } from '@/utils/hooks/useEditing';

<script lang="ts">

</script>

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

    background: white;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  }

  .infoCard {
    border: none;
    width: 50%;
    display: table;
    padding: 0 1em 0 2vw;
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
    width: 180px;
    max-width: 100%;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
    flex: 1 !important; /* Wichtigkeit setzen, um Inline-Stile zu überschreiben */
    padding-left: 1em !important; /* Wichtigkeit setzen, um Inline-Stile zu überschreiben */
  }
</style>
