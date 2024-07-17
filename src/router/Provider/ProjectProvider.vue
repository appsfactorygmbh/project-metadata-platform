<script lang="ts" setup>
  import { projectsService } from '@/services';
  import { useProjectStore } from '@/store';
  import { projectsStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';
  import { useAuth } from 'vue-auth3';

  const projectsStore = useProjectStore();
  provide<typeof projectsStore>(projectsStoreSymbol, projectsStore);

  const auth = useAuth();
  watch(
    () => auth.currentToken,
    () => projectsService.setAuth(auth.currentToken),
  );
</script>
<template>
  <slot></slot>
</template>
