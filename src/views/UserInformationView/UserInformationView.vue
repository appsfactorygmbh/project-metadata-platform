<script lang="ts" setup>
  import { UserOutlined } from '@ant-design/icons-vue';
  const isLoading = false;
  const isEditing = false;
</script>

<template>
  <div class="avatar">
    <a-avatar :size="150">
      <template #icon><UserOutlined /></template>
    </a-avatar>
  </div>

  <a-flex
    class="userInformationBox"
    :body-style="{
      height: 'fit-content',
    }"
  >
    <a-card
      :body-style="{
        display: 'flex',
        padding: '5px',
        alignItems: 'center',
      }"
      class="infoCard"
    >
      <label class="label">Name:</label>
      <template v-if="!isLoading">
        <p v-if="!isEditing">Test</p>
        <a-input v-else />
      </template>
      <a-skeleton v-else active :paragraph="false" style="padding-left: 1em" />
    </a-card>

    <a-card
      :body-style="{
        display: 'flex',
        padding: '5px',
        alignItems: 'center',
      }"
      class="infoCard"
    >
      <label class="label">Username:</label>
      <template v-if="!isLoading">
        <p v-if="!isEditing">Test</p>
        <a-input v-else />
      </template>
      <a-skeleton v-else active :paragraph="false" style="padding-left: 1em" />
    </a-card>

    <a-card
      :body-style="{
        display: 'flex',
        padding: '5px',
        alignItems: 'center',
      }"
      class="infoCard"
    >
      <label class="label">E-Mail:</label>
      <template v-if="!isLoading">
        <p v-if="!isEditing">Test</p>
        <a-input v-else />
      </template>
      <a-skeleton v-else active :paragraph="false" style="padding-left: 1em" />
    </a-card>

    <a-card
      :body-style="{
        display: 'flex',
        padding: '5px',
        alignItems: 'center',
      }"
      class="infoCard"
    >
      <label class="label">Passwort:</label>
      <template v-if="!isLoading">
        <p v-if="!isEditing">Test</p>
        <a-input v-else />
      </template>
      <a-skeleton v-else active :paragraph="false" style="padding-left: 1em" />
    </a-card>
  </a-flex>
</template>

<style>
  .userInformationBox {
    width: 100%;
    height: auto;
    justify-content: start;
    flex-direction: column;
    flex-wrap: wrap;
    padding: 2em;
    border-radius: 10px;
    container-type: inline-size;
  }

  .infoCard {
    border: none;
    width: 100%;
    padding: 1em;
    max-width: 100%;
    font-size: 1.3em;
    font-weight: bold;
  }

  .avatar {
    display: flex;
    align-items: center;
    justify-content: center;
  }
</style>
