<script lang="ts" setup>
  defineProps({
    checked: {
      type: Boolean,
      default: false,
    },
    attributeName: {
      type: String,
      required: true,
    },
    label: {
      type: String,
      default: undefined,
    },
    disabled: {
      type: Boolean,
      default: false,
    },
    isLoading: {
      type: Boolean,
      default: false,
    },
  });

  const emit = defineEmits(['update:checked']);
</script>

<template>
  <a-card
    :body-style="{
      display: 'flex',
      padding: '5px',
      alignItems: 'center',
      height: 'fit-content',
      overflow: 'auto',
    }"
    class="info"
  >
    <label v-if="label != null" class="label">{{ label }}:</label>

    <template v-if="!isLoading">
      <a-form-item :name="attributeName" class="formItem">
        <a-switch
          :checked="checked"
          class="custom-color-switch"
          :disabled="disabled"
          @update:checked="(val) => emit('update:checked', val)"
        />
      </a-form-item>
    </template>

    <a-skeleton
      v-else
      active
      :paragraph="false"
      style="margin-left: 1em; width: 4em"
    />
  </a-card>
</template>

<style lang="css" scoped>
  .formItem {
    margin: 0;
  }

  :deep(.custom-color-switch.ant-switch-checked),
  :deep(.custom-color-switch.ant-switch-checked:hover) {
    background-color: #27d157;
  }

  :deep(.custom-color-switch:not(.ant-switch-checked)),
  :deep(.custom-color-switch:not(.ant-switch-checked):hover) {
    background-color: #ff002e;
  }
</style>
