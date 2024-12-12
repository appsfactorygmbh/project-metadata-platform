<script lang="ts" setup>
  import { useEditing } from '@/utils/hooks/useEditing';
  import type { PropType } from 'vue';
  import { useFormStore } from '../Form/FormStore';
  import UserEmailInputField from './InputFields/UserEmailInputField.vue';

  const props = defineProps({
    value: {
      type: String,
      required: true,
    },
    label: {
      type: String,
      required: true,
    },
    isLoading: {
      type: Boolean,
      required: true,
    },
    type: {
      type: String as PropType<'username' | 'email'>,
      default: 'text',
    },
    isEditingKey: {
      type: String,
      required: true,
    },
    userId: {
      type: String,
      required: true,
    },
  });

  const emailFormStore = useFormStore('editEmailForm');
  const usernameFormStore = useFormStore('editUsernameForm');

  const emit = defineEmits(['safedChanges']);

  const { isEditing, startEditing, stopEditing } = useEditing(
    props.isEditingKey,
  );

  const safeEdit = async () => {
    if (props.type === 'email') {
      await emailFormStore.submit();
      emailFormStore.resetFields();
    }
    if (props.type === 'username') {
      await usernameFormStore.submit();
      usernameFormStore.resetFields();
    }
    stopEditing();
    emit('safedChanges');
  };
</script>

<template>
  <a-card
    :body-style="{
      display: 'flex',
      alignItems: 'center',
    }"
    class="info"
  >
    <label class="label">{{ label }}:</label>
    <template v-if="!isLoading">
      <p v-if="!isEditing" class="text">{{ value }}</p>

      <div v-else>
        <UserEmailInputField
          v-if="props.type === 'email'"
          :form-store="emailFormStore"
          :user-id="props.userId"
          :placeholder="props.value"
          :default="props.value"
        />
      </div>

      <EditButtons
        :is-editing="isEditing"
        :is-loading="isLoading"
        :safe-disabled="isLoading"
        class="editButton"
        @start-editing="startEditing"
        @cancle-edit="stopEditing"
        @safe-edits="safeEdit"
      />
    </template>
    <a-skeleton
      v-else
      active
      :paragraph="false"
      style="margin-left: 1em; width: 10em"
    />
  </a-card>
</template>

<style>
  .password {
    display: flex;
    margin-bottom: 10px;
  }

  .check {
    background-color: #24c223;
  }

  .abort {
    background-color: #ff002e;
  }

  .buttonGroup {
    display: flex;
    flex-direction: row;
    margin-left: auto;
    margin-top: 0;
    margin-bottom: 0;
    gap: 10px;
  }

  .info {
    border: none;
    width: 100%;
    height: auto;
    max-width: 100%;
    font-size: 1.3em;
    font-weight: bold;
    display: flex;
    flex-flow: column wrap;
    justify-content: center;
    .ant-card-body {
      padding: 12px !important;
    }
  }

  .info label {
    width: 5em;
    min-width: 5em;
    margin-right: 3em;
  }

  .input {
    margin: 0 !important;
  }
  .text {
    font-weight: 400;
  }
</style>
