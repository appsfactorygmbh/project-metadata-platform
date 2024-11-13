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
  import {
    CheckOutlined,
    CloseOutlined,
    EditOutlined,
  } from '@ant-design/icons-vue';
  import notification from 'ant-design-vue/es/notification';
  import FloatingButtonGroup from '@/components/Button/FloatingButtonGroup.vue';
  import { useUserRouting } from '@/utils/hooks';

  const router = useRouter();
  const userStore = inject(userStoreSymbol)!;
  const { setUserId } = useUserRouting();
  const { getIsLoadingUsers, getIsLoading, getUser, getMe } =
    storeToRefs(userStore);
  const { isEditing, startEditing, stopEditing } = useEditing('isEditingName');
  const user = computed(() => getUser.value);
  const me = computed(() => getMe.value);
  const isLoading = computed(
    () => getIsLoadingUsers.value || getIsLoading.value,
  );
  const nameValue = ref<string>('');

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
        deleteUser();
      },
      icon: DeleteOutlined,
      status: 'activated',
      tooltip: 'Click here to delete this user',
    },
  ];

  const [notificationApi] = notification.useNotification();

  const onSave = async (fieldValue: string, fieldType: string) => {
    console.log(fieldValue);
    if (!user.value) return;
    let reponse;
    switch (fieldType) {
      case 'name':
        reponse = await userService.updateUser(user.value?.id, {
          name: fieldValue,
        });
        break;
      case 'username':
        reponse = await userService.updateUser(user.value?.id, {
          username: fieldValue,
        });
        break;
      case 'email':
        reponse = await userService.updateUser(user.value?.id, {
          email: fieldValue,
        });
        break;
      case 'password':
        reponse = await userService.updateUser(user.value?.id, {
          email: fieldValue,
        });
        break;
    }
    console.log('res status: ', reponse?.status);
    if (reponse?.status !== 200 || reponse.status === undefined)
      notificationApi.error({
        message: 'An error occurred. Could not update the Userinformation',
      });
    userStore.fetchUser(user.value.id);
    stopEditing();
  };

  const deleteUser = async () => {
    if (!user.value) return;
    await userService.deleteUser(user.value?.id);
    await userStore.fetchUsers();
    setUserId(1);
  };
</script>

<template>
  <div class="panel">
    <!-- avatar components -->
    <a-flex class="avatar">
      <a-avatar :size="150">
        <template #icon><UserOutlined /></template>
      </a-avatar>
      <div v-if="!isLoading" class="name">
        <p v-if="!isEditing" class="text">{{ user?.name ?? '' }}</p>

        <a-form v-else name="user" autocomplete="off">
          <a-form-item class="inputName">
            <a-input v-model:value="nameValue" type="text" />
          </a-form-item>
        </a-form>

        <a-button v-if="!isEditing" class="edit" @click="startEditing">
          <EditOutlined class="icon" />
        </a-button>
        <div v-else class="buttonGroup">
          <a-button
            class="edit check"
            :disabled="nameValue === '' ? true : false"
            @click="
              () => {
                onSave(nameValue, 'name');
              }
            "
          >
            <CheckOutlined class="icon" />
          </a-button>
          <a-button class="edit abort" @click="stopEditing">
            <CloseOutlined class="icon" />
          </a-button>
        </div>
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
        @update="
          (fieldValue: string) => {
            onSave(fieldValue, 'username');
          }
        "
      />
      <EditableTextField
        :value="user?.email ?? ''"
        :is-loading="isLoading"
        :label="'Email'"
        :is-editing-key="'isEditingEmail'"
        type="email"
        @update="
          (fieldValue: string) => {
            onSave(fieldValue, 'email');
          }
        "
      />
      <EditablePasswordField
        v-if="me?.id && me.id === user?.id"
        value="**********"
        label="Password"
        :is-editing-key="'isEditingPassword'"
        :is-loading="isLoading"
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
    padding: 1em;
    margin-top: 2em;
    border-radius: 10px;
    background-color: white;
    min-width: 41ch;
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
  }

  .name button {
    border: none;
    background: none;
    color: blue;
    margin-left: 5px;
    gap: 10px;
  }

  .text {
    margin: 0.5em 0 0.5em;
  }

  .inputName {
    font-size: 0.6em;
    margin: 2.2em 0 2.3em;
  }

  .edit {
    background-color: icon !important;
  }
</style>
