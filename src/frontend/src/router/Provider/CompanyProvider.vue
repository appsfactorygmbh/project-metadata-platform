<script lang="ts" setup>
  import { useCompanyStore, type CompanyStore } from '@/store';
  import { companyStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';

  provide<CompanyStore>(companyStoreSymbol, useCompanyStore());
</script>
<template>
  <slot />
</template>
