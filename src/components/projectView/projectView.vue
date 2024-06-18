<script setup lang="ts">
  import { onMounted, computed, ref, watch } from 'vue';
  import type { Project } from '@/models/ProjectViewModel';
  import { projectStore } from '@/store/ProjectViewStore';

  const store = projectStore();

  //Get the width of the right pane from App.vue
  const props = defineProps({
    paneWidth: {
      type: Number,
      required: true,
    },
  });

  watch(
    () => props.paneWidth,
    () => {
      getWidth(props.paneWidth);
    },
  );

  // Fetch data when component is mounted
  onMounted(async () => {
    const loadProject: Project = await store.getProjectView();
    addData(loadProject);
  });

  // Style for the return button
  const returnStyle = {
    cursor: 'pointer',
    height: '60px',
    margin: '20px',
  };

  // Style for the middle section
  const mainStyle = {
    width: '60vw',
    height: '80vh',
    padding: '50px',
    margin: '10px',

    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
  } as const;

  // Style for the Project title box
  const nameBoxStyle = {
    width: '85%',
    maxWidth: '750px',
    minWidth: '250px',
    padding: '10px',
    margin: '10px',
    borderRadius: '10px',

    background: 'white',
    boxShadow: '0 2px 4px rgba(0, 0, 0, 0.5)',
    textAlign: 'center',
  };

  // Style for the Project name input box
  const projectNameInputStyle = {
    fontSize: '2.8em',
    width: '80%',
    height: '2.8em',
    textAlign: 'center',
    border: 'none',
    borderBottom: '2px solid #a5a4a4',
    color: 'black',
    backgroundColor: 'rgb(250, 250, 250)',
  } as const;

  // Style for the pencil button
  const editIconStyle = {
    cursor: 'pointer',
    position: 'absolute',
    right: '3%',
    height: '50px',
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
    boxShadow: '0 2px 4px rgba(0, 0, 0, 0.5)',
  };

  // Style for the description field box
  const projectInputStyle = {
    fontSize: '1.6em',
    width: '90%',
    height: '1.6em',
    textAlign: 'center',
    border: 'none',
    backgroundColor: 'white',
    cursor: 'default',
  };

  // Sizing for the inside box in the project description box
  const profileFieldStyle = computed(() => ({
    width: getWidth(props.paneWidth),
    display: 'flex',
    alignItems: 'center',
    margin: '5px',
    height: '90px',
  }));

  //Style for the right panel
  const paneStyle = {
    display: 'flex',
    flexDirection: 'row',
  } as const;

  // Style for the icons
  const iconStyle = {
    marginBottom: '10px',
    height: '60px',
  };

  // Style for the menu button on the top right
  const menuStyle = {
    display: 'flex',
    flexDirection: 'column',
    margin: '10px',
  };
</script>

<template>
  <div :style="paneStyle">
    <a-button :style="returnStyle" ghost @click="placeHolder">
      <!-- add return icon -->
      <img src="/icons/return.png" alt="Return" />
    </a-button>

    <div :style="mainStyle">
      <!-- create box for the project name -->
      <a-card :style="nameBoxStyle">
        <h1
          v-if="!isEditing"
          class="projectNameH1"
          style="font-size: 2.8em; font-weight: bold"
        >
          {{ projectName }}
        </h1>
        <input
          v-else
          v-model="projectName"
          class="projectNameInput"
          type="input"
          :style="projectNameInputStyle"
        />
        <!-- pencil icon for editing the project name -->
        <a-button
          class="edit-button"
          :style="editIconStyle"
          ghost
          @click="toggleEditing"
        >
          <img src="/icons/edit.png" alt="Edit" />
        </a-button>
      </a-card>

      <!-- create box for project description (BU, Team Nr, Department, Client Name) -->
      <a-row :style="descboxStyle">
        <a-card :style="profileFieldStyle">
          <label for="businessUnit" style="font-size: 1.2em; font-weight: bold"
            >Business Unit:</label
          >
          <a-input
            type="text"
            :value="data.businessUnit"
            :style="projectInputStyle"
            readonly
          />
        </a-card>

        <a-card :style="profileFieldStyle">
          <label for="teamNumber" style="font-size: 1.2em; font-weight: bold"
            >Team Number:</label
          >
          <a-input
            type="text"
            :value="data.teamNumber"
            :style="projectInputStyle"
            readonly
          />
        </a-card>

        <a-card :style="profileFieldStyle">
          <label for="department" style="font-size: 1.2em; font-weight: bold"
            >Department:</label
          >
          <a-input
            type="text"
            :value="data.department"
            :style="projectInputStyle"
            readonly
          />
        </a-card>

        <a-card :style="profileFieldStyle">
          <label for="clientName" style="font-size: 1.2em; font-weight: bold"
            >Client Name:</label
          >
          <a-input
            type="text"
            :value="data.clientName"
            :style="projectInputStyle"
            readonly
          />
        </a-card>
      </a-row>
    </div>

    <!-- add icons for profile, plugins, global logs, signout -->
    <a-col :style="menuStyle">
      <a-button :style="iconStyle" ghost @click="placeHolder">
        <img src="/icons/profile.png" alt="Profile" />
      </a-button>
      <a-button :style="iconStyle" ghost @click="placeHolder">
        <img src="/icons/plugin.png" alt="Plugins" />
      </a-button>
      <a-button :style="iconStyle" ghost @click="placeHolder">
        <img src="/icons/logs.png" alt="Global Logs" />
      </a-button>
      <a-button :style="iconStyle" ghost @click="placeHolder">
        <img src="/icons/logout.png" alt="Sign Out" />
      </a-button>
    </a-col>
  </div>
</template>

<script lang="ts">
  // Flag for editable Title
  const isEditing = ref(false);
  let data = {
    projectName: '',
    businessUnit: '',
    teamNumber: '',
    department: '',
    clientName: '',
  };
  let projectName = ref('');

  //Function to save and edit the project name
  const toggleEditing = () => {
    isEditing.value = !isEditing.value;
  };

  // Place holder for the buttons for now
  const placeHolder = () => {
    console.log('Icon clicked');
  };

  //Function to load the data from projectViewService to projectView
  function addData(loadedData: Project) {
    data = loadedData;
    projectName.value = data.projectName;
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
