<script lang="ts" setup>
  import { UserOutlined } from '@ant-design/icons-vue';
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';
  import { PlusOutlined } from '@ant-design/icons-vue';
  import { inject, ref } from 'vue';
  import { userStoreSymbol } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';
  import { useEditing } from '@/utils/hooks/useEditing';

  const userStore = inject(userStoreSymbol)!;
  const { getIsLoadingUsers, getIsLoading, getMe } = storeToRefs(userStore);
  const { isEditing, startEditing, stopEditing } = useEditing('Name');
  const me = computed(() => getMe.value);
  const isLoading = computed(
    () => getIsLoadingUsers.value || getIsLoading.value,
  );
  const fieldValue = ref<string>('');

  //Button for adding new User
  const button: FloatButtonModel = {
    name: 'CreateUserButton',
    onClick: () => {},
    icon: PlusOutlined,
    status: 'activated',
    tooltip: 'Click here to create a new user',
  };

  const toggleEdit = () => {
    startEditing();
  };

  const onSave = () => {
    stopEditing();
    console.log('Success:', fieldValue.value);
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
        <p v-if="!isEditing" class="text">{{ me?.name ?? '' }}</p>

        <a-form v-else name="user" autocomplete="off">
          <a-form-item class="input">
            <a-input v-model:value="fieldValue" type="text" />
          </a-form-item>
        </a-form>

        <a-button v-if="!isEditing" class="edit" @click="toggleEdit"
          >Edit</a-button
        >
        <a-button v-else class="edit" html-type="submit" @click="onSave"
          >Save</a-button
        >
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
      <EditableTextField
        :value="me?.username ?? ''"
        :is-loading="isLoading"
        :label="'Username'"
      />
      <EditableTextField
        :value="me?.email ?? ''"
        :is-loading="isLoading"
        :label="'Email'"
        type="email"
      />
      <EditableTextField
        :value="''"
        :is-loading="isLoading"
        :label="'Password'"
        type="password"
      />
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

  .text {
    margin: 0.5em 0 0.5em;
  }

  .input {
    font-size: 0.6em;
    margin: 2.2em 0 2.3em;
  }
</style>
