<script lang="ts" setup>
  import { computed, inject, onMounted, ref, toRaw } from 'vue';
  import {
    localLogStoreSymbol,
    projectEditStoreSymbol,
    projectRoutingSymbol,
  } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';
  import type {
    DetailedProjectModel,
    UpdateProjectModel,
  } from '@/models/Project';
  import type { ComputedRef } from 'vue';
  import {
    DeleteOutlined,
    EditOutlined,
    InboxOutlined,
    UndoOutlined,
  } from '@ant-design/icons-vue';
  import { useEditing } from '@/utils/hooks/useEditing';
  import { usePluginStore, useProjectStore } from '@/store';
  import type { EditProjectModel } from '@/models/Project/EditProjectModel';
  import ConfirmAction from '@/components/Modal/ConfirmAction.vue';
  import IconButton from '@/components/Button/IconButton.vue';
  import router from '@/router';
  import _ from 'lodash';
  import {
    EditableTextField,
    ProjectInformationInputField,
    ProjectInformationSelectField,
  } from '@/components/EditableTextField';
  import { useThemeToken } from '@/utils/hooks';
  import { CompanyState, SecurityLevel } from '@/api/generated';

  const localLogStore = inject(localLogStoreSymbol);
  const projectStore = useProjectStore();
  const projectEditStore = inject(projectEditStoreSymbol)!;
  const pluginStore = usePluginStore();
  const projectRouting = inject(projectRoutingSymbol)!;
  const token = useThemeToken();

  const editingClass = computed(() => ({
    'editing-mode': isEditing.value,
  }));

  const nonEditingClass = computed(() => ({
    'non-editing-mode': !isEditing.value,
  }));

  const { getIsLoadingProject } = storeToRefs(projectStore);
  const { getIsLoading } = storeToRefs(projectStore);
  const isLoading = computed(
    () => getIsLoadingProject.value || getIsLoading.value,
  );

  const { isEditing, stopEditing, startEditing } = useEditing();

  onMounted(async () => {
    const project = projectStore.getProject;
    if (project) addData(project);

    const data: ComputedRef<DetailedProjectModel | null> = computed(
      () => projectStore.getProject,
    );

    watch(
      () => data.value,
      (newProject, oldProject) => {
        if (!newProject) return;
        if (!_.isEqual(newProject, oldProject)) {
          addData(toRaw(newProject));
        }
      },
    );
  });

  // set watcher for isEditing. if isEditing is false reset the input status fields
  watch(
    () => isEditing.value,
    (newVal) => {
      if (!newVal) {
        projectEditStore.resetPluginChanges();
        BUInputStatus.value = '';
        teamNumberInputStatus.value = '';
        departmentInputStatus.value = '';
        clientNameInputStatus.value = '';
        offerIdInputStatus.value = '';
        companyInputStatus.value = '';
        companyStateInputState.value = '';
        ismsLevelInputState.value = '';
        addData(projectStore.getProject!);
      }
    },
  );

  const toggleEditingMode = async () => {
    if (isEditing.value) {
      await stopEditing();
    } else {
      await startEditing();
    }
  };

  const projectData = {
    id: ref<number>(0),
    slug: ref<string>(''),
    projectName: ref<string>(''),
    businessUnit: ref<string>(''),
    teamNumber: ref<number>(0),
    department: ref<string>(''),
    clientName: ref<string>(''),
    offerId: ref<string>(''),
    company: ref<DetailedProjectModel['company']>(''),
    companyState: ref<DetailedProjectModel['companyState']>('EXTERNAL'), //check if implementation matches with backend
    ismsLevel: ref<DetailedProjectModel['ismsLevel']>('NORMAL'),
    isArchived: ref<boolean>(false),
  };

  type Status = '' | 'error' | 'warning' | undefined;

  const BUInputStatus = ref<Status>('');
  const teamNumberInputStatus = ref<Status>('');
  const departmentInputStatus = ref<Status>('');
  const clientNameInputStatus = ref<Status>('');
  const offerIdInputStatus = ref<Status>('');
  const companyInputStatus = ref<Status>('');
  const companyStateInputState = ref<Status>('');
  const ismsLevelInputState = ref<Status>('');

  const BUInput = ref(projectData.businessUnit);
  const teamNumberInput = ref(projectData.teamNumber);
  const departmentInput = ref(projectData.department);
  const clientNameInput = ref(projectData.clientName);
  const offerIdInput = ref(projectData.offerId);
  const companyInput = ref(projectData.company);
  const companyStateInput = ref(projectData.companyState);
  const ismsLevelInput = ref(projectData.ismsLevel);

  type BaseInputField<T = string | number> = {
    label: string;
    name: string;
    value: Ref<T>;
    status: Ref<Status>;
    displayValue?: (value: T) => string | number | boolean | undefined;
  };

  type InputField<T = string | number> = BaseInputField<T> &
    (
      | {
          options?: string[] | (keyof T)[];
          getValue?: (value: string) => T;
          inputType: 'select';
        }
      | {
          inputType?: 'text';
        }
    );

  const textFields = ref<InputField[]>([
    {
      label: 'Business\xa0Unit',
      name: 'businessUnit',
      value: BUInput,
      status: BUInputStatus,
    },
    {
      label: 'Team\xa0Number',
      name: 'teamNumber',
      value: teamNumberInput,
      status: teamNumberInputStatus,
    },
    {
      label: 'Department',
      name: 'department',
      value: departmentInput,
      status: departmentInputStatus,
    },
    {
      label: 'Client\xa0Name',
      name: 'clientName',
      value: clientNameInput,
      status: clientNameInputStatus,
    },
    {
      label: 'Offer\xa0ID',
      name: 'offerId',
      value: offerIdInput,
      status: offerIdInputStatus,
    },
    {
      label: 'Company',
      name: 'company',
      value: companyInput,
      status: companyInputStatus,
    },
    {
      label: 'Company\xa0State',
      name: 'companyState',
      value: companyStateInput,
      status: companyStateInputState,
      options: Object.keys(CompanyState) as (keyof typeof CompanyState)[],
      displayValue: (value) =>
        Object.keys(CompanyState).find(
          (key) => CompanyState[key as keyof typeof CompanyState] === value,
        ),
      getValue: (value) => CompanyState[value as keyof typeof CompanyState],
      inputType: 'select',
    },
    {
      label: 'ISMS\xa0Level',
      name: 'ismsLevel',
      value: ismsLevelInput,
      status: ismsLevelInputState,
      options: Object.keys(SecurityLevel) as (keyof typeof SecurityLevel)[],
      displayValue: (value) =>
        Object.keys(SecurityLevel).find(
          (key) => SecurityLevel[key as keyof typeof SecurityLevel] === value,
        ),
      getValue: (value) => SecurityLevel[value as keyof typeof SecurityLevel],
      inputType: 'select',
    },
  ]);

  //Function to update the project information
  function updateProjectInformation(): void {
    const updatedProject: EditProjectModel = {
      projectName: projectData.projectName.value,
      businessUnit: BUInput.value,
      teamNumber: teamNumberInput.value,
      department: departmentInput.value,
      clientName: clientNameInput.value,
      offerId: offerIdInput.value,
      company: companyInput.value,
      companyState: companyStateInput.value,
      ismsLevel: ismsLevelInput.value,
    };
    projectEditStore.updateProjectInformationChanges(updatedProject);
  }

  //Function to load the data from projectViewService to projectView
  function addData(loadedData: DetailedProjectModel) {
    if (projectStore.getProject)
      projectEditStore.setProjectInformation(projectStore.getProject);
    projectData.id.value = loadedData.id;
    projectData.slug.value = loadedData.slug;
    projectData.projectName.value = loadedData.projectName;
    projectData.businessUnit.value = loadedData.businessUnit;
    projectData.teamNumber.value = loadedData.teamNumber;
    projectData.department.value = loadedData.department;
    projectData.clientName.value = loadedData.clientName;
    projectData.offerId.value = loadedData.offerId;
    projectData.company.value = loadedData.company;
    projectData.companyState.value = loadedData.companyState;
    projectData.ismsLevel.value = loadedData.ismsLevel;
  }

  const isArchiveModalOpen = ref(false);
  const isDeleteModalOpen = ref(false);
  const isModalOpen = ref(false);

  const handleArchive = () => {
    isArchiveModalOpen.value = true;
  };

  const handleDelete = async () => {
    isDeleteModalOpen.value = true;
  };

  const confirmDelete = async () => {
    const project = projectStore.getProject;

    if (!project?.id) {
      isDeleteModalOpen.value = false;
      return;
    }

    try {
      await projectStore.delete(project.id);
    } catch (error) {
      console.error('Error deleting project:', error);
    } finally {
      isDeleteModalOpen.value = false;
      const newProjectId =
        getNextActiveProjectId(project.id) === project.id
          ? getNextArchivedProjectId()!
          : getNextActiveProjectId(project.id);
      if (newProjectId === undefined) {
        await router.push('/');
      } else {
        projectRouting.setProjectId(newProjectId);
      }
    }
    return;
  };

  const getNextArchivedProjectId = (): number | undefined => {
    const projects = projectStore.getProjects;
    const nextProject = projects.find((project) => project.isArchived);
    if (!nextProject) return undefined;
    return nextProject.id;
  };

  const getNextActiveProjectId = (currentProjectId: number): number => {
    const projects = projectStore.getProjects;
    const nextProject = projects.find((project) => !project.isArchived);
    if (!nextProject) return currentProjectId;
    return nextProject.id;
  };

  const confirmArchive = async () => {
    const projectID = projectStore?.getProject?.id;
    const projectData = projectStore?.getProject as UpdateProjectModel;
    projectData.pluginList = pluginStore?.getPlugins;

    if (projectID) {
      try {
        await projectStore.archive(projectID);
      } finally {
        isArchiveModalOpen.value = false;
        isModalOpen.value = false;
        await localLogStore?.fetch(projectID);
        const newProjectId = getNextActiveProjectId(projectID);
        projectRouting.setProjectId(newProjectId);
      }
    }
  };

  const reactivateProject = async () => {
    const currentProject = projectStore.getProject! as UpdateProjectModel;
    const projectId = projectStore.getProject?.id;
    currentProject.pluginList = pluginStore.getPlugins;

    await projectStore.unarchive(projectId!);
    await localLogStore?.fetch(projectId!);
  };
</script>

<template>
  <div class="pane">
    <div v-if="projectData.id.value" class="main">
      <!-- create box for the project name -->
      <div class="projectNameContainer">
        <h1 v-if="!isLoading" class="projectName">
          {{ projectData.projectName.value }}
        </h1>
        <a-skeleton v-else active :paragraph="false" style="max-width: 20em" />

        <!-- Edit Button -->
        <IconButton
          v-if="!projectStore.getProject?.isArchived && !isEditing"
          tooltip-position="left"
          tooltip="Click here to activate Edit-View"
          @click="toggleEditingMode"
        >
          <template #icon>
            <EditOutlined class="icon" />
          </template>
        </IconButton>

        <!-- Reactivate Button -->
        <IconButton
          v-if="projectStore.getProject?.isArchived"
          tooltip-position="left"
          tooltip="Click here to reactivate"
          @click="reactivateProject"
        >
          <template #icon>
            <UndoOutlined class="icon" />
          </template>
        </IconButton>

        <!-- Delete Button -->
        <IconButton
          v-if="projectStore.getProject?.isArchived"
          tooltip-position="right"
          tooltip="Click here to delete the project"
          @click="handleDelete"
        >
          <template #icon>
            <DeleteOutlined class="icon" />
          </template>
        </IconButton>

        <ConfirmAction
          :is-open="isDeleteModalOpen"
          title="Delete Project"
          message="Are you sure you want to delete this project permanently?"
          @confirm="confirmDelete"
          @cancel="isDeleteModalOpen = false"
          @update:is-open="(value) => (isDeleteModalOpen = value)"
        />

        <!-- Archive Button -->
        <IconButton
          v-if="!projectStore.getProject?.isArchived && !isEditing"
          tooltip-position="right"
          tooltip="Click here to archive the project"
          @click="handleArchive"
        >
          <template #icon>
            <InboxOutlined class="icon" />
          </template>
        </IconButton>

        <ConfirmAction
          :is-open="isArchiveModalOpen"
          title="Archive Project"
          message="Are you sure you want to archive this project?"
          @confirm="confirmArchive"
          @cancel="isArchiveModalOpen = false"
          @update:is-open="(value) => (isArchiveModalOpen = value)"
        />
      </div>

      <!-- create box for project description (BU, Team Nr, Department, Client Name) -->
      <a-flex
        class="projectInformationBox"
        :body-style="{
          height: 'fit-content',
        }"
      >
        <EditableTextField
          v-if="!isEditing"
          class="infoCard"
          :value="projectData.slug.value"
          :is-loading="isLoading"
          :label="'Project\xa0Slug'"
          :has-edit-keys="false"
        />

        <EditableTextField
          v-for="field in textFields"
          :key="field.name"
          class="infoCard"
          :class="[editingClass, nonEditingClass]"
          :value="field.value"
          :is-loading="isLoading"
          :label="field.label"
          :has-edit-keys="false"
          :display-value="field.displayValue"
        >
          <ProjectInformationSelectField
            v-if="field.inputType === 'select'"
            :column-name="field.name"
            :input-value="field.value"
            :input-status="field.status"
            :edit-store="projectEditStore"
            :options="field.options!"
            :get-value="field.getValue!"
            :display-value="field.displayValue!"
            @updated="
              (newValue) => {
                field.value = newValue;
                updateProjectInformation();
              }
            "
            @error="field.status = 'error'"
            @success="field.status = ''"
          />
          <ProjectInformationInputField
            v-else
            :column-name="field.name"
            :input-value="field.value"
            :input-status="field.status"
            :edit-store="projectEditStore"
            @updated="
              (newValue) => {
                field.value = newValue;
                updateProjectInformation();
              }
            "
            @error="field.status = 'error'"
            @success="field.status = ''"
          />
        </EditableTextField>
      </a-flex>
    </div>
    <a-flex v-else justify="center" align="center" class="emptyProjects">
      <a-empty description="No project selected." />
    </a-flex>
  </div>
</template>

<style scoped lang="scss">
  /* Style for the middle section */
  .main {
    width: 100%;
    max-height: 80vh;
    height: max-content;
    padding: 0 3.5em;
    display: flex;
    flex-direction: column;
    align-items: center;
  }

  .emptyProjects {
    height: 100vh;
    width: 100vh;
    color: v-bind('token.colorText');
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
    color: v-bind('token.colorText');
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
    container-type: inline-size;
    background-color: v-bind('token.colorBgElevated') !important;
  }

  @container (max-width: 53vw) {
    .infoCard.editing-mode {
      width: 100% !important;
    }
  }

  @container (max-width: 45vw) {
    .infoCard.non-editing-mode {
      width: 100% !important;
    }
  }

  .infoCard {
    border: none;
    width: 50%;
    display: table;
    padding: 0 1em 0 1em;
    max-width: 100%;
    background-color: v-bind('token.colorBgElevated ');
  }

  .button {
    margin-bottom: 10px;
    height: 40px;
    width: 40px;
    border: none;
  }

  .icon {
    color: v-bind('token.colorText');
    font-size: 1.5em;
  }

  .label {
    font-size: 1.4em;
    font-weight: bold;
    margin: 0;
  }

  .text {
    font-size: 1.4em;
    margin: 0 auto 0 0.5em;
    white-space: nowrap;
  }
</style>
