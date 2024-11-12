<script lang="ts" setup>
  import { UserOutlined } from '@ant-design/icons-vue';
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';
  import { PlusOutlined } from '@ant-design/icons-vue';
  import { inject, ref } from 'vue';
  import { userStoreSymbol } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';
  import { useEditing } from '@/utils/hooks/useEditing';
  import { useRouter } from 'vue-router';

  const router = useRouter();
  const userStore = inject(userStoreSymbol)!;
  const { getIsLoadingUsers, getIsLoading, getUser } = storeToRefs(userStore);
  const { isEditing, startEditing, stopEditing } = useEditing('isEditingName');
  const user = computed(() => getUser.value);
  const isLoading = computed(
    () => getIsLoadingUsers.value || getIsLoading.value,
  );
  const fieldValue = ref<string>('');

  //Button for adding new User
  const button: FloatButtonModel = {
    name: 'CreateUserButton',
    onClick: () => {
      router.push('/settings/user-management/create');
    },
    icon: PlusOutlined,
    status: 'activated',
    tooltip: 'Click here to create a new user',
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
        <p v-if="!isEditing" class="text">{{ user?.name ?? '' }}</p>

        <a-form v-else name="user" autocomplete="off">
          <a-form-item class="inputName">
            <a-input v-model:value="fieldValue" type="text" />
          </a-form-item>
        </a-form>

        <a-button v-if="!isEditing" class="edit" @click="startEditing"
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
        :value="user?.username ?? ''"
        :is-loading="isLoading"
        :label="'Username'"
        :is-editing-key="'isEditingUsername'"
      />
      <EditableTextField
        :value="user?.email ?? ''"
        :is-loading="isLoading"
        :label="'Email'"
        :is-editing-key="'isEditingEmail'"
        type="email"
      />
      <EditableTextField
        :value="'**********'"
        :is-loading="isLoading"
        :label="'Password'"
        :is-editing-key="'isEditingPassword'"
        type="password"
      />
    </a-flex>
  </div>
  <RouterView />
  <FloatingButton :button="button" />
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

  .inputName {
    font-size: 0.6em;
    margin: 2.2em 0 2.3em;
  }
</style>
