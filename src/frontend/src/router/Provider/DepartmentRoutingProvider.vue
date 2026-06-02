<script setup lang="ts">
  import { departmentRoutingSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';
  import { useDepartmentRouting } from '@/utils/hooks/useDepartmentRouting';

  const departmentRouting = useDepartmentRouting();
  provide<typeof departmentRouting>(departmentRoutingSymbol, departmentRouting);
</script>

<template>
  <slot />
</template>
