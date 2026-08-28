import { PluginBillingApi } from '@/api/generated';
import type { BillingModel } from '@/models/Billing/BillingModel';
import { type PiniaStore, useStore } from 'pinia-generic';
import { piniaInstance } from './piniaInstance';
import type { Pinia } from 'pinia';
import type { CreateBillingModel } from '@/models/Billing/CreateBillingModel';
import type { UpdateBillingModel } from '@/models/Billing';
import { type ApiStore, useApiStore } from './ApiStore';

type StoreState = {
  billing: BillingModel | undefined;
  isLoadingBillingList: boolean;
  isLoadingBilling: boolean;
};

type StoreGetters = {
  getBilling: () => BillingModel | undefined;
  getBillingKinds: () => string[];
  getIsLoadingBilling: () => boolean;
};

type StoreActions = {
  refreshAuth: () => void;
  fetch: (projectId: number, pluginId: number) => Promise<void>;
  add: (
    projectId: number,
    pluginId: number,
    billingCreate: CreateBillingModel,
  ) => Promise<[number, number]>;
  update: (
    projectId: number,
    pluginId: number,
    payload: UpdateBillingModel,
  ) => Promise<BillingModel>;
  remove: (projectId: number, pluginId: number) => Promise<void>;
  setLoadingBilling: (status: boolean) => void;
  setBilling: (billing: BillingModel) => void;
  nullBilling: () => void;
};

type Store = PiniaStore<'billing', StoreState, StoreGetters, StoreActions>;

export const useBillingStore = (pinia: Pinia = piniaInstance): Store => {
  return useStore<Store, ApiStore<PluginBillingApi>>(
    'billing',
    {
      state: {
        billing: undefined,
        isLoadingBillingList: false,
        isLoadingBilling: false,
      },

      getters: {
        getBilling(): BillingModel | undefined {
          return this.billing;
        },
        getIsLoadingBilling(): boolean {
          return this.isLoadingBilling;
        },
      },

      actions: {
        setLoadingBilling(status: boolean) {
          this.isLoadingBilling = status;
        },

        refreshAuth(): void {
          this.initApi();
        },
        setBilling(billing: BillingModel) {
          this.billing = billing;
        },
        async fetch(projectId, pluginId) {
          try {
            this.setLoadingBilling(true);
            const billing: BillingModel = await this.callApi(
              'projectsProjectIdPluginsPluginIdBillingGet',
              {
                projectId: projectId,
                pluginId: pluginId,
              },
            );
            this.setBilling(billing);
          } finally {
            this.setLoadingBilling(false);
          }
        },
        async add(projectId, pluginId, billingCreate) {
          try {
            this.setLoadingBilling(true);
            const response = await this.callApi(
              'projectsProjectIdPluginsPluginIdBillingPut',
              {
                projectId: projectId,
                pluginId: pluginId,
                addPluginBillingRequest: billingCreate,
              },
            );
            return [response.projectId, response.pluginId];
          } finally {
            this.setLoadingBilling(false);
          }
        },
        async update(projectId, pluginId, payload) {
          try {
            this.setLoadingBilling(true);
            const billing = await this.callApi(
              'projectsProjectIdPluginsPluginIdBillingPatch',
              {
                projectId: projectId,
                pluginId: pluginId,
                updatePluginBillingRequest: payload,
              },
            );
            this.setBilling(billing);
            return billing;
          } finally {
            this.setLoadingBilling(false);
          }
        },
        async remove(projectId, pluginId) {
          try {
            this.setLoadingBilling(true);
            await this.callApi(
              'projectsProjectIdPluginsPluginIdBillingDelete',
              {
                projectId: projectId,
                pluginId: pluginId,
              },
            );
          } finally {
            this.setLoadingBilling(false);
          }
        },
        nullBilling() {
          this.billing = undefined;
        },
      },
    },
    useApiStore(PluginBillingApi, pinia),
  )(pinia);
};

type BillingStore = ReturnType<typeof useBillingStore>;
export type { BillingStore };
