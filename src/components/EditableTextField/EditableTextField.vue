<script lang="ts" setup>
  import type { PropType } from 'vue';
  import type { FormStore } from '../Form/FormStore';
  import { useEditing } from '@/utils/hooks';

  const props = defineProps({
    value: {
      type: [Number, String],
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
    isEditingKey: {
      type: String,
      required: false,
    },
    formStore: {
      type: Object as PropType<FormStore>,
      required: false,
    },
    hasEditKeys: {
      type: Boolean,
      required: true,
    },
  });

  const emit = defineEmits(['savedChanges']);

  const { isEditing } = useEditing(props.isEditingKey);
</script>

<template>
  <a-card
    :body-style="{
      display: 'flex',
      padding: '5px',
      alignItems: 'center',
      height: '2em',
    }"
    class="info"
  >
    <label class="label">{{ label }}:</label>
    <template v-if="!isLoading">
      <p v-if="!isEditing" class="text">{{ value }}</p>

      <div v-else>
        <slot></slot>
      </div>

      <EditButtons
        v-if="hasEditKeys"
        class="editButton"
        :isEditingKey="props.isEditingKey ?? ''"
        :is-loading="props.isLoading"
        :safe-disabled="props.isLoading"
        :formStore="props.formStore!"
        @saved-changes="emit('savedChanges')"
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
  }

  .info label {
    width: 5em;
    min-width: 5em;
    margin-right: 3em;
  }

  .input {
    margin-left: 1em;
    max-width: 100%;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
    flex: 1 !important;
    padding-left: 1em !important;
  }

  .text {
    font-weight: 400;
  }
</style>
