import type { LogEntryModel } from '@/models/Log/LogEntryModel';
import { logsService } from '@/services/LogsService';
import { defineStore } from 'pinia';

type StoreState = {
  globalLogEntries: LogEntryModel[];
  isLoadingGlobalLogs: boolean;
  loadedGlobalLogsSuccessfully: boolean;
};

export const useLogsStore = defineStore('logs', {
  state: (): StoreState => {
    return {
      globalLogEntries: [],
      isLoadingGlobalLogs: false,
      loadedGlobalLogsSuccessfully: false,
    };
  },
  getters: {
    getGlobalLogs(): LogEntryModel[] {
      return this.globalLogEntries;
    },
    getIsLoadingGlobalLogs(): boolean {
      return this.isLoadingGlobalLogs;
    },
    loadedGlobalLogsSuccessfully(): boolean {
      return this.loadedGlobalLogsSuccessfully;
    },
  },
  actions: {
    async fetchGlobalLogs(searchParam?: string) {
      console.log(searchParam);
      try {
        this.setIsLoadingGlobalLogs(true);
        const globalLogs: LogEntryModel[] =
          await logsService.fetchGlobalLogs(searchParam);
        this.setGlobalLogs(globalLogs);
      } finally {
        this.setIsLoadingGlobalLogs(false);
      }
    },
    setGlobalLogs(globalLogEntries: LogEntryModel[]): void {
      this.globalLogEntries = globalLogEntries;
    },
    setIsLoadingGlobalLogs(isLoading: boolean): void {
      this.isLoadingGlobalLogs = isLoading;
    },
    setLoadedGlobalLogsSuccessfully(state: boolean): void {
      this.loadedGlobalLogsSuccessfully = state;
    },
  },
});

type LogsStore = ReturnType<typeof useLogsStore>;
export type { LogsStore };
