<script setup lang="ts">
  import {
    onMounted,
    computed,
    ref,
    watch,
    inject,
    toRaw,
    reactive,
  } from 'vue';
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

  // Style for the return button
  const returnStyle = {
    cursor: 'pointer',
    height: '60px',
    width: '60px',
    margin: '20px',
    border: 'none',
  };

  // Style for the Project title box
  const nameBoxStyle = {
    width: '85%',
    maxWidth: '750px',
    minWidth: '250px',
    padding: '10px',
    margin: '10px',
    borderRadius: '10px',

    background: 'white',
    boxShadow: '0 2px 4px rgba(0, 0, 0, 0.1)',
    textAlign: 'center',
  };

  // Style for the pencil button
  const editIconStyle = {
    cursor: 'pointer',
    position: 'absolute',
    right: '3%',
    width: '45px',
    height: '45px',
    top: '38%',
    padding: '0',
    border: 'none',
  };

  // Style for the project description box
  const descboxStyle = {
    width: '85%',
    maxWidth: '750px',
    minWidth: '250px',
    padding: '20px',
    margin: '10px',
    borderRadius: '10px',

    background: 'white',
    boxShadow: '0 2px 4px rgba(0, 0, 0, 0.1)',
  };

  // Sizing for the inside box in the project description box
  const profileFieldStyle = computed(() => ({
    width: getWidth(props.paneWidth),
    display: 'flex',
    alignItems: 'center',
    margin: '5px',
    height: '90px',
  }));

  // Style for the icons
  const iconStyle = {
    marginBottom: '10px',
    height: '50px',
    width: '50px',
    border: 'none',
  };

  // Style for the menu button on the top right
  const menuStyle = {
    display: 'flex',
    flexDirection: 'column',
    margin: '10px',
  };
</script>

<template>
  <div class="paneStyle">
    <a-button :style="returnStyle" ghost @click="placeHolder">
      <template #icon
        ><RightCircleFilled
          style="color: black; font-size: 50px; border-radius: 50%"
        />
      </template>
    </a-button>

    <div class="mainStyle">
      <!-- create box for the project name -->
      <a-card :style="nameBoxStyle">
        <h1
          v-if="!isEditing"
          class="projectNameH1"
          style="font-size: 2.8em; font-weight: bold"
        >
          {{ projectData.projectName }}
        </h1>
        <input
          v-else
          v-model="projectData.projectName"
          class="projectNameInput"
          type="input"
        />
        <!-- pencil icon for editing the project name -->
        <a-button
          class="edit-button"
          :style="editIconStyle"
          ghost
          @click="placeHolder"
        >
          <template #icon
            ><EditOutlined style="color: black; font-size: 35px" />
          </template>
        </a-button>
      </a-card>

      <!-- create box for project description (BU, Team Nr, Department, Client Name) -->
      <a-row :style="descboxStyle">
        <a-card :style="profileFieldStyle">
          <label for="businessUnit" style="font-size: 1.2em; font-weight: bold"
            >Business Unit:</label
          >
          <p style="font-size: 1.6em; margin: 0">
            {{ projectData.businessUnit }}
          </p>
        </a-card>

        <a-card :style="profileFieldStyle">
          <label for="teamNumber" style="font-size: 1.2em; font-weight: bold"
            >Team Number:</label
          >
          <p style="font-size: 1.6em; margin: 0">
            {{ projectData.teamNumber }}
          </p>
        </a-card>

        <a-card :style="profileFieldStyle">
          <label for="department" style="font-size: 1.2em; font-weight: bold"
            >Department:</label
          >
          <p style="font-size: 1.6em; margin: 0">
            {{ projectData.department }}
          </p>
        </a-card>

        <a-card :style="profileFieldStyle">
          <label for="clientName" style="font-size: 1.2em; font-weight: bold"
            >Client Name:</label
          >
          <p style="font-size: 1.6em; margin: 0">
            {{ projectData.clientName }}
          </p>
        </a-card>
      </a-row>
    </div>

    <!-- add icons for profile, plugins, global logs, signout -->
    <a-col :style="menuStyle">
      <a-button :style="iconStyle" ghost @click="placeHolder">
        <template #icon
          ><UserOutlined style="color: black; font-size: 40px" />
        </template>
      </a-button>
      <a-button :style="iconStyle" ghost @click="placeHolder">
        <template #icon
          ><AppstoreAddOutlined style="color: black; font-size: 40px" />
        </template>
      </a-button>
      <a-button :style="iconStyle" ghost @click="placeHolder">
        <template #icon
          ><BarsOutlined style="color: black; font-size: 40px" />
        </template>
      </a-button>
      <a-button :style="iconStyle" ghost @click="placeHolder">
        <template #icon
          ><LogoutOutlined style="color: black; font-size: 40px" />
        </template>
      </a-button>
    </a-col>
  </div>
</template>

<script lang="ts">
  // Flag for editable Title
  const isEditing = ref(false);
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

<style>
  /* Style for the middle section */
  .mainStyle {
    width: 60vw;
    height: 80vh;
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
</style>
