<script lang="ts" setup>
  import { useDepartmentStore, type DepartmentStore } from '@/store';
  import { departmentStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';

  provide<DepartmentStore>(departmentStoreSymbol, useDepartmentStore());
</script>
<template>
  <slot />
</template>
