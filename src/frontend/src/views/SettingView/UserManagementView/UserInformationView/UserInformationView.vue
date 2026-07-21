<script lang="ts" setup>
  import {
    CloseOutlined,
    DeleteOutlined,
    EditOutlined,
    SaveOutlined,
    UserOutlined,
  } from '@ant-design/icons-vue';
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';
  import { inject, ref, onBeforeUnmount } from 'vue';
  import { userRoutingSymbol, userStoreSymbol } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';

  import FloatingButtonGroup from '@/components/Button/FloatingButtonGroup.vue';
  import ConfirmationDialog from '@/components/Modal/ConfirmAction.vue';
  import { PasswordInputField } from '@/components/EditableTextField';
  import { useEditing, useThemeToken } from '@/utils/hooks';
  import {
    useBusinessUnitStore,
    useCompanyStore,
    useDepartmentStore,
    useOfficeLocationStore,
    useTeamStore,
  } from '@/store';
  import { ResourceActions } from '@/models/utils';
  import { PatchOperations } from '@/api/generated';
  import { message } from 'ant-design-vue';
  import type { UserListModel, UserModel } from '@/models/User';
  import type { Rule } from 'ant-design-vue/es/form';
  import { isValidEmail } from '@/utils/form/userValidation';

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

  const { isEditing, stopEditing, startEditing } = useEditing();
  const formRef = ref();

  const toggleEditingMode = async () => {
    if (isEditing.value) {
      await stopEditing();
    } else {
      await startEditing();
    }
  };

  onMounted(async () => {
    userStore.fetchMe();
    teamStore.fetchAll();
    departmentStore.fetchAll();
    buStore.fetchAll();
    companyStore.fetchAll();
    officeLocationStore.fetchAll();
  });

  onBeforeUnmount(() => {
    if (isEditing.value) {
      stopEditing();
    }
  });

  const isConfirmModalOpen = ref<boolean>(false);
  const openModal = () => {
    isConfirmModalOpen.value = true;
  };
  const closeModal = () => {
    isConfirmModalOpen.value = false;
  };

  const isCancelModalOpen = ref(false);
  const openCancelModal = () => {
    isCancelModalOpen.value = true;
  };

  const formData = reactive({
    externalId: '',
    userName: '',
    password: '',
    active: true,
    jobTitles: [] as string[],
    teams: [] as string[],
    teamSupport: [] as string[],
    departments: [] as string[],
    businessUnits: [] as string[],
    officeLocation: '',
    company: '',
  });

  watch(
    () => user.value,
    (newUser) => {
      if (!newUser) return;
      formData.externalId = newUser.externalId ?? '';
      formData.userName = newUser.userName ?? '';
      formData.password = newUser.password ?? '';
      formData.active = newUser.active ?? true;
      formData.jobTitles =
        newUser.urnIetfParamsScimSchemasExtensionPmpUser?.jobTitles ?? [];
      formData.teams =
        newUser.urnIetfParamsScimSchemasExtensionPmpUser?.team ?? [];
      formData.teamSupport =
        newUser.urnIetfParamsScimSchemasExtensionPmpUser?.teamSupport ?? [];
      formData.departments =
        newUser.urnIetfParamsScimSchemasExtensionPmpUser?.departments ?? [];
      formData.businessUnits =
        newUser.urnIetfParamsScimSchemasExtensionPmpUser?.businessUnits ?? [];
      formData.officeLocation = newUser.addresses?.[0]?.locality ?? '';
      formData.company =
        newUser.urnIetfParamsScimSchemasExtensionEnterprise20User
          ?.organization ?? '';
    },
  );

  const resetFormData = () => {
    const newUser = user.value;
    if (!newUser) return;

    formData.externalId = newUser.externalId ?? '';
    formData.userName = newUser.userName ?? '';
    formData.password = '';
    formData.active = newUser.active ?? true;

    formData.jobTitles = [
      ...(newUser.urnIetfParamsScimSchemasExtensionPmpUser?.jobTitles ?? []),
    ];
    formData.teams = [
      ...(newUser.urnIetfParamsScimSchemasExtensionPmpUser?.team ?? []),
    ];
    formData.teamSupport = [
      ...(newUser.urnIetfParamsScimSchemasExtensionPmpUser?.teamSupport ?? []),
    ];
    formData.departments = [
      ...(newUser.urnIetfParamsScimSchemasExtensionPmpUser?.departments ?? []),
    ];
    formData.businessUnits = [
      ...(newUser.urnIetfParamsScimSchemasExtensionPmpUser?.businessUnits ??
        []),
    ];

    formData.officeLocation = newUser.addresses?.[0]?.locality ?? '';
    formData.company =
      newUser.urnIetfParamsScimSchemasExtensionEnterprise20User?.organization ??
      '';
  };

  watch(
    () => user.value?.id,
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
  const areArraysEqual = (
    arr1: string[] = [],
    arr2: string[] = [],
  ): boolean => {
    if (arr1.length !== arr2.length) return false;
    const sorted1 = [...arr1].sort();
    const sorted2 = [...arr2].sort();
    return sorted1.every((val, index) => val === sorted2[index]);
  };
  type ScimValue = string | boolean | string[] | null;
  type FormFieldValue = ScimValue | undefined;

  interface ScimPatchOperation {
    op: PatchOperations;
    path: string;
    value: ScimValue;
  }
  const handleBulkSave = async () => {
    try {
      await formRef.value.validate();
      if (!user.value?.id) return;

      const u = user.value;
      const pmpExt = u.urnIetfParamsScimSchemasExtensionPmpUser ?? {};
      const entExt = u.urnIetfParamsScimSchemasExtensionEnterprise20User ?? {};

      const operations: ScimPatchOperation[] = [];

      const addIfChanged = (
        path: string,
        newVal: FormFieldValue,
        oldVal: FormFieldValue,
        isArray = false,
      ) => {
        let hasChanged;

        if (isArray) {
          const arrNew = (newVal ?? []) as string[];
          const arrOld = (oldVal ?? []) as string[];
          hasChanged = !areArraysEqual(arrNew, arrOld);
        } else {
          const normNew = newVal ?? '';
          const normOld = oldVal ?? '';
          hasChanged = normNew !== normOld;
        }

        if (hasChanged) {
          const isEmpty = isArray
            ? ((newVal as string[]) ?? []).length === 0
            : newVal === '' || newVal === null || newVal === undefined;

          operations.push({
            op: isEmpty ? PatchOperations.Remove : PatchOperations.Replace,
            path: path,
            value: newVal === undefined ? null : newVal,
          });
        }
      };

      addIfChanged('externalId', formData.externalId, u.externalId);
      addIfChanged('userName', formData.userName, u.userName);
      addIfChanged('active', formData.active, u.active);

      addIfChanged(
        'urn:ietf:params:scim:schemas:extension:pmp:User:jobTitles',
        formData.jobTitles,
        pmpExt.jobTitles,
        true,
      );
      addIfChanged(
        'urn:ietf:params:scim:schemas:extension:pmp:User:departments',
        formData.departments,
        pmpExt.departments,
        true,
      );
      addIfChanged(
        'urn:ietf:params:scim:schemas:extension:pmp:User:businessUnits',
        formData.businessUnits,
        pmpExt.businessUnits,
        true,
      );
      addIfChanged(
        'urn:ietf:params:scim:schemas:extension:pmp:User:team',
        formData.teams,
        pmpExt.team,
        true,
      );
      addIfChanged(
        'urn:ietf:params:scim:schemas:extension:pmp:User:teamSupport',
        formData.teamSupport,
        pmpExt.teamSupport,
        true,
      );

      addIfChanged(
        'addresses[type eq "work"].locality',
        formData.officeLocation,
        u.addresses?.[0]?.locality,
      );
      addIfChanged(
        'urn:ietf:params:scim:schemas:extension:enterprise:2.0:User:organization',
        formData.company,
        entExt.organization,
      );

      if (formData.password && formData.password.trim() !== '') {
        operations.push({
          op: PatchOperations.Replace,
          path: 'password',
          value: formData.password,
        });
      }

      if (operations.length === 0) {
        message.info(
          'No changes detected \n You did not modify any fields for this user.',
        );
        return;
      }

      await userStore.update(user.value.id, { operations });

      const targetId = formData.externalId || user.value.externalId;
      await userStore.fetchUser(targetId);

      if (formData.externalId && formData.externalId !== routerUserId.value) {
        setUserId(formData.externalId);
      }

      if (targetId === me.value?.externalId) {
        userStore.fetchMe();
      }
      message.success('User updated successfully');
      await stopEditing();
    } catch (error) {
      console.error('Validation or API error:', error);
      message.error('Failed to update user. Please check your inputs.');
    }
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
      {
        name: 'EditUserButton',
        onClick: () => {
          toggleEditingMode();
        },
        icon: EditOutlined,
        type: 'primary',
        size: 'large',
        status: 'activated',
        tooltip: 'Click here to edit this user',
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
      me.value?.externalId == user.value?.externalId ||
      !routerUserId.value ||
      isEditing.value ||
      !(user.value?.meta?.permissions ?? []).includes(ResourceActions.Delete)
    )
      tempButtons[0].status = 'deactivated';

    if (
      !routerUserId.value ||
      isEditing.value ||
      !(user.value?.meta?.permissions ?? []).includes(ResourceActions.Edit)
    )
      tempButtons[1].status = 'deactivated';

    if (!isEditing.value) {
      tempButtons[2].status = 'deactivated';
      tempButtons[3].status = 'deactivated';
    }
    if (!(user.value?.meta?.permissions ?? []).includes(ResourceActions.Edit)) {
      tempButtons[3].status = 'deactivated';
    }
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

  const isUniqueEmail = (_rule: Rule, email: string) => {
    const users: UserListModel[] = userStore.getUsers;
    const currentUser: UserModel | null = userStore.getUser;

    if (
      users?.every(
        (user) =>
          user.userName !== email || user.userName === currentUser?.userName,
      )
    ) {
      return Promise.resolve();
    }
    return Promise.reject(new Error('This email is already in use.'));
  };

  const emailRules: Rule[] = [
    {
      required: true,
      message: 'Please insert a valid email.',
      validator: isValidEmail,
      trigger: 'change',
      type: 'string',
    },
    {
      required: true,
      message: 'Please insert a unique email.',
      validator: isUniqueEmail,
      trigger: 'change',
      type: 'email',
    },
  ];

  const isUniqueEmployeeNumber = (_rule: Rule, employeeNumber: string) => {
    const users: UserListModel[] = userStore.getUsers;
    const currentUser: UserModel | null = userStore.getUser;

    if (
      users?.every(
        (user) =>
          user.externalId !== employeeNumber ||
          user.externalId === currentUser?.externalId,
      )
    ) {
      return Promise.resolve();
    }
    return Promise.reject(new Error('This employee number is already in use.'));
  };

  const employeeNumberRules: Rule[] = [
    { required: true },
    {
      required: true,
      message: 'Please insert a unique employee number.',
      validator: isUniqueEmployeeNumber,
      trigger: 'change',
      type: 'string',
    },
  ];
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
  <ConfirmAction
    :is-open="isCancelModalOpen"
    title="Cancel Editing"
    message="Are you sure you want to cancel all changes?"
    @confirm="stopEditing"
    @cancel="isCancelModalOpen = false"
    @update:is-open="(value) => (isCancelModalOpen = value)"
  />
  <div v-if="user?.externalId" class="panel">
    <a-flex class="avatar">
      <a-avatar :size="150">
        <template #icon>
          <UserOutlined />
        </template>
      </a-avatar>
    </a-flex>
    <a-form ref="formRef" :model="formData" layout="vertical">
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
          :is-editing-key="'isEditing'"
          :has-edit-keys="false"
        >
          <InformationInputField
            v-model:value="formData.externalId"
            :placeholder="user?.externalId ?? ''"
            attribute-name="externalId"
            :rules="employeeNumberRules"
          />
        </EditableTextField>

        <EditableTextField
          class="textField email"
          :value="user?.userName ?? ''"
          :is-loading="isLoading"
          :label="'Email'"
          :is-editing-key="'isEditing'"
          :has-edit-keys="false"
        >
          <InformationInputField
            v-model:value="formData.userName"
            attribute-name="userName"
            :placeholder="user?.userName ?? ''"
            :rules="emailRules"
          />
        </EditableTextField>

        <EditableTextField
          v-if="me?.externalId == user?.externalId || isEditing"
          :value="'**********'"
          :label="'Password'"
          :is-editing-key="'isEditing'"
          :is-loading="isLoading"
          :has-edit-keys="false"
        >
          <PasswordInputField v-model:value="formData.password" />
        </EditableTextField>

        <EditableInputSwitch
          v-model:checked="formData.active"
          attributeName="active"
          label="Active"
          :disabled="!isEditing"
          :is-loading="isLoading"
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
          :is-editing-key="'isEditing'"
          :has-edit-keys="false"
        >
          <InformationListInputField
            v-model:value="formData.jobTitles"
            :attribute-name="'JobTitles'"
            mode="tags"
            :placeholder="
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
            user?.urnIetfParamsScimSchemasExtensionPmpUser.team?.join(', ') ??
            ''
          "
          :is-loading="isLoading"
          :label="'Teams'"
          :is-editing-key="'isEditing'"
          :has-edit-keys="false"
        >
          <InformationListInputField
            v-model:value="formData.teams"
            :attribute-name="'Teams'"
            :options="teamStore.getTeamNames"
            :placeholder="
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
          :is-editing-key="'isEditing'"
          :has-edit-keys="false"
        >
          <InformationListInputField
            v-model:value="formData.teamSupport"
            :attribute-name="'TeamSupport'"
            :options="teamStore.getTeamNames"
            :placeholder="
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
          :is-editing-key="'isEditing'"
          :has-edit-keys="false"
        >
          <InformationListInputField
            v-model:value="formData.departments"
            :attribute-name="'Departments'"
            :placeholder="
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
          :is-editing-key="'isEditing'"
          :has-edit-keys="false"
        >
          <InformationListInputField
            v-model:value="formData.businessUnits"
            :attribute-name="'BusinessUnits'"
            :placeholder="
              user?.urnIetfParamsScimSchemasExtensionPmpUser.businessUnits?.join(
                ', ',
              ) ?? ''
            "
            :options="buStore.getBusinessUnitNames"
          />
        </EditableTextField>
        <EditableTextField
          class="textField officeLocation"
          :value="(user?.addresses ?? [{}])[0]?.locality ?? ''"
          :is-loading="isLoading"
          :label="'Office Location'"
          :is-editing-key="'isEditing'"
          :has-edit-keys="false"
        >
          <InformationAutoCompleteInputField
            v-model:value="formData.officeLocation"
            :attribute-name="'OfficeLocation'"
            :placeholder="(user?.addresses ?? [{}])[0]?.locality ?? ''"
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
          :is-editing-key="'isEditing'"
          :has-edit-keys="false"
        >
          <InformationAutoCompleteInputField
            v-model:value="formData.company"
            :attribute-name="'Company'"
            :placeholder="
              user?.urnIetfParamsScimSchemasExtensionEnterprise20User
                .organization ?? ''
            "
            :options="companyStore.getCompanyNames"
          />
        </EditableTextField>
      </a-flex>
    </a-form>
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
