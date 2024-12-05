import type { LogEntryModel } from '@/models/Log';
import { type Pinia } from 'pinia';
import { type PiniaStore, useStore } from 'pinia-generic';
import { piniaInstance } from './piniaInstance';
import { type ApiStore, useApiStore } from './ApiStore';
import { LogsApi } from '@/api/generated';

type StoreState = {
  localLog: LogEntryModel[];
  isLoadingLocalLog: boolean;
};

type StoreActions = {
  refreshAuth: () => void;
  setLocalLogs: (localLog: LogEntryModel[]) => void;
  setIsLoadingLocalLog: (isLoading: boolean) => void;
  fetchLocalLog: (projectId: number) => Promise<void>;
};

type StoreGetters = {
  getLocalLog: () => LogEntryModel[];
  getIsLoadingLocalLog: () => boolean;
};

type Store = PiniaStore<'localLogs', StoreState, StoreGetters, StoreActions>;

export const useLocalLogStore = (pinia: Pinia = piniaInstance): Store => {
  return useStore<Store, ApiStore<LogsApi>>(
    'localLogs',
    {
      state: {
        localLog: [],
        isLoadingLocalLog: false,
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
        refreshAuth(): void {
          this.initApi();
        },

        setLocalLogs(localLog: LogEntryModel[]): void {
          this.localLog = localLog;
        },
        setIsLoadingLocalLog(isLoading: boolean): void {
          this.isLoadingLocalLog = isLoading;
        },

        async fetchLocalLog(projectId: number): Promise<void> {
          this.setIsLoadingLocalLog(true);
          await this.callApi('logsGet', {
            projectId,
          })
            .then((logs: LogEntryModel[]) => {
              this.setLocalLogs(logs);
            })
            .catch(() => {
              this.setLocalLogs([]);
            })
            .finally(() => {
              this.setIsLoadingLocalLog(false);
            });
        },
      },
    },
    useApiStore(LogsApi, pinia),
  )(pinia);
};

type LocalLogStore = ReturnType<typeof useLocalLogStore>;
export type { LocalLogStore };
