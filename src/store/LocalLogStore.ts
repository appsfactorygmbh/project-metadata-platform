import { logsService } from '@/services/LogsService';
import type { LogEntryModel } from '@/models/Log';
import { defineStore } from 'pinia';

type StoreState = {
  localLog: LogEntryModel[];
  isLoadingLocalLog: boolean;
};

export const useLocalLogStore = defineStore('localLogs', {
  state: (): StoreState => {
    return {
      localLog: [],
      isLoadingLocalLog: false,
    };
  },
  getters: {
    getLocalLog(): LogEntryModel[] {
      return this.localLog;
    },
    getIsLoadingLocalLog(): boolean {
      return this.isLoadingLocalLog;
    },
  },
  actions: {
    setLocalLogs(localLog: LogEntryModel[]): void {
      this.localLog = localLog;
    },
    setIsLoadingLocalLog(isLoading: boolean): void {
      this.isLoadingLocalLog = isLoading;
    },

    async fetchLocalLog(projectId: number): Promise<void> {
      try {
        this.setIsLoadingLocalLog(true);
        const localLog: LogEntryModel[] =
          await logsService.fetchLocalLog(projectId);
        this.setLocalLogs(localLog);
      } finally {
        this.setIsLoadingLocalLog(false);
      }
    },
  },
});

type LocalLogStore = ReturnType<typeof useLocalLogStore>;
export type { LocalLogStore };
