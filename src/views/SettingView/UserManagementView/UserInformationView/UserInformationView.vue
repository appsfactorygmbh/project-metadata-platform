<script lang="ts" setup>
  import {
    PlusOutlined,
    DeleteOutlined,
    UserOutlined,
  } from '@ant-design/icons-vue';
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';
  import { inject, ref } from 'vue';
  import { userRoutingSymbol, userStoreSymbol } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';
  import { useRouter } from 'vue-router';
  import FloatingButtonGroup from '@/components/Button/FloatingButtonGroup.vue';
  import ConfirmationDialog from '@/components/Modal/ConfirmAction.vue';
  import { useFormStore } from '@/components/Form';
  import {
    EmailInputField,
    PasswordInputField,
  } from '@/components/EditableTextField';
  import { useThemeToken } from '@/utils/hooks';

  const token = useThemeToken();

  const router = useRouter();
  const userStore = inject(userStoreSymbol)!;
  const { setUserId } = inject(userRoutingSymbol)!;
  const { getIsLoadingUser, getIsLoading, getUser, getMe } =
    storeToRefs(userStore);
  const user = computed(() => getUser.value);
  const me = computed(() => getMe.value);
  const isLoading = computed(
    () => getIsLoadingUser.value || getIsLoading.value,
  );

  const emailFormStore = useFormStore('editEmailForm');
  const passwordFormStore = useFormStore('patchPasswordForm');

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
      },
      {
        name: 'CreateUserButton',
        onClick: () => {
          router.push('/settings/user-management/create');
        },
        icon: PlusOutlined,
        type: 'primary',
        size: 'large',
        status: 'activated',
        tooltip: 'Click here to create a new user',
      },
    ];
    if (me.value?.id == user.value?.id) tempButtons[0].status = 'deactivated';

    return tempButtons;
  });

  const deleteUser = async () => {
    if (!user.value) return;
    await userStore.delete(user.value?.id);
    await userStore.fetchAll();
    await userStore.fetchMe();
    const myId: string =
      userStore.getMe?.id ?? userStore.getUsers[0]?.id ?? '1';
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
  <div class="panel">
    <!-- avatar components -->
    <a-flex class="avatar">
      <a-avatar :size="150">
        <template #icon>
          <UserOutlined />
        </template>
      </a-avatar>
    </a-flex>

    <!-- User informations components -->
    <a-flex
      class="userInfoBox"
      :body-style="{
        height: 'fit-content',
      }"
    >
      <!-- Email Text Field -->
      <EditableTextField
        class="textField email"
        :value="user?.email ?? ''"
        :is-loading="isLoading"
        :label="'Email'"
        :is-editing-key="'isEditingEmail'"
        :form-store="emailFormStore"
        :has-edit-keys="true"
        @saved-changes="
          async () => user && (await userStore.fetchUser(user.id))
        "
      >
        <EmailInputField
          :user-id="user?.id ?? ''"
          :form-store="emailFormStore"
          :placeholder="user?.email ?? ''"
          :default="user?.email ?? ''"
        />
      </EditableTextField>

      <!-- Password Text Field -->
      <EditableTextField
        v-if="me?.id && me.id === user?.id"
        :value="'**********'"
        :label="'Password'"
        :is-editing-key="'isEditingPassword'"
        :is-loading="isLoading"
        :form-store="passwordFormStore"
        :has-edit-keys="true"
      >
        <PasswordInputField
          :user-id="user?.id ?? ''"
          :form-store="passwordFormStore"
        />
      </EditableTextField>
    </a-flex>
  </div>
  <RouterView />
  <FloatingButtonGroup :buttons="buttons" />
</template>

<style scoped>
  .panel {
    min-width: 150px;
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

  .panel {
    position: relative;
    max-height: 100vh; /* Set a maximum height */
    overflow-y: auto; /* Enable vertical scrolling */
  }
</style>
