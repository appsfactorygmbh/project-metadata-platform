<script lang="ts" setup>
  import { inject, computed, onMounted, ref, watch } from 'vue';
  import { teamRoutingSymbol, teamStoreSymbol } from '@/store/injectionSymbols';
  import { storeToRefs } from 'pinia';
  import { useThemeToken } from '@/utils/hooks';

  const token = useThemeToken();

  const collapsed = ref<boolean>(false);
  const selectedKeys = ref<string[]>([]);
  const teamStore = inject(teamStoreSymbol)!;

  const { routerTeamId, setTeamId } = inject(teamRoutingSymbol)!;
  const { getTeams, getIsLoadingTeams } = storeToRefs(teamStore);

  const isLoading = computed(() => getIsLoadingTeams.value);
  const teamData = computed(() => getTeams.value);

  const selectedTeamId = ref<string>('');
  watch(
    () => routerTeamId.value,
    async () => {
      if (routerTeamId.value == '') {
        if (selectedTeamId.value != '') {
          console.log('write ');
          setTeamId(selectedTeamId.value);
        }
      }
      await teamStore?.fetch(Number(routerTeamId.value));
      selectedKeys.value = [routerTeamId.value];
    },
  );

  // used for scrolling to the selected team on mount
  const siderRef = ref<HTMLElement | any>(null);

  const scrollToSelectedMenuItem = async () => {
    await nextTick();
    if (siderRef.value && selectedKeys.value && selectedKeys.value.length > 0) {
      const siderElement = siderRef.value.$el || siderRef.value;

      const selectedItemElement = siderElement.querySelector(
        '.ant-menu-item-selected',
      ) as HTMLElement;

      if (selectedItemElement) {
        selectedItemElement.scrollIntoView({
          behavior: 'smooth',
          block: 'nearest',
        });
      }
    }
  };

  const clickTab = async (teamId: string) => {
    selectedTeamId.value = teamId;
    setTeamId(teamId);
  };

  onMounted(async () => {
    if (teamStore.getTeam?.id != undefined) {
      setTeamId(String(teamStore.getTeam?.id));
    }
    await teamStore?.fetchAll();
    if (routerTeamId.value) {
      await teamStore?.fetch(Number(routerTeamId.value));
      selectedKeys.value = [routerTeamId.value];
      scrollToSelectedMenuItem();
    }
  });
</script>

<template>
  <a-layout class="layout">
    <a-layout-sider
      ref="siderRef"
      v-model:collapsed="collapsed"
      class="sideSlider"
      collapsible
      :width="250"
    >
      <a-menu
        v-if="!isLoading"
        v-model:selected-keys="selectedKeys"
        mode="inline"
        class="menuItem"
      >
        <a-menu-item
          v-for="team in teamData"
          :key="String(team.id)"
          @click="clickTab(String(team.id))"
        >
          <span>{{ team.teamName }}</span>
        </a-menu-item>
      </a-menu>
      <a-skeleton
        v-else
        active
        :paragraph="false"
        style="margin-left: 1em; width: 15em"
      />
    </a-layout-sider>
    <a-layout-content>
      <!-- renders the TeamInformationView -->
      <RouterView v-slot="{ Component }">
        <component :is="Component" @team-deleted="selectedTeamId = ''" />
      </RouterView>
    </a-layout-content>
  </a-layout>
</template>

<style scoped>
  .layout {
    height: 100vh;
  }

  .ant-layout-sider {
    background-color: v-bind('token.colorBgElevated');
    height: 90vh;
    overflow: auto;
    border-radius: 10px;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  }

  .content {
    padding: 10px;
    min-height: calc(100vh - 20px);
  }

  span {
    font-size: 1em;
  }

  .ant-layout-content {
    margin: 0 16px;
  }

  :deep(.ant-layout-sider-trigger) {
    background-color: v-bind('token.colorBgElevated');
    color: white !important;
    height: 0;
  }
  .menuItem {
    background-color: v-bind('token.colorBgElevated');
  }
</style>
