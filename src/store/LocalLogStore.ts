import { localLogService } from '@/services/LocalLogService';
import type { LocalLogModel } from '@/models/Logs';
import { defineStore } from 'pinia';

type StoreState = {
  localLog: LocalLogModel[];
  isLoadingLocalLog: boolean;
};

export const useLocalLogStore = defineStore('localLog', {
  state: (): StoreState => {
    return {
      localLog: [],
      isLoadingLocalLog: false,
    };
  },
  getters: {
    getLocalLog(): LocalLogModel[] {
      return this.localLog;
    },
    getIsLoadingLocalLog(): boolean {
      return this.isLoadingLocalLog;
    },
  },
  actions: {
    setLocalLogs(localLog: LocalLogModel[]): void {
      this.localLog = localLog;
    },
    setIsLoadingLocalLog(isLoading: boolean): void {
      this.isLoadingLocalLog = isLoading;
    },

    async fetchLocalLog(projectId: number): Promise<void> {
      try {
        this.setIsLoadingLocalLog(true);
        const localLog = await localLogService.fetchLocalLog(projectId);
        this.setLocalLogs(localLog);
      } finally {
        this.setIsLoadingLocalLog(false);
      }
    },
  },
});

type LocalLogStore = ReturnType<typeof useLocalLogStore>;
export type { LocalLogStore };
