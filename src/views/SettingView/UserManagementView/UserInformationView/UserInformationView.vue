<script lang="ts" setup>
  import { DeleteOutlined, UserOutlined } from '@ant-design/icons-vue';
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';
  import { PlusOutlined } from '@ant-design/icons-vue';
  import { inject, ref } from 'vue';
  import { userStoreSymbol } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';
  import { useRouter } from 'vue-router';
  import { userService } from '@/services';
  import { useEditing } from '@/utils/hooks';
  import FloatingButtonGroup from '@/components/Button/FloatingButtonGroup.vue';
  import ConfirmationDialog from '@/components/Modal/ConfirmAction.vue';
  import { useUserRouting } from '@/utils/hooks';
  import { useFormStore } from '@/components/Form';

  const router = useRouter();
  const userStore = inject(userStoreSymbol)!;
  const { routerUserId, setUserId } = useUserRouting();
  const { getIsLoadingUsers, getIsLoading, getUser, getMe } =
    storeToRefs(userStore);
  const { isEditing, startEditing, stopEditing } = useEditing('isEditingName');
  const user = computed(() => getUser.value);
  const me = computed(() => getMe.value);
  const isLoading = computed(
    () => getIsLoadingUsers.value || getIsLoading.value,
  );

  const nameFormStore = useFormStore('editNameForm');

  const isConfirmModalOpen = ref<boolean>(false);
  const openModal = () => {
    isConfirmModalOpen.value = true;
  };
  const closeModal = () => {
    isConfirmModalOpen.value = false;
  };

  watch(routerUserId , () => {
    userStore.fetchUser(routerUserId.value)
  })

  //Button for adding new User
  const buttons: FloatButtonModel[] = [
    {
      name: 'CreateUserButton',
      onClick: () => {
        router.push('/settings/user-management/create');
      },
      icon: PlusOutlined,
      status: 'activated',
      tooltip: 'Click here to create a new user',
    },
    {
      name: 'DeleteUserButton',
      onClick: () => {
        openModal();
      },
      icon: DeleteOutlined,
      status: 'activated',
      tooltip: 'Click here to delete this user',
    },
  ];

  const deleteUser = async () => {
    if (!user.value) return;
    await userService.deleteUser(user.value?.id);
    await userStore.fetchUsers();
    const firstId = userStore.getUsers[0].id;
    setUserId(firstId);
  };

  const safeNameEdit = async () => {
    await nameFormStore.submit();
    nameFormStore.resetFields();
    stopEditing();
    userStore.fetchUser(user.value!.id)
  };

  const cancleNameEdit = () => {
    nameFormStore.resetFields();
    stopEditing();
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
        <template #icon><UserOutlined /></template>
      </a-avatar>
      <div v-if="!isLoading" class="nameContainer">
        <p v-if="!isEditing" class="text name">{{ user?.name ?? '' }}</p>

        <NameInputTextField
          v-else
          :form-store="nameFormStore"
          :placeholder="user?.name ?? ''"
          :user-id="user?.id ?? -1"
          class="nameInput"
        />
        <EditButtons
          :is-editing="isEditing"
          :is-loading="isLoading"
          :safe-disabled="isLoading"
          @start-editing="startEditing"
          @cancle-edit="cancleNameEdit"
          @safe-edits="safeNameEdit"
        />
      </div>
      <a-skeleton v-else active :paragraph="false" style="width: 10em" />
    </a-flex>

    <!-- User informations components -->
    <a-flex
      class="userInfoBox"
      :body-style="{
        height: 'fit-content',
      }"
    >
      <EditableTextField
        :value="user?.username ?? ''"
        :is-loading="isLoading"
        :label="'Username'"
        :is-editing-key="'isEditingUsername'"
        :user-id="user ? user.id : -1"
        type="username"
        class="textField"
        :placeholder="user?.email"
        @safed-changes="
          async () => user && (await userStore.fetchUser(user.id))
        "
      />
      <EditableTextField
        :value="user?.email ?? ''"
        :is-loading="isLoading"
        :label="'Email'"
        :is-editing-key="'isEditingEmail'"
        class="textField"
        type="email"
        :user-id="user ? user.id : -1"
        :placeholder="user?.email"
        @safed-changes="
          async () => user && (await userStore.fetchUser(user.id))
        "
      />
      <EditablePasswordField
        v-if="me?.id && me.id === user?.id"
        value="**********"
        label="Password"
        :is-editing-key="'isEditingPassword'"
        :is-loading="isLoading"
        :user-id="user.id"
        class="passwordField"
      />
    </a-flex>
  </div>
  <RouterView />
  <FloatingButtonGroup :buttons="buttons" />
</template>

<style scoped>
  .panel {
    min-width: 150px;
  }
  .nameContainer {
    display: flex;
    justify-content: center;
    align-items: center;
    height: 100px;
  }

  .userInfoBox {
    padding: 1em 3em 1em 3em;
    margin-top: 2em;
    border-radius: 10px;
    background-color: white;
    min-width: 450px;
    width: 100%;
    height: auto;
    flex-direction: column;
    flex-wrap: wrap;
  }

  .avatar {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
  }

  .name {
    font-weight: bold;
    font-size: 2em;
    flex-direction: row;
    display: flex;
    align-items: center;
    justify-content: center;
    position: relative;
    margin-right: 10px;
  }

  .nameInput {
    margin-right: 10px;
  }

  .textField {
    height: 5em;
  }
  .edit {
    background-color: icon !important;
  }
  .panel{
    overflow-y: auto
  }
</style>
