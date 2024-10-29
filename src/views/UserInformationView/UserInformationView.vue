<script lang="ts" setup>
  import { UserOutlined } from '@ant-design/icons-vue';
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';
  import { PlusOutlined } from '@ant-design/icons-vue';
  //import { EditOutlined } from '@ant-design/icons-vue';
  import { inject, onMounted, toRaw } from 'vue';
  import { userStoreSymbol } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';
  import type { UserModel } from '@/models/User';
  import type { ComputedRef } from 'vue';

  const isEditingName = false;
  const isEditingUserName = false;
  const isEditingEMail = false;
  const isEditingPass = false;

  const userStore = inject(userStoreSymbol)!;

  const { getIsLoadingUsers } = storeToRefs(userStore);
  const { getIsLoading } = storeToRefs(userStore);
  const isLoading = computed(
    () => getIsLoadingUsers.value || getIsLoading.value,
  );

  onMounted(async () => {
    const user = userStore.getUser;
    if (user) addData(user);

    const data: ComputedRef<UserModel | null> = computed(
      () => userStore.getUser,
    );

    watch(
      () => data.value,
      (newProject, oldProject) => {
        if (!newProject) return;
        if (newProject.id !== oldProject?.id) {
          addData(toRaw(newProject));
        }
      },
    );
  });

  const userData = {
    id: ref<number>(0),
    name: ref<string>(''),
    username: ref<string>(''),
    email: ref<string>(''),
  };

  function addData(loadedData: UserModel) {
    if (userStore.getUser) userData.id.value = loadedData.id;
    userData.name.value = loadedData.name;
    userData.username.value = loadedData.username;
    userData.email.value = loadedData.email;
  }

  //Button for adding new project
  const button: FloatButtonModel = {
    name: 'CreateProjectButton',
    onClick: () => {},
    icon: PlusOutlined,
    status: 'activated',
    tooltip: 'Click here to create a new user',
  };
</script>

<template>
  <!-- avatar components -->
  <div class="avatar">
    <a-avatar :size="150">
      <template #icon><UserOutlined /></template>
    </a-avatar>
  </div>

  <!-- User informations components -->
  <a-flex class="userInfo">
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
        <label class="label">Name:</label>
        <template v-if="!isLoading">
          <p v-if="!isEditingName" class="text">{{ userData.name }}</p>
          <a-input v-else class="input" />
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
        <label class="label">Username:</label>
        <template v-if="!isLoading">
          <p v-if="!isEditingUserName" class="text">{{ userData.username }}</p>
          <a-input v-else class="input" />
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
          <p v-if="!isEditingEMail" class="text">{{ userData.email }}</p>
          <a-input v-else class="input" />
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
          <p v-if="!isEditingPass" class="text"></p>
          <a-input v-else class="input" />
        </template>
        <a-skeleton
          v-else
          active
          :paragraph="false"
          style="margin-left: 1em; width: 10em"
        />
      </a-card>
    </a-flex>

    <!-- Edit button components -->
    <a-flex class="editBox">
      <a-button class="edit">Edit</a-button>
      <a-button class="edit">Edit</a-button>
      <a-button class="edit">Edit</a-button>
      <a-button class="edit">Edit</a-button>
    </a-flex>
  </a-flex>
  <FloatingButton :button="button" />
</template>

<style>
  .userInfo {
    padding: 1em;
    margin-top: 2em;
    border-radius: 10px;
    background-color: white;
    min-width: 41ch;
  }

  .userInfoBox {
    width: 100%;
    height: auto;
    flex-direction: column;
    flex-wrap: wrap;
  }

  .editBox {
    width: 6em;
    flex-direction: column;
    flex-wrap: wrap;
  }

  .edit {
    height: 4em;
    width: 4em;
    border: none;
    margin: 0.6em 0 0.6em;
    color: blue;
  }

  .info {
    border: none;
    width: 100%;
    height: 4em;
    max-width: 100%;
    font-size: 1.3em;
    font-weight: bold;
    display: flex;
    align-items: center;
    justify-content: left;
  }

  .info label {
    width: 4em;
    margin-right: 3em;
  }

  .avatar {
    display: flex;
    align-items: center;
    justify-content: center;
  }
</style>
