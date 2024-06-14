<script setup lang="ts">
import { onMounted, ref , Ref } from 'vue'

// Flag for editable Title
const isEditing: Ref<boolean> = ref(false);

const projectName: Ref<string> = ref('Your Project Name');
const businessUnit: Ref<string> = ref('Business Unit');
const teamNr: Ref<string> = ref('Team Number');
const department: Ref<string> = ref('Department');
const clientName: Ref<string> = ref('Client Name');

// Place holder for the buttons for now
const placeHolder = () => {
  console.log('Icon clicked');
};
//Function to save and edit the project name
const toggleEditing = () => {
  
  if (isEditing.value) {
    projectName.value = (document.getElementById('projectNameInput') as HTMLInputElement).value
  }
  isEditing.value = !isEditing.value;
};

//fetch json object from backend 
const reloadData = async () => {
  try {
      /* const response = await fetch(
        import.meta.env.VITE_BACKEND_URL + 
          '/Projects', 
        {
          headers: {
            Accept: 'text/plain',
            'Access-Control-Allow-Origin': '*',
            cors: 'no-cors',
          },
        },
      ); */

    // Fetch test data manually
    const response = await fetch('src/test.json');
    const data = await response.json()
    console.log(data.ProjectName)

    // Update data in Vue component state
    businessUnit.value = data.BusinessUnit
    teamNr.value = data.TeamNumber
    department.value = data.Department
    clientName.value = data.ClientName
    projectName.value = data.ProjectName // Update ref

    console.log('Data reloaded:', data)

  } catch (error) {
    console.error('Error fetching data:', error)
  }
}
// Fetch data when component is mounted
onMounted(reloadData)

// Style for the return button
const returnStyle = {
  cursor: 'pointer',
  height: '60px',
  margin: '10px',
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
  padding: '20px',
  margin: '10px',
  borderRadius: '10px',
  
  background: 'white',
  boxShadow: '0 2px 4px rgba(0, 0, 0, 0.5)',
  textAlign: 'center',
};

// Style for the Project name input box
const projectNameInputStyle = {
  fontSize: '2.5em',
  width: '60%',
  height: '2.5em',
  textAlign: 'center',
  border: 'none',
  borderBottom: '2px solid #a5a4a4',
  color: 'black',
  backgroundColor: 'rgb(250, 250, 250)',
}  as const;

// Style for the pencil button
const editIconStyle = {
  cursor: 'pointer',
  position: 'absolute',
  right: '10px',
  height: '50px',
  top: '50%',
  transform: 'translateY(-50%)',
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
  display: 'flex',

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
} ;

//
const profileFieldStyle = {
  width: '48%',
  margin: '5px',
  height: '100px',
} ;

// Style for the menu button on the top right
const menuStyle = {
  display: 'flex',
  flexDirection: 'column',
  margin: '10px',
  cursor: 'pointer',
};

const paneStyle = {
  display: 'flex',
  flexDirection: 'row',
} as const;

// Style for the icons
const iconStyle = {
  marginBottom: '10px',
  height: '60px',
};
</script>

<template>
  <div :style="paneStyle">
    <a-button :style="returnStyle" ghost @click="placeHolder">
      <!-- add return icon -->
      <img
        src="https://img.icons8.com/?size=50&id=26146&format=png&color=000000"
        alt="Return"
      >
    </a-button>

    <div :style="mainStyle">
      <!-- create box for the project name -->
      <a-card :style="nameBoxStyle">

        <h1 v-if="!isEditing" style="font-size: 2.5em; font-weight: bold;">{{projectName}}</h1>
        <input
          v-if="isEditing"
          id="projectNameInput"
          type="text"
          :value="projectName"
          :style="projectNameInputStyle" 
        />
        <!-- pencil icon for editing the project name -->
        <a-button class="edit-button"  :style="editIconStyle" ghost @click="toggleEditing">
          <img
            src="https://img.icons8.com/ios-glyphs/40/000000/pencil.png"
            alt="Edit"
          />
        </a-button>
      </a-card>

      <!-- create box for project description (BU, Team Nr, Department, Client Name) -->
      <a-row :style="descboxStyle">

        <a-card :style="profileFieldStyle">
          <label for="businessUnit" style="font-size: 1.2em; font-weight: bold;">Business Unit:</label>
          <a-input 
            type="text" 
            :value="businessUnit"
            :style="projectInputStyle" 
            readonly 
          />
        </a-card>

        <a-card :style="profileFieldStyle">
          <label for="teamNumber" style="font-size: 1.2em; font-weight: bold;">Team Number:</label>
          <a-input 
            type="text" 
            :value="teamNr"
            :style="projectInputStyle" 
            readonly 
          />
        </a-card>

        <a-card :style="profileFieldStyle">
          <label for="department" style="font-size: 1.2em; font-weight: bold;">Department:</label>
          <a-input
            type="text" 
            :value="department"
            :style="projectInputStyle" 
            readonly 
          />
        </a-card>

        <a-card :style="profileFieldStyle">
          <label for="clientName" style="font-size: 1.2em; font-weight: bold;">Client Name:</label>
          <a-input 
            type="text" 
            :value="clientName"
            :style="projectInputStyle" 
            readonly 
          />
        </a-card>
      </a-row>
    </div>

    <!-- add icons for profile, plugins, global logs, signout -->
    <a-col :style="menuStyle">
      <a-button :style="iconStyle" ghost @click="placeHolder">
        <img
          src="https://img.icons8.com/?size=50&id=98957&format=png&color=000000"
          alt="Profile"
        />
      </a-button>
      <a-button :style="iconStyle" ghost @click="placeHolder">
        <img
          src="https://img.icons8.com/?size=50&id=61018&format=png&color=000000"
          alt="Plugins"
        />
      </a-button>
      <a-button :style="iconStyle" ghost @click="placeHolder">
        <img
          src="https://img.icons8.com/?size=50&id=60674&format=png&color=000000"
          alt="Global Logs"
        />
      </a-button>
      <a-button :style="iconStyle" ghost @click="placeHolder">
        <img
          src="https://img.icons8.com/?size=50&id=59781&format=png&color=000000"
          alt="Sign Out"
        />
      </a-button>
    </a-col>
  </div>
</template>