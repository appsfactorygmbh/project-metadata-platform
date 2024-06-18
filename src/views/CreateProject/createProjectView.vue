<script lang="ts" setup>
  import { ref, inject, computed } from 'vue';
  import { PlusOutlined } from '@ant-design/icons-vue';
  import {
    FontColorsOutlined,
    ShoppingOutlined,
    TeamOutlined,
    BankOutlined,
    UserOutlined,
  } from '@ant-design/icons-vue';
  import { projectsService } from '@/services/ProjectService.ts';
  import { TableStore } from '@/store/TableStore.ts';
  import { reactive } from 'vue';
  import type { UnwrapRef } from 'vue';

  const open = ref<boolean>(false);
  const formRef = ref();
  const labelCol = { style: { width: '150px' } };
  const wrapperCol = { span: 14 };
  import type { InputStateType } from '@/models/InputStateModel.ts';
  import { projectsStoreSymbol } from '@/store/injectionSymbols';

  // TableStore to refetch Table after Project was added
  const projectsStore = inject(projectsStoreSymbol)

  const open = ref<boolean>(false);

  const isAdding = computed (() => projectsStore?.getIsAdding)

  // Required values for creating a project
  const projectName = ref<string>('');
  const businessUnit = ref<string>('');
  const teamNumber = ref<string>('');
  const department = ref<string>('');
  const clientName = ref<string>('');

  const projectNameStatus = ref<InputStateType>('');
  const businessUnitStatus = ref<InputStateType>('');
  const teamNumberStatus = ref<InputStateType>('');
  const departmentStatus = ref<InputStateType>('');
  const clientNameStatus = ref<InputStateType>('');

  const fetchError = ref<boolean>(false);

  interface FormState {
    projectName: string;
    businessUnit: string;
    teamNumber: number | null;
    department: string;
    clientName: string;
  }
  const formState: UnwrapRef<FormState> = reactive({
    projectName: '',
    businessUnit: '',
    teamNumber: null,
    department: '',
    clientName: '',
  });
  const validateMessages = {
    required: 'Please input the field.',
    types: {
      number: 'Team number is not a valid number!',
    },
    number: {
      range: 'Team number must be positive number.',
    },
  };

  // opens modal when plussign is clicked
  const showModal = () => {
    open.value = true;
  };

  const reset = () => {
    formRef.value.resetFields();
  };

  // checks for correct input
  const handleOk = () => {
    formRef.value
      .validate()
      .then(() => {
        submit();
      })
      .catch((error) => {
        console.log('error', error);
      });
  };

  // sends PUT request to the backend
  const submit = async () => {
    const projectData = {
      projectName: formState.projectName,
      businessUnit: formState.businessUnit,
      teamNumber: formState.teamNumber,
      department: formState.department,
      clientName: formState.clientName,
    };

    const response = await projectsService.addProject(projectData);
    console.log(response);
    if (!response?.ok || undefined) {
      fetchError.value = true;
      open.value = true;
    } else {
      fetchError.value = false;
      await tableStore.fetchTable();
      reset();
      open.value = false;
  // checks for correct input and does PUT request to the backend
  const handleOk = async () => {
    validateField(projectName.value, projectNameStatus);
    validateField(businessUnit.value, businessUnitStatus);
    validateField(department.value, departmentStatus);
    validateField(clientName.value, clientNameStatus);

    const validTeamNumber = validateTeamNumber(teamNumber.value);

    if (
      projectName.value &&
      businessUnit.value &&
      teamNumber.value &&
      department.value &&
      clientName.value &&
      validTeamNumber
    ) {
      const projectData = {
        projectName: projectName.value,
        businessUnit: businessUnit.value,
        teamNumber: parseInt(teamNumber.value),
        department: department.value,
        clientName: clientName.value,
      };
      await projectsStore?.addProjects(projectData)
      if (!projectsStore?.getAddedSuccessfully) {
        fetchError.value = true;
        open.value = true;
      } else {
        setTimeout(() => {
          fetchError.value = true;
          stopWatch();
        }, 1000);
        const stopWatch = watch(() => isAdding.value, async (added) => {
          if (added === false) {
            await projectsStore?.fetchProjects();
            resetAndCloseModal();
          }
        });
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
      width="500px"
      title="Create Project"
      @ok="handleOk"
      :ok-button-props="{ disabled: isAdding}"
    >
      <a-form
        ref="formRef"
        :model="formState"
        :validate-messages="validateMessages"
        :label-col="labelCol"
        :wrapper-col="wrapperCol"
      >
        <a-form-item name="projectName" :rules="[{ required: true }]">
          <a-input
            v-model:value="formState.projectName"
            class="inputField"
            placeholder="Project Name"
          >
            <template #prefix>
              <FontColorsOutlined />
            </template>
          </a-input>
        </a-form-item>
        <a-form-item name="businessUnit" :rules="[{ required: true }]">
          <a-input
            v-model:value="formState.businessUnit"
            class="inputField"
            placeholder="Business Unit"
          >
            <template #prefix>
              <ShoppingOutlined />
            </template>
          </a-input>
        </a-form-item>
        <a-form-item
          name="teamNumber"
          :rules="[{ required: true }, { type: 'number', min: 0 }]"
        >
          <a-input-number
            v-model:value="formState.teamNumber"
            class="inputField"
            placeholder="Team Number"
          >
            <template #prefix>
              <TeamOutlined />
            </template>
          </a-input-number>
        </a-form-item>
        <a-form-item name="department" :rules="[{ required: true }]">
          <a-input
            v-model:value="formState.department"
            class="inputField"
            placeholder="Department"
          >
            <template #prefix>
              <BankOutlined />
            </template>
          </a-input>
        </a-form-item>
        <a-form-item name="clientName" :rules="[{ required: true }]">
          <a-input
            v-model:value="formState.clientName"
            class="inputField"
            placeholder="Client Name"
          >
            <template #prefix>
              <UserOutlined />
            </template>
          </a-input>
        </a-form-item>
        <!--shows error if the PUT request failed-->
        <a-alert
          v-if="fetchError"
          message="Failed to create Project"
          type="error"
          show-icon
        ></a-alert>
      </a-form>
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
