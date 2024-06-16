<script lang="ts" setup>
  import { ref } from 'vue';
  import { PlusOutlined } from '@ant-design/icons-vue';
  import {
    FontColorsOutlined,
    ShoppingOutlined,
    TeamOutlined,
    BankOutlined,
    UserOutlined,
  } from '@ant-design/icons-vue';
  import { projectsService } from '../../services/ProjectService.ts';
  import {InputState} from '../../models/InputStateModel.ts'
  import {TableStore} from '../../store/TableStore.ts'

  const tableStore = TableStore()

  const open = ref<boolean>(false);

  const projectName = ref<string>('');
  const businessUnit = ref<string>('');
  const teamNumber = ref<string>('');
  const department = ref<string>('');
  const clientName = ref<string>('');

  const projectNameStatus = ref<InputState>('');
  const businessUnitStatus = ref<InputState>('');
  const teamNumberStatus = ref<InputState>('');
  const departmentStatus = ref<InputState>('');
  const clientNameStatus = ref<InputState>('');

  const fetchError = ref<boolean>(false);

  const showModal = () => {
    open.value = true;
  };

  const validateField = (
    fieldValue: string,
    fieldStatus: { value: InputState },
  ) => {
    if (!fieldValue) {
      fieldStatus.value = 'error';
    } else {
      fieldStatus.value = '';
    }
  };

  const handleOk = async () => {
    validateField(projectName.value, projectNameStatus);
    validateField(businessUnit.value, businessUnitStatus);
    validateField(teamNumber.value, teamNumberStatus);
    validateField(department.value, departmentStatus);
    validateField(clientName.value, clientNameStatus);

    if (
      projectName.value &&
      businessUnit.value &&
      teamNumber.value &&
      department.value &&
      clientName.value
    ) {
      const projectData = {
        projectName: projectName.value,
        businessUnit: businessUnit.value,
        teamNumber: teamNumber.value,
        department: department.value,
        clientName: clientName.value,
      };
      const response = await projectsService.addProject(projectData);
      console.log(response);
      if (!response?.ok) {
        fetchError.value = true;
      } else {
        fetchError.value = false;
        open.value = false;
        await tableStore.fetchTable();
      }
    }
  };
</script>

<template>
  <div>
    <a-float-button @click="showModal">
      <template #icon>
        <PlusOutlined />
      </template>
    </a-float-button>
    <a-modal
      v-model:open="open"
      width="400px"
      title="Create Project"
      @ok="handleOk"
    >
      <a-space direction="vertical" class="space">
        <a-input
          id="projectNameField"
          v-model:value="projectName"
          class="inputField"
          :status = "projectNameStatus"
          placeholder="Project Name"
        >
          <template #prefix>
            <FontColorsOutlined />
          </template>
        </a-input>
        <a-input
          id="businessUnitField"
          v-model:value="businessUnit"
          class="inputField"
          :status="businessUnitStatus"
          placeholder="Business Unit"
        >
          <template #prefix>
            <ShoppingOutlined />
          </template>
        </a-input>
        <a-input
          id="teamNumberField"
          v-model:value="teamNumber"
          class="inputField"
          :status="teamNumberStatus"
          placeholder="Team Number"
        >
          <template #prefix>
            <TeamOutlined />
          </template>
        </a-input>
        <a-input
          id="departmentField"
          v-model:value="department"
          class="inputField"
          :status="departmentStatus"
          placeholder="Department"
        >
          <template #prefix>
            <BankOutlined />
          </template>
        </a-input>
        <a-input
          id="clientNameField"
          v-model:value="clientName"
          class="inputField"
          :status="clientNameStatus"
          placeholder="Client Name"
        >
          <template #prefix>
            <UserOutlined />
          </template>
        </a-input>
        <a-alert
          v-if="fetchError"
          message="Failed to create Project"
          type="error"
          show-icon
        ></a-alert>
      </a-space>
    </a-modal>
  </div>
</template>

<style scoped lang="scss">
  .space {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    & > * {
      width: 100%;
    }
  }
</style>