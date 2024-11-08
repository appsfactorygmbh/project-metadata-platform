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
  });

  const item = ref<string>(props.label);
  const fieldValue = ref<string>(props.value);
  const { isEditing, startEditing, stopEditing } = useEditing(item.value);

  const toggleEdit = () => {
    startEditing();
  };

  const onSave = () => {
    stopEditing();
    console.log('Success:', fieldValue.value);
    emit('update', fieldValue);
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

      <a-form v-else name="user" autocomplete="off">
        <a-form-item class="input">
          <a-input v-model:value="fieldValue" :type="type" />
        </a-form-item>
      </a-form>

      <a-button v-if="!isEditing" class="edit" @click="toggleEdit"
        >Edit</a-button
      >
      <a-button v-else class="edit" html-type="submit" @click="onSave"
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
    margin: 0;
  }
</style>
