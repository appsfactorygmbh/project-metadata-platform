<script setup lang="ts">
//import HelloWorld from './components/HelloWorld.vue'
import { onMounted, ref } from 'vue'
import {Splitpanes, Pane} from 'splitpanes' //external framework that implements the sliders
import 'splitpanes/dist/splitpanes.css'


//editable project name field
const isEditing = ref(false)
const projectName = ref('Project Name')

//initialize empty strings for the project attributes boxes
const businessUnit = ref('')
const teamNr = ref('')
const department = ref('')
const clientName = ref('') 

// Save the new project name after edited
const toggleEditing = () => {
  if (isEditing.value) {
    projectName.value = (document.getElementById('projectNameInput') as HTMLInputElement).value
  }
  isEditing.value = !isEditing.value
}

//fetch json object from backend 
const reloadData = async () => {
  try {
      const response = await fetch(
        import.meta.env.VITE_BACKEND_URL +
          '/Projects', 
        {
          headers: {
            Accept: 'text/plain',
            'Access-Control-Allow-Origin': '*',
            cors: 'no-cors',
          },
        },
      );

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

</script>

<template>
  <div class="container">
    <!-- divide the UI into pane 1 & pane 2-->
    <splitpanes class="default-theme">
      <pane id="pane1" size="25">
        <div class="main"></div>
      </pane>
      
      <pane id="pane2" size="75">
          <div class="return">  <!-- add return icon -->
             <img src="https://img.icons8.com/?size=50&id=26146&format=png&color=000000" alt="Return" class="return-icon">
          </div>

          <div class="main">
            <!-- create box for the project name-->    
            <div class="box">
              <div class="editable-field">
                <h1 v-if="!isEditing" id="projectName">{{ projectName }}</h1>
                <input v-if="isEditing" type="text" id="projectNameInput" :value="projectName" class="project-name-input">
                <img src="https://img.icons8.com/ios-glyphs/40/000000/pencil.png" class="edit-icon" @click="toggleEditing">
              </div>
            </div>
            <!-- create box for project description (BU, Team Nr, Department, Client Name)-->
            <div class="box description-box">
              <div class="profile-field">
                <label for="businessUnit">Business Unit:</label>
                <input type="text" v-model="businessUnit" readonly />
              </div>

              <div class="profile-field">
                <label for="teamNumber">Team Number:</label>
                <input type="text" v-model="teamNr" readonly />
              </div>

              <div class="profile-field">
                <label for="department">Department:</label>
                <input type="text" v-model="department" readonly />
              </div>

              <div class="profile-field">
                <label for="clientName">Client Name:</label>
                <input type="text" v-model="clientName" readonly />
              </div>
            </div>
          </div>

          <!-- add icons for profile, plugins, global logs, signout -->
          <div class="menu">
            <div class="icon">
              <img src="https://img.icons8.com/?size=50&id=98957&format=png&color=000000" alt="Profile" class="profile-icon">
            </div>
            <div class="icon">
              <img src="https://img.icons8.com/?size=50&id=61018&format=png&color=000000" alt="Plugins" class="plugins-icon">
            </div>
            <div class="icon">
              <img src="https://img.icons8.com/?size=50&id=60674&format=png&color=000000" alt="Global Logs" class="logs-icon">
            </div>
            <div class="icon">
              <img src="https://img.icons8.com/?size=50&id=59781&format=png&color=000000" alt="Sign Out" class="out-icon">
            </div>
          
          </div>
      </pane>

    </splitpanes>
  </div>
</template>

<style scoped>
  
    body {
      color: #EBF0F6;
      display: flex;
    }
    /* set default color font for titles*/
    * {
      color: rgb(0, 0, 0);
    }

    #pane1 {
     min-width: 25vw;
    }

    #pane2 {
      padding-right: 30px;
      min-width: 5vw;
      color: #EBF0F6;
      display: flex;
    }

    .container {
      width: 100vw;
      height: 100vh;
      display: flex;
      align-items: center;
      
    }

    input {
      background-color: rgb(255, 255, 255);
      color: rgb(0, 0, 0);
    }
    /* return icon*/
    .return {
      cursor: pointer;
      width: 50px;
      height: 50px;
      margin: 10px;
    }

    .main {
      width: 80vw;
      height: 80vh;
      padding: 50px;
      margin: 10px;
      
      display: flex;
      flex-direction: column;
      align-items: center;
    }

    /* box for the project name headline*/
    .box {
      width: 85%;
      max-width: 750px;
      padding: 30px;
      margin: 10px;
      border-radius: 10px;

      background: white;
      box-shadow: 0 2px 4px rgba(0, 0, 0, 0.5);
      text-align: center;
    }

    /* box for the editable field project name*/
    .box .editable-field {
      display: flex;
      align-items: center;
      justify-content: center;
      position: relative;
    }

    /* project name headline*/
    .box .editable-field h1 {
      font-size: 3em;
      display: inline-block;
    }
    
    /* pencil icon*/
    .box .editable-field .edit-icon {
      cursor: pointer;
      position: absolute;
      right: 10px;
      top: 50%;
      transform: translateY(-50%);
    }
    /* project name font size while being edited*/
    .box .editable-field .project-name-input {
      font-size: 1.5em;
      margin: 20px;
      width: 60%;
      height: 3em;
      text-align: center;
      border: none;
      border-bottom: 2px solid #a5a4a4;
      background-color: rgb(250, 250, 250);
    }

.hidden {
  display: none;
}
    /* project description box*/
    .description-box {
      width: 85%;
      max-width: 750px;
      padding: 30px;
      margin: 10px;

      display: flex;
      flex-wrap: wrap;
      justify-content: space-between;
    }
    /* text box for the descriptions */
    .description-box .profile-field {
      margin-bottom: 10px;
      text-align: left;
      width: 48%;
    }
    /* sub-headline for the project description*/
    .description-box .profile-field label {
      display: block;
      margin: 8px;
      font-weight: Bold;
    }

    /* fetched description of the project*/
    .description-box .profile-field input {
      width: 95%;
      padding: 8px;
      box-sizing: border-box;
      border: 1px solid #ccc;
      border-radius: 5px;
      font-size: 16px;
    }

    /* icons (profile, plugins, global logs, signout) */
    .menu {
      
      display: flex;
      flex-direction: column;
      align-items: left;
      margin: 10px ;
      cursor: pointer;
      width: 50px;
      height: 200px;
      
    }

</style>