<script lang="ts" setup>
  import { useOfficeLocationStore, type OfficeLocationStore } from '@/store';
  import { officeLocationStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';

  provide<OfficeLocationStore>(officeLocationStoreSymbol, useOfficeLocationStore());
</script>
<template>
  <slot />
</template>
