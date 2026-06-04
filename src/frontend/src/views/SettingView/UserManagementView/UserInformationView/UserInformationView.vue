<script lang="ts" setup>
  import { DeleteOutlined, UserOutlined } from '@ant-design/icons-vue';
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';
  import { inject, ref } from 'vue';
  import { userRoutingSymbol, userStoreSymbol } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';

  import FloatingButtonGroup from '@/components/Button/FloatingButtonGroup.vue';
  import ConfirmationDialog from '@/components/Modal/ConfirmAction.vue';
  import { useFormStore } from '@/components/Form';
  import {
    EmailInputField,
    PasswordInputField,
  } from '@/components/EditableTextField';
  import { useThemeToken } from '@/utils/hooks';
  import {
    useBusinessUnitStore,
    useCompanyStore,
    useDepartmentStore,
    useOfficeLocationStore,
    useTeamStore,
  } from '@/store';

  const token = useThemeToken();

  const route = useRoute();
  const userStore = inject(userStoreSymbol)!;
  const { setUserId, routerUserId } = inject(userRoutingSymbol)!;
  const { getIsLoadingUser, getIsLoading, getUser, getMe } =
    storeToRefs(userStore);
  const user = computed(() => getUser.value);
  const me = computed(() => getMe.value);
  const isLoading = computed(
    () => getIsLoadingUser.value || getIsLoading.value,
  );
  const teamStore = useTeamStore();
  const departmentStore = useDepartmentStore();
  const buStore = useBusinessUnitStore();
  const companyStore = useCompanyStore();
  const officeLocationStore = useOfficeLocationStore();
  const employeeNrFormStore = useFormStore('editemployeeNrForm');
  const emailFormStore = useFormStore('editEmailForm');
  const passwordFormStore = useFormStore('patchPasswordForm');
  const isActiveFormStore = useFormStore('editIsActiveForm');
  const companyFormStore = useFormStore('editCompanyForm');
  const officeLocationFormStore = useFormStore('editofficeLocationForm');
  const departmentsFormStore = useFormStore('editDepartmentsForm');
  const jobTitlesFormStore = useFormStore('editJobTitlesForm');
  const teamsFormStore = useFormStore('editTeamsForm');
  const teamSupportFormStore = useFormStore('editTeamSupportForm');
  const businessUnitFormStore = useFormStore('editbusinessUnitsForm');

  onMounted(async () => {
    teamStore.fetchAll();
    departmentStore.fetchAll();
    buStore.fetchAll();
    companyStore.fetchAll();
    officeLocationStore.fetchAll();
  });

  const isConfirmModalOpen = ref<boolean>(false);
  const openModal = () => {
    isConfirmModalOpen.value = true;
  };
  const closeModal = () => {
    isConfirmModalOpen.value = false;
  };

  //Button for adding new User and deleting User
  const buttons = computed((): FloatButtonModel[] => {
    const tempButtons: FloatButtonModel[] = [
      {
        name: 'DeleteUserButton',
        onClick: () => {
          openModal();
        },
        icon: DeleteOutlined,
        type: 'primary',
        specialType: 'danger',
        size: 'large',
        status: 'activated',
        tooltip: 'Click here to delete this user',
        isLink: false,
      },
    ];
    if (me.value?.externalId == user.value?.externalId || !routerUserId.value)
      tempButtons[0].status = 'deactivated';

    return tempButtons;
  });

  const deleteUser = async () => {
    if (!user.value) return;
    await userStore.delete(user.value?.externalId);
    await userStore.fetchAll();
    await userStore.fetchMe();
    const myId: string =
      userStore.getMe?.externalId ?? userStore.getUsers[0]?.externalId ?? '1';
    setUserId(myId);
  };
</script>
<template>
  <ConfirmationDialog
    :is-open="isConfirmModalOpen"
    title="Delete confirm"
    message="Are you sure you want to delete this user?"
    @confirm="deleteUser"
    @cancel="closeModal"
    @update:is-open="isConfirmModalOpen = $event"
  />
  <div v-if="user?.externalId" class="panel">
    <a-flex class="avatar">
      <a-avatar :size="150">
        <template #icon>
          <UserOutlined />
        </template>
      </a-avatar>
    </a-flex>

    <a-flex
      class="userInfoBox"
      :body-style="{
        height: 'fit-content',
      }"
    >
      <EditableTextField
        class="textField employeeNr"
        :value="user?.externalId ?? ''"
        :is-loading="isLoading"
        :label="'Employee Nr.'"
        :is-editing-key="'isEditingEmployeeNr'"
        :form-store="employeeNrFormStore"
        :has-edit-keys="true"
        @saved-changes="
          async () => user && (await userStore.fetchUserByEmail(user.userName))
        "
      >
        <UserInformationInputField
          :user-id="user?.id ?? ''"
          :attribute-name="'externalId'"
          :form-store="employeeNrFormStore"
          :placeholder="user?.externalId ?? ''"
          :default="user?.externalId ?? ''"
        />
      </EditableTextField>

      <EditableTextField
        class="textField email"
        :value="user?.userName ?? ''"
        :is-loading="isLoading"
        :label="'Email'"
        :is-editing-key="'isEditingEmail'"
        :form-store="emailFormStore"
        :has-edit-keys="true"
        @saved-changes="
          async () => user && (await userStore.fetchUser(user.externalId))
        "
      >
        <EmailInputField
          :user-id="user?.id ?? ''"
          :form-store="emailFormStore"
          :placeholder="user?.userName ?? ''"
          :default="user?.userName ?? ''"
        />
      </EditableTextField>

      <EditableTextField
        v-if="me?.id && me.id === user?.id"
        :value="'**********'"
        :label="'Password'"
        :is-editing-key="'isEditingPassword'"
        :is-loading="isLoading"
        :form-store="passwordFormStore"
        :has-edit-keys="true"
        @saved-changes="
          async () => user && (await userStore.fetchUser(user.externalId))
        "
      >
        <PasswordInputField
          :user-id="user?.id ?? ''"
          :form-store="passwordFormStore"
        />
      </EditableTextField>

      <EditableInputSwitch
        class="switch IsActive"
        :user-id="user?.id ?? ''"
        :attribute-name="'IsActive'"
        :label="'Active'"
        :form-store="isActiveFormStore"
        :default="user?.active ?? true"
        :is-loading="isLoading"
        :has-edit-keys="true"
        :is-editing-key="'isActive'"
        @saved-changes="
          async () => user && (await userStore.fetchUser(user.externalId))
        "
      />

      <EditableTextField
        class="textField jobtitles"
        :value="
          user?.urnIetfParamsScimSchemasExtensionPmpUser.jobTitles?.join(
            ', ',
          ) ?? ''
        "
        :is-loading="isLoading"
        :label="'Jobtitles'"
        :is-editing-key="'isEditingJobTitles'"
        :form-store="jobTitlesFormStore"
        :has-edit-keys="true"
        @saved-changes="
          async () => user && (await userStore.fetchUser(user.externalId))
        "
      >
        <UserInformationListInputField
          :user-id="user?.id ?? ''"
          :attribute-name="'JobTitles'"
          :form-store="jobTitlesFormStore"
          :placeholder="
            user?.urnIetfParamsScimSchemasExtensionPmpUser.jobTitles?.join(
              ', ',
            ) ?? ''
          "
          :default="
            user?.urnIetfParamsScimSchemasExtensionPmpUser.jobTitles?.join(
              ', ',
            ) ?? ''
          "
          :options="[]"
        />
      </EditableTextField>

      <EditableTextField
        class="textField teams"
        :value="
          user?.urnIetfParamsScimSchemasExtensionPmpUser.team?.join(', ') ?? ''
        "
        :is-loading="isLoading"
        :label="'Teams'"
        :is-editing-key="'isEditingTeams'"
        :form-store="teamsFormStore"
        :has-edit-keys="true"
        @saved-changes="
          async () => user && (await userStore.fetchUser(user.externalId))
        "
      >
        <UserInformationTeamsInputField
          :user-id="user?.id ?? ''"
          :attribute-name="'Teams'"
          :form-store="teamsFormStore"
          :options="teamStore.getTeamNames"
          :placeholder="
            user?.urnIetfParamsScimSchemasExtensionPmpUser.team?.join(', ') ??
            ''
          "
          :default="
            user?.urnIetfParamsScimSchemasExtensionPmpUser.team?.join(', ') ??
            ''
          "
        />
      </EditableTextField>

      <EditableTextField
        class="textField teamSupport"
        :value="
          user?.urnIetfParamsScimSchemasExtensionPmpUser.teamSupport?.join(
            ', ',
          ) ?? ''
        "
        :is-loading="isLoading"
        :label="'TeamSupport'"
        :is-editing-key="'isEditingTeamSupport'"
        :form-store="teamSupportFormStore"
        :has-edit-keys="true"
        @saved-changes="
          async () => user && (await userStore.fetchUser(user.externalId))
        "
      >
        <UserInformationTeamsInputField
          :user-id="user?.id ?? ''"
          :attribute-name="'TeamSupport'"
          :form-store="teamSupportFormStore"
          :options="teamStore.getTeamNames"
          :placeholder="
            user?.urnIetfParamsScimSchemasExtensionPmpUser.teamSupport?.join(
              ', ',
            ) ?? ''
          "
          :default="
            user?.urnIetfParamsScimSchemasExtensionPmpUser.teamSupport?.join(
              ', ',
            ) ?? ''
          "
        />
      </EditableTextField>

      <EditableTextField
        class="textField departments"
        :value="
          user?.urnIetfParamsScimSchemasExtensionPmpUser.departments?.join(
            ', ',
          ) ?? ''
        "
        :is-loading="isLoading"
        :label="'Departments'"
        :is-editing-key="'isEditingDepartment'"
        :form-store="departmentsFormStore"
        :has-edit-keys="true"
        @saved-changes="
          async () => user && (await userStore.fetchUser(user.externalId))
        "
      >
        <UserInformationListInputField
          :user-id="user?.id ?? ''"
          :attribute-name="'Departments'"
          :form-store="departmentsFormStore"
          :placeholder="
            user?.urnIetfParamsScimSchemasExtensionPmpUser.departments?.join(
              ', ',
            ) ?? ''
          "
          :default="
            user?.urnIetfParamsScimSchemasExtensionPmpUser.departments?.join(
              ', ',
            ) ?? ''
          "
          :options="departmentStore.getDepartmentNames"
        />
      </EditableTextField>

      <EditableTextField
        class="textField businessUnits"
        :value="
          user?.urnIetfParamsScimSchemasExtensionPmpUser.businessUnits?.join(
            ', ',
          ) ?? ''
        "
        :is-loading="isLoading"
        :label="'Business Units'"
        :is-editing-key="'isEditingBusinessUnits'"
        :form-store="businessUnitFormStore"
        :has-edit-keys="true"
        @saved-changes="
          async () => user && (await userStore.fetchUser(user.externalId))
        "
      >
        <UserInformationListInputField
          :user-id="user?.id ?? ''"
          :attribute-name="'BusinessUnits'"
          :form-store="businessUnitFormStore"
          :placeholder="
            user?.urnIetfParamsScimSchemasExtensionPmpUser.businessUnits?.join(
              ', ',
            ) ?? ''
          "
          :default="
            user?.urnIetfParamsScimSchemasExtensionPmpUser.businessUnits?.join(
              ', ',
            ) ?? ''
          "
          :options="buStore.getBusinessUnitNames"
        />
      </EditableTextField>
      <EditableTextField
        class="textField officeLocation"
        :value="user?.addresses[0]?.locality ?? ''"
        :is-loading="isLoading"
        :label="'Office Location'"
        :is-editing-key="'isEditingOfficeLocation'"
        :form-store="officeLocationFormStore"
        :has-edit-keys="true"
        @saved-changes="
          async () => user && (await userStore.fetchUser(user.externalId))
        "
      >
        <UserInformationAutoCompleteInputField
          :user-id="user?.id ?? ''"
          :attribute-name="'OfficeLocation'"
          :form-store="officeLocationFormStore"
          :placeholder="user?.addresses[0]?.locality ?? ''"
          :default="user?.addresses[0]?.locality ?? ''"
          :options="officeLocationStore.getOfficeLocationNames"
        />
      </EditableTextField>
      <EditableTextField
        class="textField company"
        :value="
          user?.urnIetfParamsScimSchemasExtensionEnterprise20User
            .organization ?? ''
        "
        :is-loading="isLoading"
        :label="'Company'"
        :is-editing-key="'isEditingCompany'"
        :form-store="companyFormStore"
        :has-edit-keys="true"
        @saved-changes="
          async () => user && (await userStore.fetchUser(user.externalId))
        "
      >
        <UserInformationAutoCompleteInputField
          :user-id="user?.id ?? ''"
          :attribute-name="'Company'"
          :form-store="companyFormStore"
          :placeholder="
            user?.urnIetfParamsScimSchemasExtensionEnterprise20User
              .organization ?? ''
          "
          :default="
            user?.urnIetfParamsScimSchemasExtensionEnterprise20User
              .organization ?? ''
          "
          :options="companyStore.getCompanyNames"
        />
      </EditableTextField>
    </a-flex>
  </div>
  <a-empty
    v-else-if="route.query.userId"
    :description="`No User Found for Id ${route.query.userId}`"
  ></a-empty>
  <a-empty v-else description="No User Selected"></a-empty>
  <FloatingButtonGroup :buttons="buttons" class="floating-buttons" />
  <RouterView />
</template>

<style scoped>
  .panel {
    position: relative;
    /* Make sure the panel is a positioning context */
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
