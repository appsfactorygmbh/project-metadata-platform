<script lang="ts" setup>
  import {
    CloseOutlined,
    DeleteOutlined,
    EditOutlined,
    SaveOutlined,
  } from '@ant-design/icons-vue';
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';
  import { inject, ref } from 'vue';
  import { teamRoutingSymbol, teamStoreSymbol } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';

  import FloatingButtonGroup from '@/components/Button/FloatingButtonGroup.vue';
  import ConfirmationDialog from '@/components/Modal/ConfirmAction.vue';
  import { useEditing, useThemeToken } from '@/utils/hooks';
  import { message } from 'ant-design-vue';
  import { useBusinessUnitStore } from '@/store';
  import { ResourceActions } from '@/models/utils';
  import type { Rule } from 'ant-design-vue/es/form';
  import type { TeamModel } from '@/models/Team';

  const token = useThemeToken();

  const route = useRoute();

  const { isEditing, stopEditing, startEditing } = useEditing();
  const formRef = ref();

  const teamStore = inject(teamStoreSymbol)!;
  const { getTeam, getIsLoadingTeam, getLinkedProjects } =
    storeToRefs(teamStore);
  const buStore = useBusinessUnitStore();
  const team = computed(() => getTeam.value);
  const linkedProjects = computed(() => getLinkedProjects.value);
  const isLoading = computed(() => getIsLoadingTeam.value);
  const { setTeamId } = inject(teamRoutingSymbol)!;

  const emit = defineEmits(['teamDeleted']);

  onMounted(async () => {
    buStore.fetchAll();
  });

  onBeforeUnmount(() => {
    if (isEditing.value) {
      stopEditing();
    }
  });
  const toggleEditingMode = async () => {
    if (isEditing.value) {
      await stopEditing();
    } else {
      await startEditing();
    }
  };

  const formData = reactive({
    teamName: '',
    businessUnitId: undefined as number | undefined,
    ptl: '',
  });

  watch(
    () => team.value,
    (newTeam) => {
      if (!newTeam) return;
      formData.teamName = newTeam.teamName ?? '';
      formData.businessUnitId = newTeam.businessUnit.id ?? undefined;
      formData.ptl = newTeam.ptl ?? '';
    },
  );

  const resetFormData = () => {
    const newTeam = team.value;
    if (!newTeam) return;
    formData.teamName = newTeam.teamName ?? '';
    formData.businessUnitId = newTeam.businessUnit.id ?? undefined;
    formData.ptl = newTeam.ptl ?? '';
  };

  watch(
    () => team.value?.id,
    (newId, oldId) => {
      if (!newId || newId === oldId) return;
      stopEditing();
      resetFormData();
    },
    { immediate: true },
  );
  watch(isEditing, (newVal) => {
    if (!newVal) {
      resetFormData();
    }
  });

  const handleBulkSave = async () => {
    try {
      await formRef.value.validate();
      if (!team.value?.id) return;

      const updateRequest = {
        teamName: formData.teamName,
        businessUnitId: formData.businessUnitId,
        ptl: formData.ptl,
      };

      await teamStore.update(team.value.id, updateRequest);

      await teamStore.fetch(team.value?.id);

      message.success('Team updated successfully');
      await stopEditing();
    } catch (error) {
      console.error('Validation or API error:', error);
      message.error('Failed to update team. Please check your inputs.');
    }
  };

  const isConfirmModalOpen = ref<boolean>(false);
  const openModal = () => {
    if (linkedProjects.value.length > 0) {
      message.error(
        `Team is still linked to these projects: [${linkedProjects.value}]`,
        5,
      );
      return;
    }
    isConfirmModalOpen.value = true;
  };
  const closeModal = () => {
    isConfirmModalOpen.value = false;
  };

  const isCancelModalOpen = ref(false);
  const openCancelModal = () => {
    isCancelModalOpen.value = true;
  };

  //Button for adding new Team and deleting Teams
  const buttons = computed((): FloatButtonModel[] => {
    const tempButtons: FloatButtonModel[] = [
      {
        name: 'DeleteTeamButton',
        onClick: () => {
          openModal();
        },
        icon: DeleteOutlined,
        type: 'primary',
        specialType: 'danger',
        size: 'large',
        status: 'activated',
        tooltip: 'Click here to delete this team',
        isLink: false,
      },
      {
        name: 'EditTeamButton',
        onClick: () => {
          toggleEditingMode();
        },
        icon: EditOutlined,
        type: 'primary',
        size: 'large',
        status: 'activated',
        tooltip: 'Click here to edit this team',
        isLink: false,
      },
      {
        name: 'CancelButton',
        onClick: () => {
          openCancelModal();
        },
        icon: CloseOutlined,
        status: 'activated',
        type: 'primary',
        size: 'large',
        specialType: 'danger',
        tooltip: 'Click to cancel editing',
      },
      {
        name: 'SafeEditButton',
        onClick: () => {
          handleBulkSave();
        },
        icon: SaveOutlined,
        status: 'activated',
        type: 'primary',
        size: 'large',
        specialType: 'success',
        tooltip: 'Click to save changes',
      },
    ];
    if (
      !team.value ||
      isEditing.value ||
      !teamStore.getPermissions.includes(ResourceActions.Delete)
    )
      tempButtons[0].status = 'deactivated';

    if (
      !team.value?.id ||
      isEditing.value ||
      !team.value?.permissions?.includes(ResourceActions.Edit)
    )
      tempButtons[1].status = 'deactivated';

    if (!isEditing.value) {
      tempButtons[2].status = 'deactivated';
      tempButtons[3].status = 'deactivated';
    }
    if (!team.value?.permissions?.includes(ResourceActions.Edit)) {
      tempButtons[3].status = 'deactivated';
    }
    return tempButtons;
  });

  const deleteTeam = async () => {
    if (!team.value) return;
    await teamStore.delete(team.value?.id);
    emit('teamDeleted');
    teamStore.nullTeam();
    setTeamId(null);
  };

  const isUniqueTeamName = (_rule: Rule, name: string) => {
    const teams: TeamModel[] = teamStore.getTeams;
    const currentTeam: TeamModel | undefined = teamStore.getTeam;
    if (!currentTeam) {
      return Promise.reject(new Error('Current team undefined'));
    }
    if (
      teams?.every(
        (team) => team.teamName !== name || name === currentTeam.teamName,
      )
    ) {
      return Promise.resolve();
    }
    return Promise.reject(new Error('This team name is already in use.'));
  };

  const teamNameRules: Rule[] = [
    {
      required: true,
      message: 'Please insert an unique team name.',
      validator: isUniqueTeamName,
      trigger: 'change',
      type: 'string',
    },
  ];
</script>
<template>
  <ConfirmationDialog
    :is-open="isConfirmModalOpen"
    title="Delete confirm"
    message="Are you sure you want to delete this team?"
    @confirm="deleteTeam"
    @cancel="closeModal"
    @update:is-open="isConfirmModalOpen = $event"
  />
  <ConfirmAction
    :is-open="isCancelModalOpen"
    title="Cancel Editing"
    message="Are you sure you want to cancel all changes?"
    @confirm="stopEditing"
    @cancel="isCancelModalOpen = false"
    @update:is-open="(value) => (isCancelModalOpen = value)"
  />
  <div v-if="team && team.id" class="panel">
    <a-form ref="formRef" :model="formData" layout="vertical">
      <a-flex
        class="userInfoBox"
        :body-style="{
          height: 'fit-content',
        }"
      >
        <EditableTextField
          class="textField teamName"
          :value="team?.teamName ?? ''"
          :is-loading="isLoading"
          :label="'Team\xa0Name'"
          :is-editing-key="'isEditing'"
          :has-edit-keys="false"
        >
          <InformationInputField
            v-model:value="formData.teamName"
            attribute-name="teamName"
            :placeholder="team?.teamName ?? ''"
            :rules="teamNameRules"
          />
        </EditableTextField>

        <EditableTextField
          :value="
            team == undefined ? '' : (team.businessUnit.businessUnitName ?? '')
          "
          :label="'Business\xa0Unit'"
          :is-editing-key="'isEditing'"
          :is-loading="isLoading"
          :has-edit-keys="false"
        >
          <InformationSearchSelectField
            v-model:value="formData.businessUnitId"
            :attributeName="'businessUnitId'"
            :placeholder="team?.businessUnit?.businessUnitName ?? ''"
            :options="
              buStore.getBusinessUnits.map((bu) => ({
                id: bu.id,
                name: bu.businessUnitName,
              }))
            "
          />
        </EditableTextField>
        <EditableTextField
          :value="team == undefined ? '' : (team.ptl ?? '')"
          :label="'PTL'"
          :is-editing-key="'isEditing'"
          :is-loading="isLoading"
          :has-edit-keys="false"
        >
          <InformationInputField
            v-model:value="formData.ptl"
            :attributeName="'ptl'"
            :placeholder="'PTL'"
          >
          </InformationInputField>
        </EditableTextField>
      </a-flex>
    </a-form>
  </div>
  <a-skeleton
    v-else-if="isLoading"
    :description="`No Team Found for Id ${route.query.teamId}`"
  ></a-skeleton>
  <a-empty
    v-else-if="route.query.teamId"
    :description="`No Team Found for Id ${route.query.teamId}`"
  ></a-empty>
  <a-empty v-else description="No Team Selected"></a-empty>
  <FloatingButtonGroup :buttons="buttons" class="floating-buttons" />
  <RouterView />
</template>

<style scoped>
  .panel {
    position: relative;
    min-width: 150px;
    max-height: 100vh;
    overflow-y: auto;
  }

  .ant-float-btn-group {
    height: max-content !important;
    width: max-content !important;
    position: absolute;
    right: 20px;
    bottom: 40px;
  }

  .userInfoBox {
    padding: 1em 3em;
    margin: 2em 1em;
    border-radius: 10px;
    background-color: v-bind('token.colorBgElevated');
    min-width: 450px;
    height: auto;
    flex-direction: column;
    flex-wrap: wrap;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  }

  :deep(.ant-card) {
    margin: 0.5em 0;
    background-color: v-bind('token.colorBgElevated');
  }

  .avatar {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
  }
</style>
