import { BusinessUnitsApi } from '@/api/generated';
import type {
  BusinessUnitListModel,
  BusinessUnitModel,
} from '@/models/BusinessUnit/BusinessUnitModel';
import { type PiniaStore, useStore } from 'pinia-generic';
import { piniaInstance } from './piniaInstance';
import type { Pinia } from 'pinia';
import type { CreateBusinessUnitModel } from '@/models/BusinessUnit/CreateBusinessUnitModel';
import type { BusinessUnitEditModel } from '@/models/BusinessUnit';
import { type ApiStore, useApiStore } from './ApiStore';
import type { ResourceActions } from '@/models/utils';

type StoreState = {
  businessUnits: BusinessUnitModel[];
  businessUnit: BusinessUnitModel | undefined;
  // in sync with businessUnit
  linkedTeams: number[];
  isLoadingBusinessUnit: boolean;
  isLoadingBusinessUnits: boolean;
  cursor: string | null;
  isFetchingMore: boolean;
  limit: number | undefined;
};

type StoreGetters = {
  getBusinessUnits: () => BusinessUnitModel[];
  getBusinessUnit: () => BusinessUnitModel | undefined;
  getLinkedTeams: () => number[];
  getBusinessUnitNames: () => string[];
  getPermissions: () => ResourceActions[];
  getIsLoadingBusinessUnits: () => boolean;
  getIsLoadingBusinessUnit: () => boolean;
  getCursor: () => string | null;
  getIsFetchingMore: () => boolean;
};

type StoreActions = {
  refreshAuth: () => void;
  fetchAll: (reset?: boolean) => Promise<void>;
  setCursor: (cursor: string | null) => void;
  setFetchingMore: (status: boolean) => void;
  fetch: (businessUnitId: number) => Promise<void>;
  fetchLinkedTeams: (businessUnitId: number) => Promise<void>;
  create: (businessUnitCreate: CreateBusinessUnitModel) => Promise<number>;
  update: (
    businessUnitId: BusinessUnitModel['id'],
    payload: BusinessUnitEditModel,
  ) => Promise<void>;
  delete: (businessUnitId: number) => Promise<void>;
  setLoadingBusinessUnits: (status: boolean) => void;
  setLoadingBusinessUnit: (status: boolean) => void;
  setBusinessUnits: (businessUnits: BusinessUnitModel[]) => void;
  setBusinessUnit: (businessUnit: BusinessUnitModel) => void;
  nullBusinessUnit: () => void;
  getIdToName: (name: string) => number | undefined;
  getNameToId: (id: number) => string | undefined;
};

type Store = PiniaStore<'businessUnit', StoreState, StoreGetters, StoreActions>;

export const useBusinessUnitStore = (pinia: Pinia = piniaInstance): Store => {
  return useStore<Store, ApiStore<BusinessUnitsApi>>(
    'businessUnit',
    {
      state: {
        businessUnits: [],
        businessUnit: undefined,
        linkedTeams: [],
        isLoadingBusinessUnits: false,
        isLoadingBusinessUnit: false,
        cursor: null,
        isFetchingMore: false,
        limit: Number(import.meta.env.VITE_LIST_PAGINATION_LIMIT) || undefined,
      },

      getters: {
        getBusinessUnits(): BusinessUnitModel[] {
          return this.businessUnits;
        },
        getBusinessUnit(): BusinessUnitModel | undefined {
          return this.businessUnit;
        },
        getPermissions(): ResourceActions[] {
          return this.permissions;
        },
        getIsLoadingBusinessUnits(): boolean {
          return this.isLoadingBusinessUnits;
        },
        getIsLoadingBusinessUnit(): boolean {
          return this.isLoadingBusinessUnit;
        },
        getBusinessUnitNames(): string[] {
          return this.businessUnits.map(
            (businessUnit) => businessUnit.businessUnitName,
          );
        },
        getLinkedTeams(): number[] {
          return this.linkedTeams;
        },
        getCursor(): string | null {
          return this.cursor;
        },
        getIsFetchingMore(): boolean {
          return this.isFetchingMore;
        },
      },

      actions: {
        getIdToName(name: string): number | undefined {
          return this.businessUnits.find(
            (businessUnit) => businessUnit.businessUnitName == name,
          )?.id;
        },
        getNameToId(id: number): string | undefined {
          return this.businessUnits.find(
            (businessUnit) => businessUnit.id == id,
          )?.businessUnitName;
        },
        setLoadingBusinessUnits(status: boolean) {
          this.isLoadingBusinessUnits = status;
        },
        setLoadingBusinessUnit(status: boolean) {
          this.isLoadingBusinessUnit = status;
        },
        setBusinessUnits(businessUnits: BusinessUnitModel[]) {
          this.businessUnits = businessUnits;
        },
        refreshAuth(): void {
          this.initApi();
        },
        setBusinessUnit(businessUnit: BusinessUnitModel) {
          this.businessUnit = businessUnit;
        },
        setCursor(cursor: string | null) {
          this.cursor = cursor;
        },
        setFetchingMore(status: boolean) {
          this.isFetchingMore = status;
        },
        async create(
          businessUnitCreate: CreateBusinessUnitModel,
        ): Promise<number> {
          try {
            this.setLoadingBusinessUnit(true);
            const res = await this.callApi('businessUnitsPut', {
              createBusinessUnitRequest: businessUnitCreate,
            });
            return res.id;
          } finally {
            this.setLoadingBusinessUnit(false);
          }
        },
        async fetch(businessUnitId: number): Promise<void> {
          try {
            this.setLoadingBusinessUnit(true);
            const businessUnitGet: BusinessUnitModel = await this.callApi(
              'businessUnitsIdGet',
              {
                id: businessUnitId,
              },
            );
            await this.fetchLinkedTeams(businessUnitId);
            this.setBusinessUnit(businessUnitGet);
          } finally {
            this.setLoadingBusinessUnit(false);
          }
        },
        async fetchAll(reset = true): Promise<void> {
          try {
            if (reset) {
              this.setLoadingBusinessUnits(true);
              this.setCursor(null);
              this.setBusinessUnits([]);
            } else {
              this.setFetchingMore(true);
            }

            const payload: { limit: number | undefined; cursor?: string } = {
              limit: this.limit,
            };

            if (!reset && this.cursor) {
              payload.cursor = this.cursor;
            }

            const businessUnitsGet: BusinessUnitListModel = await this.callApi(
              'businessUnitsGet',
              payload,
            );

            if (reset) {
              this.setBusinessUnits(businessUnitsGet.resources);
            } else {
              this.setBusinessUnits([
                ...this.businessUnits,
                ...businessUnitsGet.resources,
              ]);
            }

            this.setPermissions(businessUnitsGet.permissions);
            this.setCursor(businessUnitsGet.cursor ?? null);
          } finally {
            this.setLoadingBusinessUnits(false);
            this.setFetchingMore(false);
          }
        },
        async delete(businessUnitId: number): Promise<void> {
          try {
            this.setLoadingBusinessUnit(true);
            await this.callApi('businessUnitsIdDelete', { id: businessUnitId });
            await this.fetchAll();
          } finally {
            this.setLoadingBusinessUnit(false);
          }
        },
        async update(
          businessUnitId: BusinessUnitModel['id'],
          payload: BusinessUnitEditModel,
        ): Promise<void> {
          try {
            this.setLoadingBusinessUnit(true);
            await this.callApi('businessUnitsIdPatch', {
              id: businessUnitId,
              updateBusinessUnitRequest: payload,
            });
            this.fetchAll();
            this.fetch(businessUnitId);
          } finally {
            this.setLoadingBusinessUnit(false);
          }
        },
        async fetchLinkedTeams(businessUnitId: number): Promise<void> {
          try {
            this.linkedTeams = (
              await this.callApi('businessUnitsIdLinkedTeamsGet', {
                id: businessUnitId,
              })
            ).teamIds;
          } catch (e) {
            console.error('failed retrieving linked teams for business unit');
            throw e;
          }
        },
        nullBusinessUnit() {
          this.businessUnit = undefined;
        },
      },
    },
    useApiStore(BusinessUnitsApi, pinia),
  )(pinia);
};

type BusinessUnitStore = ReturnType<typeof useBusinessUnitStore>;
export type { BusinessUnitStore };
