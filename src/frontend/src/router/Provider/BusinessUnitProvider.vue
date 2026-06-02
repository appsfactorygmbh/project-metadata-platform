<script lang="ts" setup>
  import { useBusinessUnitStore, type BusinessUnitStore } from '@/store';
  import { businessUnitStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';

  provide<BusinessUnitStore>(businessUnitStoreSymbol, useBusinessUnitStore());
</script>
<template>
  <slot />
</template>
