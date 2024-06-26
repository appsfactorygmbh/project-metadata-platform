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
  import type { CreateProjectModel } from '@/models/Project';
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';

  const open = ref<boolean>(false);
  const formRef = ref();
  const labelCol = { style: { width: '150px' } };
  const wrapperCol = { span: 14 };
  const cancelFetch = ref<boolean>();

  // TableStore to refetch Table after Project was added
  const projectsStore = inject(projectsStoreSymbol);

  const isAdding = computed(() => projectsStore?.getIsLoadingAdd);
  const fetchError = ref<boolean>(false);

  const formState: UnwrapRef<CreateProjectModel> = reactive({
    projectName: '',
    businessUnit: '',
    teamNumber: undefined,
    department: '',
    clientName: '',
  });
  const validateMessages = {
    required: 'Please enter valid input.',
    types: {
      number: 'Team number is not a valid number!',
    },
    number: {
      range: 'Team number must be positive number.',
    },
  };

  const button: FloatButtonModel = {
    name: 'CreateProjectButton',
    onClick: () => {
      showModal();
    },
    icon: PlusOutlined,
    status: 'activated',
    tooltip: 'Click here to create a new project',
  };

  // opens modal when plussign is clicked
  const showModal = () => {
    open.value = true;
  };

  const resetModal = () => {
    formRef.value.resetFields();
    fetchError.value = false;
  };

  // checks for correct input
  const handleOk = () => {
    cancelFetch.value = false;
    formRef.value
      .validate()
      .then(() => {
        submit();
      })
      .catch((error: unknown) => {
        console.log('error', error);
      });
  };

  // sends PUT request to the backend
  const submit = async () => {
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

    const projectData: CreateProjectModel = {
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
    <FloatingButton :button="button" />

    <a-modal
      v-model:open="open"
      width="400px"
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
        <a-form-item
          name="projectName"
          :rules="[{ required: true, whitespace: true }]"
          class="column"
          :no-style="true"
          :whitespace="true"
        >
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
        <a-form-item
          name="businessUnit"
          :rules="[{ required: true, whitespace: true }]"
          :no-style="true"
        >
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
          :no-style="true"
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
        <a-form-item
          name="department"
          :rules="[{ required: true, whitespace: true }]"
          :no-style="true"
        >
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
        <a-form-item
          name="clientName"
          :rules="[{ required: true, whitespace: true }]"
          :no-style="true"
        >
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
  .formItem {
    max-width: none !important;
  }

  .inputField {
    width: 100%;
    margin: 10px 0 10px 0;
  }
</style>
