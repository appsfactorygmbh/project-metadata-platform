<script lang="ts" setup>
  import { UserOutlined } from '@ant-design/icons-vue';
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';
  import { PlusOutlined } from '@ant-design/icons-vue';

  const isLoading = false;
  const isEditingName = false;
  const isEditingUserName = false;
  const isEditingEMail = false;
  const isEditingPass = false;

  const button: FloatButtonModel = {
    name: 'CreateProjectButton',
    onClick: () => {},
    icon: PlusOutlined,
    status: 'activated',
    tooltip: 'Click here to create a new user',
  };
</script>

<template>
  <div class="avatar">
    <a-avatar :size="150">
      <template #icon><UserOutlined /></template>
    </a-avatar>
  </div>

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
          <p v-if="!isEditingName">Test</p>
          <a-input v-else />
        </template>
        <a-skeleton
          v-else
          active
          :paragraph="false"
          style="padding-left: 1em"
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
          <p v-if="!isEditingUserName">Test Username</p>
          <a-input v-else />
        </template>
        <a-skeleton
          v-else
          active
          :paragraph="false"
          style="padding-left: 1em"
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
          <p v-if="!isEditingEMail">Test</p>
          <a-input v-else />
        </template>
        <a-skeleton
          v-else
          active
          :paragraph="false"
          style="padding-left: 1em"
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
          <p v-if="!isEditingPass">Test</p>
          <a-input v-else />
        </template>
        <a-skeleton
          v-else
          active
          :paragraph="false"
          style="padding-left: 1em"
        />
      </a-card>
    </a-flex>
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
