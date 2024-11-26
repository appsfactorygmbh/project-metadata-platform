<script lang="ts" setup>
  import { DeleteOutlined, UserOutlined } from '@ant-design/icons-vue';
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';
  import { PlusOutlined } from '@ant-design/icons-vue';
  import { inject, ref } from 'vue';
  import { userStoreSymbol } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';
  import { useRouter } from 'vue-router';
  import { userService } from '@/services';
  import FloatingButtonGroup from '@/components/Button/FloatingButtonGroup.vue';
  import ConfirmationDialog from '@/components/Modal/ConfirmAction.vue';
  import { useUserRouting } from '@/utils/hooks';

  const router = useRouter();
  const userStore = inject(userStoreSymbol)!;
  const { routerUserId, setUserId } = useUserRouting();
  const { getIsLoadingUsers, getIsLoading, getUser, getMe } =
    storeToRefs(userStore);
  const user = computed(() => getUser.value);
  const me = computed(() => getMe.value);
  const isLoading = computed(
    () => getIsLoadingUsers.value || getIsLoading.value,
  );

  const isConfirmModalOpen = ref<boolean>(false);
  const openModal = () => {
    isConfirmModalOpen.value = true;
  };
  const closeModal = () => {
    isConfirmModalOpen.value = false;
  };

  watch(routerUserId, () => {
    userStore.fetchUser(routerUserId.value);
  });

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
    const firstId: number = userStore.getUsers[0].id;
    setUserId(firstId);
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
    </a-flex>

    <!-- User informations components -->
    <a-flex
      class="userInfoBox"
      :body-style="{
        height: 'fit-content',
      }"
    >
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
    margin-bottom: 100px;
  }

  .avatar {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
  }

  .textField {
    height: 5em;
  }

  .panel {
    position: relative;
    max-height: 100vh; /* Set a maximum height */
    overflow-y: auto; /* Enable vertical scrolling */
  }
</style>
