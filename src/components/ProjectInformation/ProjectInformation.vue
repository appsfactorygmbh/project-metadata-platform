<script setup lang="ts">
  import { onMounted, computed, watch, inject, toRaw, reactive } from 'vue';
  import type { ProjectInformationModel } from '@/models/ProjectInformationModel';
  import {
    RightCircleFilled,
    EditOutlined,
    UserOutlined,
    LogoutOutlined,
    BarsOutlined,
    AppstoreAddOutlined,
  } from '@ant-design/icons-vue';
  import { projectInformationStoreSymbol } from '@/store/injectionSymbols';
  import { ProjectInformationStore } from '@/store/ProjectInformationStore';
  import type { ComputedRef } from 'vue';
  import { storeToRefs } from 'pinia';

  //Get the width of the right pane from App.vue
  const props = defineProps({
    paneWidth: {
      type: Number,
      required: true,
    },
    isTest: {
      type: Boolean,
      default: false,
    },
  });

  watch(
    () => props.paneWidth,
    () => {
      getWidth(props.paneWidth);
    },
  );

  let store;

  if (props.isTest) {
    store = ProjectInformationStore();
    store.fetchProjectInformation(100);
  } else {
    store = inject(projectInformationStoreSymbol)!;
  }

  const { isLoading } = storeToRefs(store);

  const profileFieldSize = computed(() => ({
    width: getWidth(props.paneWidth),
  }));

  // Fetch data when component is mounted
  onMounted(async () => {
    addData(store.getProjectInformation);

    const data: ComputedRef<ProjectInformationModel> = computed(
      () => store.getProjectInformation,
    );

    watch(
      () => data.value,
      (newProject, oldProject) => {
        if (newProject.id !== oldProject.id) {
          addData(toRaw(newProject));
        }
      },
    );
  });
</script>

<template>
  <div class="pane">
    <a-button class="button" ghost @click="placeHolder">
      <template #icon><RightCircleFilled class="icon"/></template>
    </a-button>

    <div class="main">
      <!-- create box for the project name -->
      <div class="projectNameContainer" :loading="isLoading">
        <h1 class="projectName">
          {{ projectData.projectName }}
        </h1>
        <a-button class="button" ghost @click="placeHolder">
          <template #icon><EditOutlined class="icon"/></template>
        </a-button>
      </div>

      <!-- create box for project description (BU, Team Nr, Department, Client Name) -->
      <a-row class="projectInformationBox">
        <a-card class="infoCard" :style="profileFieldSize">
          <label class="label">Business Unit</label>
          <p v-if="!isLoading" class="projectInfo">
            {{ projectData.businessUnit }}
          </p>
          <a-skeleton v-else active :paragraph="false" />
        </a-card>

        <a-card class="infoCard" :style="profileFieldSize">
          <label class="label">Team Number</label>
          <p v-if="!isLoading" class="projectInfo">
            {{ projectData.teamNumber }}
          </p>
          <a-skeleton v-else active :paragraph="false" />
        </a-card>

        <a-card class="infoCard" :style="profileFieldSize">
          <label class="label">Department</label>
          <p v-if="!isLoading" class="projectInfo">
            {{ projectData.department }}
          </p>
          <a-skeleton v-else active :paragraph="false" />
        </a-card>

        <a-card class="infoCard" :style="profileFieldSize">
          <label class="label">Client Name</label>
          <p v-if="!isLoading" class="projectInfo">
            {{ projectData.clientName }}
          </p>
          <a-skeleton v-else active :paragraph="false" />
        </a-card>
      </a-row>
    </div>

    <!-- add icons for profile, plugins, global logs, signout -->
    <a-col class="menu">
      <a-button class="button" ghost @click="placeHolder">
        <template #icon
          ><UserOutlined class="icon"/>
        </template>
      </a-button>
      <a-button class="button" ghost @click="placeHolder">
        <template #icon
          ><AppstoreAddOutlined class="icon"/>
        </template>
      </a-button>
      <a-button class="button" ghost @click="placeHolder">
        <template #icon
          ><BarsOutlined class="icon"/>
        </template>
      </a-button>
      <a-button class="button" ghost @click="placeHolder">
        <template #icon
          ><LogoutOutlined class="icon"/>
        </template>
      </a-button>
    </a-col>
  </div>
</template>

<script lang="ts">
  // Flag for editable Title
  const projectData: ProjectInformationModel = reactive({
    id: 0,
    projectName: '',
    businessUnit: '',
    teamNumber: '',
    department: '',
    clientName: '',
  });

  // Place holder for the buttons for now
  const placeHolder = () => {
    console.log('Icon clicked');
  };

  //Function to load the data from projectViewService to projectView
  function addData(loadedData: ProjectInformationModel) {
    projectData.id = loadedData.id;
    projectData.projectName = loadedData.projectName;
    projectData.businessUnit = loadedData.businessUnit;
    projectData.teamNumber = loadedData.teamNumber;
    projectData.department = loadedData.department;
    projectData.clientName = loadedData.clientName;
  }

  const getWidth = (pwidth: number) => {
    switch (getBreakpoint(pwidth)) {
      case 'lg':
        return '48%';

      case 'sm':
        return '100%';
    }
  };

  function getBreakpoint(pwidth: number): string {
    const breakpoint: number[] = [978];
    if (pwidth >= breakpoint[0]) {
      return 'lg';
    } else {
      return 'sm';
    }
  }
</script>

<style scoped lang="scss">
  /* Style for the middle section */
  .main {
    width: 60vw;
    height: 80vh;
    padding: 50px;
    margin: 10px;

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
  .projectNameInput                                                                                                                                                                                                  {
    font-size: 2.8em;
    width: 80%;
    height: 2.8em;
    text-align: center;
    border: none;
    border-bottom: 2px solid #a5a4a4;
    color: black;
    background-color: rgb(250, 250, 250);
  }

  /* Style for the return button */
  .return {
    cursor: pointer;
    height: 60px;
    width: 60px;
    margin: 20px;
    border: none;
  }

  /* Style for the Project title box */
  .projectNameContainer {
    width: 85%;
    max-width: 750px;
    min-width: 250px;
    padding: 10px;
    margin: 10px;
    border-radius: 10px;
    align-items: center;
    //background: white;
    //box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    text-align: center;
  }

  .projectName {
    font-size: 2.8em;
    font-weight: bold;
  }

  .projectInformationBox {
    width: 110%;
    //max-width: 90%;
    min-width: 250px;
    padding-bottom: 20px;
    margin: 10px;
    border-radius: 10px;

    background: white;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  }

  .infoCard {
    display: flex;
    align-items: center;
    justify-content: center;
    margin: 5px;
    height: 50px;
    border: none;
  }

  .button {
    margin-bottom: 10px;
    height: 50px;
    width: 50px;
    border: none;
  }

  .icon{
    color: black;   //TODO: change to appsfactory grey
    font-size: 30px;
  }

  .label {
    font-size: 1.3em;
    font-weight: bold;
  }

  .projectInfo{
    font-size: 1.6em;
    margin: 0;
  }

  .menu {
    display: flex;
    flex-direction: column;
    margin: 10px;
  }
</style>
