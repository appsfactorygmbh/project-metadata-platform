<script lang="ts" setup>
  import { useEditing } from '@/utils/hooks/useEditing';
  import { defineEmits, defineProps, ref } from 'vue';
  import type { PropType } from 'vue';

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
      type: String as PropType<'text' | 'password' | 'email'>,
      default: 'text',
    },
    isEditingKey: {
      type: String,
      required: true,
    },
  });

  const confirmPassword = ref<string>('');
  const fieldValue = ref<string>('');
  const { isEditing, startEditing, stopEditing } = useEditing(
    props.isEditingKey,
  );
  const isEdit = computed(() => isEditing.value);
  const passwordsMatch = computed(() => {
    return fieldValue.value === confirmPassword.value;
  });

  const onSave = () => {
    stopEditing();
    console.log('Success:', fieldValue.value);
    emit('update', fieldValue);
    confirmPassword.value = '';
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
      <p v-if="!isEdit" class="text">{{ value }}</p>

      <a-form v-else name="user" autocomplete="off">
        <a-form-item class="input">
          <a-input v-model:value="fieldValue" :type="type" />
        </a-form-item>
        <a-form-item v-if="type === 'password'" class="input">
          <a-input
            v-model:value="confirmPassword"
            :type="type"
            placeholder="Confirm Password"
            style="margin-top: 5px"
          />
        </a-form-item>
        <p
          v-if="type === 'password' && !passwordsMatch"
          class="error"
          style="color: red"
        >
          Passwords do not match
        </p>
      </a-form>

      <a-button v-if="!isEditing" class="edit" @click="startEditing"
        >Edit</a-button
      >
      <a-button
        v-else
        class="edit"
        html-type="submit"
        :disabled="!passwordsMatch && type === 'password'"
        @click="onSave"
        >Save</a-button
      >
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
    color: blue;
    margin-left: auto;
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
</style>
