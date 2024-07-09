<script lang="ts" setup>
  import { inject, onMounted, toRaw, reactive } from 'vue';
  import { projectsStoreSymbol } from '@/store/injectionSymbols';
  import { useProjectStore } from '@/store';
  import { storeToRefs } from 'pinia';
  import type { DetailedProjectModel } from '@/models/Project';
  import type { ComputedRef } from 'vue';
  import { EditOutlined } from '@ant-design/icons-vue';

  const props = defineProps({
    isTest: {
      type: Boolean,
      default: false,
    },
  });

  const projectsStore = props.isTest
    ? useProjectStore()
    : inject(projectsStoreSymbol)!;

  const { getIsLoadingProject } = storeToRefs(projectsStore);
  const { getIsLoading } = storeToRefs(projectsStore);
  const isLoading = computed(
    () => getIsLoadingProject.value || getIsLoading.value,
  );

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
          @click="() => {}"
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
            alignItems: 'center',
            padding: '5px',
          }"
          class="infoCard"
        >
          <label class="label">Business&nbsp;Unit:</label>
          <p v-if="!isLoading" class="projectInfo">
            {{ projectData.businessUnit }}
          </p>
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
            alignItems: 'center',
            padding: '5px',
          }"
          class="infoCard"
        >
          <label class="label">Team&nbsp;Number:</label>
          <p v-if="!isLoading" class="projectInfo">
            {{ projectData.teamNumber }}
          </p>
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
            alignItems: 'center',
            padding: '5px',
          }"
          class="infoCard"
        >
          <label class="label">Department:</label>
          <p v-if="!isLoading" class="projectInfo">
            {{ projectData.department }}
          </p>
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
            alignItems: 'center',
            padding: '5px',
          }"
          class="infoCard"
        >
          <label class="label">Client&nbsp;Name:</label>
          <p v-if="!isLoading" class="projectInfo">
            {{ projectData.clientName }}
          </p>
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

<script lang="ts">
  const projectData: DetailedProjectModel = reactive({
    id: 0,
    projectName: '',
    businessUnit: '',
    teamNumber: 0,
    department: '',
    clientName: '',
  });

  //Function to load the data from projectViewService to projectView
  function addData(loadedData: DetailedProjectModel) {
    projectData.id = loadedData.id;
    projectData.projectName = loadedData.projectName;
    projectData.businessUnit = loadedData.businessUnit;
    projectData.teamNumber = loadedData.teamNumber;
    projectData.department = loadedData.department;
    projectData.clientName = loadedData.clientName;
  }
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

  .pluginView {
    padding: 0;
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
    display: flex;
    justify-content: center;
    align-items: center;
    flex-direction: row;
    flex-wrap: wrap;
    padding-top: 1em;
    padding-bottom: 1em;
    border-radius: 10px;

    background: white;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  }

  .infoCard {
    border: none;
    width: 50%;
    display: table;
    padding-left: 1em;
    padding-right: 1em;
    border: 2px solid red;
    align-items: left;
  }

  .button {
    margin-bottom: 10px;
    height: 50px;
    width: 50px;
    border: none;
  }
  .button {
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
    margin: 0 0 0 auto;
  }

  .projectInfo {
    font-size: 1.4em;
    margin: 0 auto 0 1em;
    white-space: nowrap;
  }
</style>
