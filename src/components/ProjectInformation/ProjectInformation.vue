<script setup lang="ts">
  import { onMounted, computed, watch, inject, toRaw, reactive } from 'vue';
  import type { DetailedProjectModel } from '@/models/Project';
  import { EditOutlined } from '@ant-design/icons-vue';
  import { projectsStoreSymbol } from '@/store/injectionSymbols';
  import { useProjectStore } from '@/store';
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
    store = useProjectStore();
    store.fetchProject(100);
  } else {
    store = inject(projectsStoreSymbol)!;
  }

  const { getIsLoadingProject } = storeToRefs(store);
  const isLoading = computed(() => getIsLoadingProject.value);

  const profileFieldSize = computed(() => ({
    width: getWidth(props.paneWidth),
  }));

  // Fetch data when component is mounted
  onMounted(async () => {
    const project = store.getProject;
    if (project) addData(project);

    const data: ComputedRef<DetailedProjectModel | null> = computed(
      () => store.getProject,
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
  <button>RESET</button>
  <button>SAVE</button>
  <div class="pane">
    <div class="main">
      <!-- create box for the project name -->
      <div class="projectNameContainer" :loading="isLoading">
        <h1 class="projectName">
          {{ projectData.projectName }}
        </h1>
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
            alignItems: 'center',
            padding: '5px',
          }"
          class="infoCard"
          :style="profileFieldSize"
        >
          <label class="label">Business&nbsp;Unit:</label>
          <p v-if="!isLoading" class="projectInfo">
            {{ projectData.businessUnit }}
          </p>
          <a-skeleton v-else active :paragraph="false" />
        </a-card>

        <a-card
          :body-style="{
            display: 'flex',
            alignItems: 'center',
            padding: '5px',
          }"
          class="infoCard"
          :style="profileFieldSize"
        >
          <label class="label">Team&nbsp;Number:</label>
          <p v-if="!isLoading" class="projectInfo">
            {{ projectData.teamNumber }}
          </p>
          <a-skeleton v-else active :paragraph="false" />
        </a-card>
        <a-card
          :body-style="{
            display: 'flex',
            alignItems: 'center',
            padding: '5px',
          }"
          class="infoCard"
          :style="profileFieldSize"
        >
          <label class="label">Department:</label>
          <p v-if="!isLoading" class="projectInfo">
            {{ projectData.department }}
          </p>
          <a-skeleton v-else active :paragraph="false" />
        </a-card>
        <a-card
          :body-style="{
            display: 'flex',
            alignItems: 'center',
            padding: '5px',
          }"
          class="infoCard"
          :style="profileFieldSize"
        >
          <label class="label">Client&nbsp;Name:</label>
          <p v-if="!isLoading" class="projectInfo">
            {{ projectData.clientName }}
          </p>
          <a-skeleton v-else active :paragraph="false" />
        </a-card>
      </a-flex>
    </div>
  </div>
</template>

<script lang="ts">
  import { useEditing } from '@/utils/hooks/useEditing.tsx';

  const { isEditing, startEditing, stopEditing } = useEditing();

  // Flag for editable Title
  const projectData: DetailedProjectModel = reactive({
    id: 0,
    projectName: '',
    businessUnit: '',
    teamNumber: 0,
    department: '',
    clientName: '',
  });

  // Place holder for the buttons for now
  const placeHolder = () => {
    console.log('placeHolder');
    startEditing();
  };

  const toggleEditingMode = () => {
    if (isEditing.value === true) {
      stopEditing();
    } else {
      startEditing();
    }
  };

  //Function to load the data from projectViewService to projectView
  function addData(loadedData: DetailedProjectModel) {
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
    width: 100%;
    max-height: 80vh;
    height: max-content;
    //margin-top: 10px;
    //padding-top: 50px;
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
    height: max-content;
    margin: 10px;
    padding-top: 1em;
    padding-bottom: 1em;
    border-radius: 10px;

    background: white;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);

    display: flex;
    flex-wrap: wrap;
  }

  .infoCard {
    border: none;
    align-items: center;
    flex-direction: row;
    display: flex;
    justify-content: center;
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
    color: black; //TODO: change to appsfactory grey
    font-size: 2.5em;
  }

  .label {
    font-size: 1.4em;
    font-weight: bold;
  }

  .projectInfo {
    font-size: 1.4em;
    margin: 0;
  }
</style>
