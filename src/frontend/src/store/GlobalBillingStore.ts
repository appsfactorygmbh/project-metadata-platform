import { BillingApi } from '@/api/generated';
import type {
  GlobalBillingListModel,
  GlobalBillingModel,
} from '@/models/GlobalBilling/GlobalBillingModel';
import { type PiniaStore, useStore } from 'pinia-generic';
import { piniaInstance } from './piniaInstance';
import type { Pinia } from 'pinia';
import type { CreateGlobalBillingModel } from '@/models/GlobalBilling/CreateGlobalBillingModel';
import type { UpdateGlobalBillingModel } from '@/models/GlobalBilling';
import { type ApiStore, useApiStore } from './ApiStore';
import type { ResourceActions } from '@/models/utils';

type StoreState = {
  billingList: GlobalBillingModel[];
  billing: GlobalBillingModel | undefined;

  isLoadingGlobalBillingList: boolean;
  isLoadingGlobalBilling: boolean;
  cursor: string | null;
  isFetchingMore: boolean;
  limit: number | undefined;
};

type StoreGetters = {
  getGlobalBillingList: () => GlobalBillingModel[];
  getGlobalBilling: () => GlobalBillingModel | undefined;
  getPermissions: () => ResourceActions[];
  getGlobalBillingKinds: () => string[];
  getIsLoadingGlobalBillingList: () => boolean;
  getIsLoadingGlobalBilling: () => boolean;
  getCursor: () => string | null;
  getIsFetchingMore: () => boolean;
};

type StoreActions = {
  refreshAuth: () => void;
  fetchAll: (reset?: boolean) => Promise<void>;
  setCursor: (cursor: string | null) => void;
  setFetchingMore: (status: boolean) => void;
  fetch: (billingId: number) => Promise<void>;
  create: (billingCreate: CreateGlobalBillingModel) => Promise<number>;
  update: (
    billingId: GlobalBillingModel['id'],
    payload: UpdateGlobalBillingModel,
  ) => Promise<void>;
  delete: (billingId: number) => Promise<void>;
  setLoadingGlobalBillingList: (status: boolean) => void;
  setLoadingGlobalBilling: (status: boolean) => void;
  setGlobalBillingList: (billing: GlobalBillingModel[]) => void;
  setGlobalBilling: (billing: GlobalBillingModel) => void;
  nullGlobalBilling: () => void;
  getIdToKind: (name: string) => number | undefined;
  getNameToId: (id: number) => string | undefined;
};

type Store = PiniaStore<
  'globalBilling',
  StoreState,
  StoreGetters,
  StoreActions
>;

export const useGlobalBillingStore = (pinia: Pinia = piniaInstance): Store => {
  return useStore<Store, ApiStore<BillingApi>>(
    'globalBilling',
    {
      state: {
        billingList: [],
        billing: undefined,
        isLoadingGlobalBillingList: false,
        isLoadingGlobalBilling: false,
        cursor: null,
        isFetchingMore: false,
        limit: Number(import.meta.env.VITE_LIST_PAGINATION_LIMIT) || undefined,
      },

      getters: {
        getGlobalBillingList(): GlobalBillingModel[] {
          return this.billingList;
        },
        getGlobalBilling(): GlobalBillingModel | undefined {
          return this.billing;
        },
        getIsLoadingGlobalBillingList(): boolean {
          return this.isLoadingGlobalBillingList;
        },
        getIsLoadingGlobalBilling(): boolean {
          return this.isLoadingGlobalBilling;
        },
        getGlobalBillingKinds(): string[] {
          return this.billingList.map((billing) => billing.billingKind);
        },
        getPermissions(): ResourceActions[] {
          return this.permissions;
        },
        getCursor(): string | null {
          return this.cursor;
        },
        getIsFetchingMore(): boolean {
          return this.isFetchingMore;
        },
      },

      actions: {
        getIdToKind(kind: string): number | undefined {
          return this.billingList.find((billing) => billing.billingKind == kind)
            ?.id;
        },
        getNameToId(id: number): string | undefined {
          return this.billingList.find((billing) => billing.id == id)
            ?.billingKind;
        },
        setLoadingGlobalBillingList(status: boolean) {
          this.isLoadingGlobalBillingList = status;
        },
        setLoadingGlobalBilling(status: boolean) {
          this.isLoadingGlobalBilling = status;
        },
        setGlobalBillingList(billingList: GlobalBillingModel[]) {
          this.billingList = billingList;
        },
        refreshAuth(): void {
          this.initApi();
        },
        setGlobalBilling(billing: GlobalBillingModel) {
          this.billing = billing;
        },
        setCursor(cursor: string | null) {
          this.cursor = cursor;
        },
        setFetchingMore(status: boolean) {
          this.isFetchingMore = status;
        },
        async create(billingCreate: CreateGlobalBillingModel): Promise<number> {
          try {
            this.setLoadingGlobalBilling(true);
            const res = await this.callApi('billingPut', {
              createBillingRequest: billingCreate,
            });
            return res.id;
          } finally {
            this.setLoadingGlobalBilling(false);
          }
        },
        async fetch(billingId: number): Promise<void> {
          try {
            this.setLoadingGlobalBilling(true);
            const billingGet: GlobalBillingModel = await this.callApi(
              'billingBillingIdGet',
              {
                billingId: billingId,
              },
            );
            this.setGlobalBilling(billingGet);
          } finally {
            this.setLoadingGlobalBilling(false);
          }
        },
        async fetchAll(reset = true): Promise<void> {
          try {
            if (reset) {
              this.setLoadingGlobalBillingList(true);
              this.setCursor(null);
              this.setGlobalBillingList([]);
            } else {
              this.setFetchingMore(true);
            }

            const payload: { limit: number | undefined; cursor?: string } = {
              limit: this.limit,
            };

            if (!reset && this.cursor) {
              payload.cursor = this.cursor;
            }

            const globalBillingGet: GlobalBillingListModel = await this.callApi(
              'billingGet',
              payload,
            );

            if (reset) {
              this.setGlobalBillingList(globalBillingGet.resources);
            } else {
              this.setGlobalBillingList([
                ...this.billingList,
                ...globalBillingGet.resources,
              ]);
            }

            this.setPermissions(globalBillingGet.permissions);
            this.setCursor(globalBillingGet.cursor ?? null);
          } finally {
            this.setLoadingGlobalBillingList(false);
            this.setFetchingMore(false);
          }
        },
        async update(
          billingId: GlobalBillingModel['id'],
          payload: UpdateGlobalBillingModel,
        ): Promise<void> {
          try {
            this.setLoadingGlobalBilling(true);
            await this.callApi('billingBillingIdPatch', {
              billingId: billingId,
              updateBillingRequest: payload,
            });
            this.fetchAll();
            this.fetch(billingId);
          } finally {
            this.setLoadingGlobalBilling(false);
          }
        },
        nullGlobalBilling() {
          this.billing = undefined;
        },
      },
    },
    useApiStore(BillingApi, pinia),
  )(pinia);
};

type GlobalBillingStore = ReturnType<typeof useGlobalBillingStore>;
export type { GlobalBillingStore };
