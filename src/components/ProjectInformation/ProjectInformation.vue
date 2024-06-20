<script setup lang="ts">
  import { onMounted, computed, watch, inject, toRaw, reactive } from 'vue';
  import type { ProjectDetailedModel } from '@/models/Project';
  import {
    RightCircleFilled,
    EditOutlined,
    UserOutlined,
    LogoutOutlined,
    BarsOutlined,
    AppstoreAddOutlined,
  } from '@ant-design/icons-vue';
  import { projectsStoreSymbol } from '@/store/injectionSymbols';
  import { useProjectStore } from '@/store';
  import type { ComputedRef } from 'vue';
  import { storeToRefs } from 'pinia';
  import PluginView from '@/views/PluginView/PluginView.vue';

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

    const data: ComputedRef<ProjectDetailedModel | null> = computed(
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
  <div class="paneStyle">
    <a-button class="returnStyle" ghost @click="placeHolder">
      <template #icon
        ><RightCircleFilled
          style="color: black; font-size: 50px; border-radius: 50%"
        />
      </template>
    </a-button>

    <div class="mainStyle">
      <!-- create box for the project name -->
      <a-card class="nameBoxStyle" :loading="isLoading">
        <h1 class="projectNameH1" style="font-size: 2.8em; font-weight: bold">
          {{ projectData.projectName }}
        </h1>
        <!--<input
          v-else
          v-model="projectData.projectName"
          class="projectNameInput"
          type="input"
        />--->
        <!-- pencil icon for editing the project name -->
        <a-button class="editIconStyle" ghost @click="placeHolder">
          <template #icon
            ><EditOutlined style="color: black; font-size: 35px" />
          </template>
        </a-button>
      </a-card>

      <!-- create box for project description (BU, Team Nr, Department, Client Name) -->
      <a-row class="descboxStyle">
        <a-card class="profileFieldStyle" :style="profileFieldSize">
          <label for="businessUnit" style="font-size: 1.3em; font-weight: bold"
            >Business Unit:</label
          >
          <p v-if="!isLoading" style="font-size: 1.6em; margin: 0">
            {{ projectData.businessUnit }}
          </p>
          <a-skeleton v-else active :paragraph="false" />
        </a-card>

        <a-card class="profileFieldStyle" :style="profileFieldSize">
          <label for="teamNumber" style="font-size: 1.3em; font-weight: bold"
            >Team Number:</label
          >
          <p v-if="!isLoading" style="font-size: 1.6em; margin: 0">
            {{ projectData.teamNumber }}
          </p>
          <a-skeleton v-else active :paragraph="false" />
        </a-card>

        <a-card class="profileFieldStyle" :style="profileFieldSize">
          <label for="department" style="font-size: 1.3em; font-weight: bold"
            >Department:</label
          >
          <p v-if="!isLoading" style="font-size: 1.6em; margin: 0">
            {{ projectData.department }}
          </p>
          <a-skeleton v-else active :paragraph="false" />
        </a-card>

        <a-card class="profileFieldStyle" :style="profileFieldSize">
          <label for="clientName" style="font-size: 1.3em; font-weight: bold"
            >Client Name:</label
          >
          <p v-if="!isLoading" style="font-size: 1.6em; margin: 0">
            {{ projectData.clientName }}
          </p>
          <a-skeleton v-else active :paragraph="false" />
        </a-card>
      </a-row>
      <div v-if="!isLoading">
        <PluginView :project-i-d="projectData.id"></PluginView>
      </div>
    </div>

    <!-- add icons for profile, plugins, global logs, signout -->
    <a-col class="menuStyle">
      <a-button class="iconStyle" ghost @click="placeHolder">
        <template #icon
          ><UserOutlined style="color: black; font-size: 40px" />
        </template>
      </a-button>
      <a-button class="iconStyle" ghost @click="placeHolder">
        <template #icon
          ><AppstoreAddOutlined style="color: black; font-size: 40px" />
        </template>
      </a-button>
      <a-button class="iconStyle" ghost @click="placeHolder">
        <template #icon
          ><BarsOutlined style="color: black; font-size: 40px" />
        </template>
      </a-button>
      <a-button class="iconStyle" ghost @click="placeHolder">
        <template #icon
          ><LogoutOutlined style="color: black; font-size: 40px" />
        </template>
      </a-button>
    </a-col>
  </div>
</template>

<script lang="ts">
  // Flag for editable Title
  const projectData: ProjectDetailedModel = reactive({
    id: 0,
    projectName: '',
    businessUnit: '',
    teamNumber: 0,
    department: '',
    clientName: '',
  });

  // Place holder for the buttons for now
  const placeHolder = () => {
    console.log('Icon clicked');
  };

  //Function to load the data from projectViewService to projectView
  function addData(loadedData: ProjectDetailedModel) {
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
  .mainStyle {
    width: 60vw;
    max-height: 80vh;
    height: max-content;
    padding: 50px;
    margin: 10px;

    display: flex;
    flex-direction: column;
    align-items: center;
  }

  /* Style for the right panel */
  .paneStyle {
    display: flex;
    flex-direction: row;
    overflow: scroll;
    height: 100vh;
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

  /* Style for the return button */
  .returnStyle {
    cursor: pointer;
    height: 60px;
    width: 60px;
    margin: 20px;
    border: none;
  }

  /* Style for the Project title box */
  .nameBoxStyle {
    width: 85%;
    max-width: 750px;
    min-width: 250px;
    padding: 10px;
    margin: 10px;
    border-radius: 10px;

    background: white;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    text-align: center;
  }

  /* Style for the pencil button */
  .editIconStyle {
    cursor: pointer;
    position: absolute;
    right: 3%;
    width: 45px;
    height: 45px;
    top: 38%;
    padding: 0;
    border: none;
  }

  /* Style for the project description box */
  .descboxStyle {
    width: 85%;
    max-width: 750px;
    min-width: 250px;
    padding-bottom: 20px;
    margin: 10px;
    border-radius: 10px;

    background: white;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  }

  /* Sizing for the inside box in the project description box  */
  .profileFieldStyle {
    display: flex;
    justify-content: center;
    margin: 5px;
    height: 90px;
    border: none;
  }

  /* Style for the icons */
  .iconStyle {
    margin-bottom: 10px;
    height: 50px;
    width: 50px;
    border: none;
  }

  /* Style for the menu button on the top right */
  .menuStyle {
    display: flex;
    flex-direction: column;
    margin: 10px;
  }
</style>
