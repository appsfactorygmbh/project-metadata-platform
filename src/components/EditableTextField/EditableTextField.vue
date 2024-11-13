<script lang="ts" setup>
  import { userStoreSymbol } from '@/store/injectionSymbols';
  import { useEditing } from '@/utils/hooks/useEditing';
  import {
    CheckOutlined,
    CloseOutlined,
    EditOutlined,
  } from '@ant-design/icons-vue';
  import { defineEmits, defineProps, inject, ref } from 'vue';
  import type { PropType } from 'vue';

  const userStore = inject(userStoreSymbol)!;

  const emit = defineEmits(['update']);

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
  });

  const fieldValue = ref('');

  watch(
    () => props.value,
    (newValue) => {
      fieldValue.value = newValue;
    },
  );

  const { isEditing, startEditing, stopEditing } = useEditing(
    props.isEditingKey,
  );

  const saveEdits = () => {
    stopEditing();
    emit('update', fieldValue.value);
  };

  const checkCorrectInput = () => {
    console.log('type:_ ', props.type);
    const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;

    if (props.type === 'username') {
      const alreadyExists = userStore.getUsers.some(
        (user) => user.username === fieldValue.value,
      );
      console.log();
      console.log('already exists: ', alreadyExists);
      inputState.value =
        fieldValue.value && !alreadyExists ? undefined : 'error';
      console.log(inputState.value);
    }

    if (props.type === 'email') {
      inputState.value = emailRegex.test(fieldValue.value)
        ? undefined
        : 'error';
    }
  };
  const inputState = ref<undefined | 'error'>(undefined);
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

      <a-form v-else name="user" autocomplete="off">
        <a-form-item class="input">
          <a-input
            v-model:value="fieldValue"
            :placeholder="props.value"
            :status="inputState"
            @change="checkCorrectInput"
          />
        </a-form-item>
      </a-form>

      <a-button v-if="!isEditing" class="edit" @click="startEditing">
        <EditOutlined class="icon" />
      </a-button>
      <div v-else class="buttonGroup">
        <a-button
          class="edit check"
          :disabled="inputState === 'error' || inputState === '' ? true : false"
          @click="saveEdits"
        >
          <CheckOutlined class="icon" />
        </a-button>
        <a-button class="edit abort" @click="stopEditing">
          <CloseOutlined class="icon" />
        </a-button>
      </div>
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
  .edit {
    border: none;
    margin: 0.6em 0 0.6em;
    margin-left: auto;
    background-color: rgba(0, 0, 0, 0.88);
    width: 2em;
    height: 2em;
    display: flex;
    justify-content: center;
    align-items: center;
  }

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

  .icon {
    color: white;
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
  }

  .ant-card-body {
    padding: 12px !important;
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
