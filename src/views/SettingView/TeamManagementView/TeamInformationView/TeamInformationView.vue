<script lang="ts" setup>
  import { PlusOutlined, DeleteOutlined } from '@ant-design/icons-vue';
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';
  import { inject, ref } from 'vue';
  import { teamRoutingSymbol, teamStoreSymbol } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';
  import { useRouter } from 'vue-router';
  import FloatingButtonGroup from '@/components/Button/FloatingButtonGroup.vue';
  import ConfirmationDialog from '@/components/Modal/ConfirmAction.vue';
  import { useFormStore } from '@/components/Form';
  import { TeamNameInputField } from '@/components/EditableTextField';
  import { useThemeToken } from '@/utils/hooks';
  import { message } from 'ant-design-vue';

  const token = useThemeToken();

  const router = useRouter();
  const route = useRoute();
  const teamStore = inject(teamStoreSymbol)!;
  const { getTeam, getIsLoadingTeam, getLinkedProjects } =
    storeToRefs(teamStore);
  const team = computed(() => getTeam.value);
  const linkedProjects = computed(() => getLinkedProjects.value);
  const isLoading = computed(() => getIsLoadingTeam.value);
  const { setTeamId } = inject(teamRoutingSymbol)!;

  const emit = defineEmits(['teamDeleted']);

  const teamNameFormStore = useFormStore('editTeamNameForm');
  const businessUnitFormStore = useFormStore('editBuForm');
  const ptlFormStore = useFormStore('editPtlForm');

  const isConfirmModalOpen = ref<boolean>(false);
  const openModal = () => {
    if (linkedProjects.value.length > 0) {
      message.error(
        `Team is still linked to these projects (ids): [${linkedProjects.value}]`,
        5,
      );
      return;
    }
    isConfirmModalOpen.value = true;
  };
  const closeModal = () => {
    isConfirmModalOpen.value = false;
  };

  //Button for adding new User and deleting User
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
        name: 'CreateTeamButton',
        onClick: () => {
          router.push('/settings/team-management/create');
        },
        icon: PlusOutlined,
        type: 'primary',
        size: 'large',
        status: 'activated',
        tooltip: 'Click here to create a new team',
        isLink: false,
      },
    ];
    return tempButtons;
  });

  const deleteTeam = async () => {
    if (!team.value) return;
    await teamStore.delete(team.value?.id);
    emit('teamDeleted');
    teamStore.nullTeam();
    setTeamId(null);
  };
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
  <div v-if="team && team.id" class="panel">
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
        :is-editing-key="'isEditingTeamName'"
        :form-store="teamNameFormStore"
        :has-edit-keys="true"
      >
        <TeamNameInputField
          :team-id="team?.id ?? -1"
          :form-store="teamNameFormStore"
          :placeholder="team?.teamName ?? ''"
          :default="team?.teamName ?? ''"
        />
      </EditableTextField>

      <EditableTextField
        :value="team == undefined ? '' : (team.businessUnit ?? '')"
        :label="'Business\xa0Unit'"
        :is-editing-key="'isEditingBU'"
        :is-loading="isLoading"
        :form-store="businessUnitFormStore"
        :has-edit-keys="true"
      >
        <TeamInformationInputField
          :team-id="team?.id ?? -1"
          :attributeName="'businessUnit'"
          :form-store="businessUnitFormStore"
          :default="team == undefined ? '' : (team.businessUnit ?? '')"
          :placeholder="'BU'"
        >
        </TeamInformationInputField>
      </EditableTextField>
      <EditableTextField
        :value="team == undefined ? '' : (team.ptl ?? '')"
        :label="'PTL'"
        :is-editing-key="'isEditingPTL'"
        :is-loading="isLoading"
        :form-store="ptlFormStore"
        :has-edit-keys="true"
      >
        <TeamInformationInputField
          :team-id="team?.id ?? -1"
          :attributeName="'ptl'"
          :form-store="ptlFormStore"
          :default="team == undefined ? '' : (team.ptl ?? '')"
          :placeholder="'PTL'"
        >
        </TeamInformationInputField>
      </EditableTextField>
    </a-flex>
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
  <FloatingButtonGroup v-if="team" :buttons="buttons" class="floating-buttons" />
  <RouterView />
</template>

<style scoped>
  .panel {
    position: relative; /* Make sure the panel is a positioning context */
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
