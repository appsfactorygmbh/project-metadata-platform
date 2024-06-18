<script lang="ts" setup>
  import { ref, inject, computed, watch } from 'vue';
  import { PlusOutlined } from '@ant-design/icons-vue';
  import {
    FontColorsOutlined,
    ShoppingOutlined,
    TeamOutlined,
    BankOutlined,
    UserOutlined,
  } from '@ant-design/icons-vue';
  import { reactive } from 'vue';
  import type { UnwrapRef } from 'vue';
  import { projectsStoreSymbol } from '@/store/injectionSymbols';

  const open = ref<boolean>(false);
  const formRef = ref();
  const labelCol = { style: { width: '150px' } };
  const wrapperCol = { span: 14 };
  const cancelFetch = ref<boolean>();

  // TableStore to refetch Table after Project was added
  const projectsStore = inject(projectsStoreSymbol);

  const isAdding = computed(() => projectsStore?.getIsAdding);
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

  const resetModal = () => {
    formRef.value.resetFields();
  };

  // checks for correct input
  const handleOk = () => {
    cancelFetch.value = false;
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
    projectsStore?.setIsAdding(true);

    // wait for project creation and checks whether it has been created correctly
    watch(isAdding, (newVal) => {
      if (newVal == false) {
        if (projectsStore?.getAddedSuccessfully) {
          projectsStore.fetchProjects();
          fetchError.value = false;
          open.value = false;
          resetModal();
        } else {
          fetchError.value = true;
        }
      }
    });

    const projectData = {
      projectName: formState.projectName,
      businessUnit: formState.businessUnit,
      teamNumber: formState.teamNumber,
      department: formState.department,
      clientName: formState.clientName,
    };

    await projectsStore?.addProject(projectData);
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
      :ok-button-props="{ disabled: isAdding }"
      @ok="handleOk"
      @cancel="resetModal"
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
