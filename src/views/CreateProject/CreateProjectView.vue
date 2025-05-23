<script setup lang="ts">
  import { computed, ref } from 'vue';
  import {
    FontColorsOutlined,
    PlusOutlined,
    NumberOutlined,
    UserOutlined,
    SafetyCertificateOutlined,
    TrademarkOutlined,
    SwapOutlined,
    TeamOutlined,
  } from '@ant-design/icons-vue';
  import type { UnwrapRef } from 'vue';
  import type { CreateProjectModel } from '@/models/Project';
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';
  import { useProjectStore, useTeamStore } from '@/store';
  import { projectRoutingSymbol } from '@/store/injectionSymbols';
  import { useThemeToken } from '@/utils/hooks';
  import { message } from 'ant-design-vue';
  import { storeToRefs } from 'pinia';
  import type { SelectValue } from 'ant-design-vue/es/select';

  const token = useThemeToken();

  const open = ref<boolean>(false);

  // Form- and Stateconfiguration
  const formRef = ref();
  const labelCol = { style: { width: '150px' } };
  const wrapperCol = { span: 14 };
  const cancelFetch = ref<boolean>();

  // TableStore to refetch Table after Project was added

  const projectStore = useProjectStore();

  const teamStore = useTeamStore();

  const { getTeams } = storeToRefs(teamStore);

  const { setProjectId } = inject(projectRoutingSymbol)!;

  const isAdding = computed(() => projectStore.getIsLoadingAdd);

  const formState: UnwrapRef<CreateProjectModel> = reactive({
    projectName: '',
    businessUnit: '',
    department: '',
    clientName: '',
    offerId: '',
    company: '',
    companyState: 'EXTERNAL',
    ismsLevel: 'NORMAL',
    isArchived: false,
    teamId: undefined,
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
    type: 'primary',
    icon: PlusOutlined,
    status: 'activated',
    size: 'large',
    tooltip: 'Click here to create a new project',
  };

  // opens modal when plus sign is clicked
  const showModal = () => {
    open.value = true;
  };

  const closeModal = () => {
    open.value = false;
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
      .catch((error: unknown) => {
        console.error('error', error);
      });
  };

  // sends PUT request to the backend
  const submit = async () => {
    const projectData: CreateProjectModel = {
      projectName: formState.projectName,
      teamId: formState.teamId!,
      clientName: formState.clientName,
      isArchived: false,
      offerId: formState.offerId,
      company: formState.company,
      companyState: formState.companyState,
      ismsLevel: formState.ismsLevel,
    };

    try {
      await projectStore.create(projectData);
      message.success('Project created successfully');
      resetModal();
      closeModal();
    } catch (error) {
      message.error((error as Error).message ?? 'An error occurred');
      return;
    }
    open.value = false;
    await projectStore.fetchAll();
    const projects = projectStore.getProjects;
    const newProject = projects.find(
      (project) => project.projectName === projectData.projectName,
    );
    setProjectId(newProject?.id ?? undefined);
  };

  // handling because a-select cant handle null values
  const selectedTeamForSelect = computed<SelectValue>({
  get() {
    if(formState.teamId === undefined || formState.teamId === null){
      return undefined;
    }
    const name = teamStore.getNameToId(formState.teamId);
    return name;
  },
  set(newValue) {
    if (newValue === undefined || newValue === null) {
      formState.teamId = undefined;
    } else {
      const id = teamStore.getIdToName(JSON.stringify(newValue));
      formState.teamId = id;
    }
  }
});
</script>

<template>
  <div>
    <FloatingButton :button="button" />

    <!-- Modal for project creation -->
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
        class="formContainer"
      >
        <a-form-item
          name="projectName"
          :rules="[{ required: true, whitespace: true }]"
          class="column"
          :no-style="true"
        >
          <a-input
            v-model:value="formState.projectName"
            placeholder="Project Name"
          >
            <template #suffix>
              <FontColorsOutlined class="icon" />
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
            placeholder="Client Name"
          >
            <template #suffix>
              <UserOutlined class="icon" />
            </template>
          </a-input>
        </a-form-item>
        <a-form-item
          name="offerId"
          :rules="[{ required: true, whitespace: true }]"
          :no-style="true"
        >
          <a-input v-model:value="formState.offerId" placeholder="Offer ID">
            <template #suffix>
              <NumberOutlined class="icon" />
            </template>
          </a-input>
        </a-form-item>
        <a-form-item
          name="company"
          :rules="[{ required: true, whitespace: true }]"
          :no-style="true"
        >
          <a-input v-model:value="formState.company" placeholder="Company">
            <template #suffix>
              <TrademarkOutlined class="icon" />
            </template>
          </a-input>
        </a-form-item>
        <a-form-item
          name="companyState"
          :rules="[{ required: true }]"
          :no-style="true"
        >
          <a-select
            v-model:value="formState.companyState"
            placeholder="Company State"
          >
            <template #suffixIcon>
              <SwapOutlined class="icon" />
            </template>
            <a-select-option value="INTERNAL">Internal</a-select-option>
            <a-select-option value="EXTERNAL">External</a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item
          name="ismsLevel"
          :rules="[{ required: true }]"
          :no-style="true"
        >
          <a-select
            v-model:value="formState.ismsLevel"
            placeholder="ISMS Level"
          >
            <template #suffixIcon>
              <SafetyCertificateOutlined class="icon" />
            </template>
            <a-select-option value="NORMAL">Normal</a-select-option>
            <a-select-option value="HIGH">High</a-select-option>
            <a-select-option value="VERY_HIGH">Very High</a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item
        name="teamId"
        :rules="[{ required: false }]"
        :no-style="true"
        >
        <a-select
          v-model:value="selectedTeamForSelect"
          placeholder="Team"
          show-search
        >
            <template #suffixIcon>
              <TeamOutlined class="icon" />
            </template>
          <a-select-option :value="undefined">{{ "<null>" }}</a-select-option>
          <a-select-option v-for="team in getTeams" :key="team.teamName" :value="team.teamName">{{ team.teamName }}</a-select-option>
        </a-select>
      </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>



<style scoped lang="scss">
  .formItem {
    max-width: none !important;
  }
  .formContainer > * {
    margin-bottom: 20px;
  }
  .icon {
    width: 12px;
    height: 12px;
    color: v-bind('token.colorText');
  }
  :deep(.ant-select .ant-select-arrow) {
    color: unset;
  }
</style>
