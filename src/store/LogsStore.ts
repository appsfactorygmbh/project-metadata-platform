import type { LogEntryModel } from '@/models/Log/LogEntryModel';
import { type Pinia } from 'pinia';
import { type PiniaStore, useStore } from 'pinia-generic';
import { piniaInstance } from './piniaInstance';
import { type ApiStore, useApiStore } from './ApiStore';
import { LogsApi } from '@/api/generated';

type StoreState = {
  globalLogEntries: LogEntryModel[];
  isLoadingGlobalLogs: boolean;
  loadedGlobalLogsSuccessfully: boolean;
};

type StoreActions = {
  refreshAuth: () => void;
  fetchGlobalLogs: (searchParam?: string) => Promise<LogEntryModel[]>;
  setGlobalLogs: (globalLogEntries: LogEntryModel[]) => void;
  setIsLoadingGlobalLogs: (isLoading: boolean) => void;
  setLoadedGlobalLogsSuccessfully: (state: boolean) => void;
};

type StoreGetters = {
  getGlobalLogs: () => LogEntryModel[];
  getIsLoadingGlobalLogs: () => boolean;
  getLoadedGlobalLogsSuccessfully: () => boolean;
  loadedGlobalLogsSuccessfully: () => boolean;
};

type Store = PiniaStore<'logs', StoreState, StoreGetters, StoreActions>;

export const useLogsStore = (pinia: Pinia = piniaInstance): Store => {
  return useStore<Store, ApiStore<LogsApi>>(
    'logs',
    {
      state: {
        globalLogEntries: [],
        isLoadingGlobalLogs: false,
        loadedGlobalLogsSuccessfully: false,
      },
      getters: {
        getGlobalLogs(): LogEntryModel[] {
          return this.globalLogEntries.reverse();
        },
        getIsLoadingGlobalLogs(): boolean {
          return this.isLoadingGlobalLogs;
        },
        loadedGlobalLogsSuccessfully(): boolean {
          return this.loadedGlobalLogsSuccessfully;
        },
      },
      actions: {
        refreshAuth(): void {
          this.initApi();
        },

        async fetchGlobalLogs(searchParam?: string) {
          this.setIsLoadingGlobalLogs(true);
          this.setLoadedGlobalLogsSuccessfully(false);
          return await this.callApi('logsGet', {
            search: searchParam,
          })
            .then((logs: LogEntryModel[]) => {
              this.setGlobalLogs(logs);
              this.setLoadedGlobalLogsSuccessfully(true);
              return logs;
            })
            .catch((e) => {
              this.setLoadedGlobalLogsSuccessfully(false);
              return [];
            })
            .finally(() => {
              this.setIsLoadingGlobalLogs(false);
            });
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
    },
    useApiStore(LogsApi, pinia),
  )(pinia);
};

type LogsStore = ReturnType<typeof useLogsStore>;
export type { LogsStore };
