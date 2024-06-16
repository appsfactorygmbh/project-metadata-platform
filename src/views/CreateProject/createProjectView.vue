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

  const open = ref<boolean>(false);

  const projectName = ref<string>('');
  const businessUnit = ref<string>('');
  const teamNumber = ref<string>('');
  const department = ref<string>('');
  const clientName = ref<string>('');

  const projectNameStatus = ref<string>('');
  const businessUnitStatus = ref<string>('');
  const teamNumberStatus = ref<string>('');
  const departmentStatus = ref<string>('');
  const clientNameStatus = ref<string>('');

  const showModal = () => {
    open.value = true;
  };

  const resetAndCloseModal = () => {
    // Reset the input fields
    projectName.value = '';
    businessUnit.value = '';
    teamNumber.value = '';
    department.value = '';
    clientName.value = '';

    // Reset the status fields
    projectNameStatus.value = '';
    businessUnitStatus.value = '';
    teamNumberStatus.value = '';
    departmentStatus.value = '';
    clientNameStatus.value = '';

    // Close the modal
    open.value = false;
  };

  const validateField = (
    fieldValue: string,
    fieldStatus: { value: string },
  ) => {
    if (!fieldValue) {
      fieldStatus.value = 'error';
    } else {
      fieldStatus.value = '';
    }
  };

  const handleOk = () => {
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
      projectsService.addProject(projectData);

      resetAndCloseModal()
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
      width="500px"
      title="Create Project"
      @ok="handleOk"
    >
      <a-space direction="vertical" class="space">
        <a-input
          v-model:value="projectName"
          class="inputField"
          :status="projectNameStatus"
          placeholder="Project Name"
        >
          <template #prefix>
            <FontColorsOutlined />
          </template>
        </a-input>
        <a-input
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
          v-model:value="clientName"
          class="inputField"
          :status="clientNameStatus"
          placeholder="Client Name"
        >
          <template #prefix>
            <UserOutlined />
          </template>
        </a-input>
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

  .inputField {
    width: 90%;
  }
</style>
