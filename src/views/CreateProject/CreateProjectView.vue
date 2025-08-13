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
    CommentOutlined,
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
    notes: '',
  });

  // needed for mapping type null -> undefined in form input
  const displayOfferId = computed({
    get() {
      return formState.offerId === null ? undefined : formState.offerId;
    },
    set(newValue: string | undefined) {
      formState.offerId =
        newValue === undefined || newValue === '' ? null : newValue;
    },
  });

  const validateMessages = {
    required: 'Please enter valid input.',
    whitespace: 'Please enter valid input.',
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
      notes: formState.notes ?? '',
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

  // mapping team name on the select input -> id / id -> name
  const selectedTeamForSelect = computed<SelectValue>({
    get() {
      if (formState.teamId === undefined || formState.teamId === null) {
        return undefined;
      }
      const name = teamStore.getNameToId(formState.teamId);
      return name;
    },
    set(newValue: SelectValue) {
      if (newValue === undefined || newValue === null) {
        formState.teamId = undefined;
      } else if (typeof newValue === 'string') {
        const id = teamStore.getIdToName(newValue);
        formState.teamId = id;
      } else {
        console.warn(
          'Unexpected newValue type for selectedTeamForSelect:',
          newValue,
        );
        formState.teamId = undefined;
      }
    },
  });

  const getDropdownContainer = () => {
    const globalContainer = document.querySelector(
      '.team-local-popup-container',
    );
    if (globalContainer instanceof HTMLElement) {
      return globalContainer;
    } else {
      return document.body;
    }
  };
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
        <a-form-item name="offerId" :no-style="true">
          <a-input v-model:value="displayOfferId" placeholder="Offer ID">
            <template #suffix>
              <NumberOutlined class="icon" />
            </template>
          </a-input>
        </a-form-item>
        <a-form-item
          name="company"
          :rules="[{ required: true, whitespace: true }]"
        >
          <a-input v-model:value="formState.company" placeholder="Company">
            <template #suffix>
              <TrademarkOutlined class="icon" />
            </template>
          </a-input>
        </a-form-item>
        <a-form-item name="companyState" :rules="[{ required: true }]">
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
        <a-form-item name="ismsLevel" :rules="[{ required: true }]">
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
        <a-form-item name="teamId" :rules="[{ required: false }]">
          <div
            ref="localPopupContainer"
            class="team-local-popup-container"
          ></div>
          <a-select
            v-model:value="selectedTeamForSelect"
            placeholder="Team"
            class="team-select"
            show-search
            allow-clear
            data-test="team-id-select"
            :get-popup-container="getDropdownContainer"
          >
            <template #suffixIcon>
              <TeamOutlined class="icon" />
            </template>
            <a-select-option :value="undefined">{{
              'No Team'
            }}</a-select-option>
            <a-select-option
              v-for="(team, index) in getTeams"
              :key="team.teamName"
              :value="team.teamName"
              :data-testid="'team-select-' + index"
              >{{ team.teamName }}</a-select-option
            >
          </a-select>
        </a-form-item>
        <a-form-item
          name="notes"
          :rules="[{ required: false }]"
        >
          <a-textarea v-model:value="formState.notes" placeholder="Notes" :auto-size="true" :maxlength=500 :show-count="true">
            <template #suffixIcon>
              <CommentOutlined class="icon"/>
            </template>
          </a-textarea />
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
  :deep(.ant-col-14) {
    max-width: 100%;
  }
</style>
