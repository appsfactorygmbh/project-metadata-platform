<script lang="ts" setup>
  import { UserOutlined } from '@ant-design/icons-vue';
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';
  import { PlusOutlined } from '@ant-design/icons-vue';
  import { inject } from 'vue';
  import { userStoreSymbol } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';
  import { useEditing } from '@/utils/hooks/useEditing';

  const userStore = inject(userStoreSymbol)!;
  const { getIsLoadingUsers, getIsLoading, getUser, getMe } =
    storeToRefs(userStore);
  const {
    isEditing: isEditingName,
    startEditing: startEditingName,
    stopEditing: stopEditingName,
  } = useEditing('isEditingName');
  const {
    isEditing: isEditingUsername,
    startEditing: startEditingUsername,
    stopEditing: stopEditingUsername,
  } = useEditing('isEditingUsername');
  const {
    isEditing: isEditingEmail,
    startEditing: startEditingEmail,
    stopEditing: stopEditingEmail,
  } = useEditing('isEditingEmail');
  const {
    isEditing: isEditingPassword,
    startEditing: startEditingPassword,
    stopEditing: stopEditingPassword,
  } = useEditing('isEditingPassword');

  const toggleEditName = () => {
    if (isEditingName.value) {
      stopEditingName();
    } else {
      startEditingName();
    }
  };

  const toggleEditUsername = () => {
    if (isEditingUsername.value) {
      stopEditingUsername();
    } else {
      startEditingUsername();
    }
  };

  const toggleEditEmail = () => {
    if (isEditingEmail.value) {
      stopEditingEmail();
    } else {
      startEditingEmail();
    }
  };

  const toggleEditPassword = () => {
    if (isEditingPassword.value) {
      stopEditingPassword();
    } else {
      startEditingPassword();
    }
  };

  const isLoading = computed(
    () => getIsLoadingUsers.value || getIsLoading.value,
  );
  const me = computed(() => getMe.value);
  const isUser = computed(() => checkCurrentUser());
  const userData = computed(() => getUser.value);

  function checkCurrentUser(): boolean {
    if (me?.value) {
      return me.value.username === userData.value?.username;
    } else return false;
  }

  //Button for adding new User
  const button: FloatButtonModel = {
    name: 'CreateUserButton',
    onClick: () => {},
    icon: PlusOutlined,
    status: 'activated',
    tooltip: 'Click here to create a new user',
  };
</script>

<template>
  <div class="panel">
    <!-- avatar components -->
    <a-flex class="avatar">
      <a-avatar :size="150">
        <template #icon><UserOutlined /></template>
      </a-avatar>
      <a-flex v-if="!isLoading" class="name">
        <p v-if="!isEditingName" class="text">{{ userData?.name }}</p>
        <a-input v-else class="input" />

        <a-button @click="toggleEditName">Edit</a-button>
      </a-flex>
      <a-skeleton v-else active :paragraph="false" style="width: 10em" />
    </a-flex>

    <!-- User informations components -->
    <a-flex
      class="userInfoBox"
      :body-style="{
        height: 'fit-content',
      }"
    >
      <a-card
        :body-style="{
          display: 'flex',
          alignItems: 'center',
        }"
        class="info"
      >
        <label class="label">Username:</label>
        <template v-if="!isLoading">
          <p v-if="!isEditingUsername" class="text">{{ userData?.username }}</p>
          <a-input v-else class="input" />

          <a-button class="edit" @click="toggleEditUsername">Edit</a-button>
        </template>
        <a-skeleton
          v-else
          active
          :paragraph="false"
          style="margin-left: 1em; width: 10em"
        />
      </a-card>

      <a-card
        :body-style="{
          display: 'flex',
          alignItems: 'center',
        }"
        class="info"
      >
        <label class="label">E-Mail:</label>
        <template v-if="!isLoading">
          <p v-if="!isEditingEmail" class="text">{{ userData?.email }}</p>
          <a-input v-else class="input" />

          <a-button class="edit" @click="toggleEditEmail">Edit</a-button>
        </template>
        <a-skeleton
          v-else
          active
          :paragraph="false"
          style="margin-left: 1em; width: 10em"
        />
      </a-card>

      <a-card
        :body-style="{
          display: 'flex',
          alignItems: 'center',
        }"
        class="info"
      >
        <label class="label">Password:</label>
        <template v-if="!isLoading">
          <template v-if="isUser">
            <p v-if="!isEditingPassword" class="text">Super Secret Password</p>
            <a-input v-else class="input" type="password" />

            <a-button class="edit" @click="toggleEditPassword">Edit</a-button>
          </template>
          <p v-else class="text"></p>
        </template>
        <a-skeleton
          v-else
          active
          :paragraph="false"
          style="margin-left: 1em; width: 10em"
        />
      </a-card>
    </a-flex>
    <FloatingButton :button="button" />
  </div>
</template>

<style>
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

  .edit {
    border: none;
    margin: 0.6em 0 0.6em;
    color: blue;
    margin-left: auto;
  }

  .info {
    border: none;
    width: 100%;
    height: 4em;
    max-width: 100%;
    font-size: 1.3em;
    font-weight: bold;
    display: flex;
    flex-flow: column wrap;
    justify-content: center;
  }

  .info label {
    width: 5em;
    min-width: 5em;
    margin-right: 3em;
  }

  .input {
    max-width: 60%;
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
    left: 1em;
  }

  .name button {
    border: none;
    background: none;
    color: blue;
    margin-left: 5px;
  }

  .name input {
    font-size: 0.6em;
    margin: 1.9em 0 1.8em;
  }
</style>
