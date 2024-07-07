<script lang="ts" setup>
  import { inject, onMounted, toRaw, reactive, onBeforeMount } from 'vue';
  import { projectsStoreSymbol } from '@/store/injectionSymbols';
  import { pluginStoreSymbol } from '@/store/injectionSymbols';
  import PluginView from '@/views/PluginView/PluginView.vue';
  import type { DetailedProjectModel } from '@/models/Project';
  import type { ComputedRef } from 'vue';
  import { EditOutlined } from '@ant-design/icons-vue';
  import ProjectEditButtons from '@/components/ProjectEditButtons/ProjectEditButtons.vue';
  import { useEditing } from '@/utils/hooks/useEditing';
  import type { PluginModel } from '@/models/Plugin';
  import type { DetailedProjectModel } from '@/models/Project';
  import type { UpdateProjectModel } from '@/models/Project';

  const projectsStore = inject(projectsStoreSymbol);

  const isLoading = computed(() => projectsStore?.getIsLoadingProject);

  onBeforeMount(async () => {
    projectsStore?.fetchProject(100);
  });

  const { isEditing, stopEditing } = useEditing();

  const projectStore = inject(projectsStoreSymbol)!;
  const pluginStore = inject(pluginStoreSymbol)!;

  onMounted(async () => {
    const project = projectsStore?.getProject;
    if (project) addData(project);

    const data: ComputedRef<DetailedProjectModel | null | undefined> = computed(
      () => projectsStore?.getProject,
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

  const pluginModel = defineModel<PluginModel[] | null>({
    required: true,
    type: Array,
  });

  const cancelEdit = () => {
    console.log('plugins:       ', pluginModel.value);
    pluginModel.value = pluginStore.getPlugins;
    stopEditing();
  };
  const saveEdit = async () => {
    console.log('plugins:       ', pluginModel.value);
    const updateProjectInformation: DetailedProjectModel | null =
      projectStore.getProject || null;
    const updatedProject: UpdateProjectModel = {
      projectName: updateProjectInformation?.projectName,
      businessUnit: updateProjectInformation?.businessUnit,
      teamNumber: updateProjectInformation?.teamNumber,
      department: updateProjectInformation?.department,
      clientName: updateProjectInformation?.clientName,
      pluginList: pluginModel.value,
    };
    console.log('updated Project', updatedProject);
    const projectID = computed(() => projectStore.getProject?.id);
    if (projectID.value != null) {
      await projectStore.updateProject(updatedProject, projectID.value);
      await pluginStore.fetchPlugins(projectID.value);
    }
    stopEditing();
  };
</script>

<template>
  <ProjectEditButtons v-if="isEditing" @cancel="cancelEdit" @save="saveEdit" />
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
  <PluginView v-model="pluginModel" :project-i-d="100"></PluginView>
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
    margin: 0 0 0 auto;
  }

  .projectInfo {
    font-size: 1.4em;
    margin: 0 auto 0 1em;
    white-space: nowrap;
  }

  .pluginView {
    display: flex;
    justify-content: center;
    padding-top: 1em;
  }
</style>
