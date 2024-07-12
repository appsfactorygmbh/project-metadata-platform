<script lang="ts" setup>
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';
  import { UndoOutlined } from '@ant-design/icons-vue';

  const button: FloatButtonModel = {
    name: 'ResetFilterButton',
    icon: UndoOutlined,
    status: 'activated',
    tooltip: 'Click here to reset to search filter',
  };
</script>

<template>
  <FloatingButton :button="button" class="reset" />
</template>

<style scoped>
  .reset {
    position: absolute;
    top: 15px;
    right: 15px;
  }
</style>
