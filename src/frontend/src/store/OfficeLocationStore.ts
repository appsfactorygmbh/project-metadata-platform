import { OfficeLocationsApi } from '@/api/generated';
import type {
  OfficeLocationListModel,
  OfficeLocationModel,
} from '@/models/OfficeLocation/OfficeLocationModel';
import { type PiniaStore, useStore } from 'pinia-generic';
import { piniaInstance } from './piniaInstance';
import type { Pinia } from 'pinia';
import type { CreateOfficeLocationModel } from '@/models/OfficeLocation/CreateOfficeLocationModel';
import type { OfficeLocationEditModel } from '@/models/OfficeLocation';
import { type ApiStore, useApiStore } from './ApiStore';
import type { ResourceActions } from '@/models/utils';

type StoreState = {
  officeLocations: OfficeLocationModel[];
  officeLocation: OfficeLocationModel | undefined;

  isLoadingOfficeLocation: boolean;
  isLoadingOfficeLocations: boolean;
  cursor: string | null;
  isFetchingMore: boolean;
  limit: number | undefined;
};

type StoreGetters = {
  getOfficeLocations: () => OfficeLocationModel[];
  getOfficeLocation: () => OfficeLocationModel | undefined;
  getPermissions: () => ResourceActions[];
  getOfficeLocationNames: () => string[];
  getIsLoadingOfficeLocations: () => boolean;
  getIsLoadingOfficeLocation: () => boolean;
  getCursor: () => string | null;
  getIsFetchingMore: () => boolean;
};

type StoreActions = {
  refreshAuth: () => void;
  fetchAll: (reset?: boolean) => Promise<void>;
  setCursor: (cursor: string | null) => void;
  setFetchingMore: (status: boolean) => void;
  fetch: (officeLocationId: number) => Promise<void>;
  create: (officeLocationCreate: CreateOfficeLocationModel) => Promise<number>;
  update: (
    officeLocationId: OfficeLocationModel['id'],
    payload: OfficeLocationEditModel,
  ) => Promise<void>;
  delete: (officeLocationId: number) => Promise<void>;
  setLoadingOfficeLocations: (status: boolean) => void;
  setLoadingOfficeLocation: (status: boolean) => void;
  setOfficeLocations: (officeLocations: OfficeLocationModel[]) => void;
  setOfficeLocation: (officeLocation: OfficeLocationModel) => void;
  nullOfficeLocation: () => void;
  getIdToName: (name: string) => number | undefined;
  getNameToId: (id: number) => string | undefined;
};

type Store = PiniaStore<
  'officeLocation',
  StoreState,
  StoreGetters,
  StoreActions
>;

export const useOfficeLocationStore = (pinia: Pinia = piniaInstance): Store => {
  return useStore<Store, ApiStore<OfficeLocationsApi>>(
    'officeLocation',
    {
      state: {
        officeLocations: [],
        officeLocation: undefined,

        isLoadingOfficeLocations: false,
        isLoadingOfficeLocation: false,
        cursor: null,
        isFetchingMore: false,
        limit: Number(import.meta.env.VITE_LIST_PAGINATION_LIMIT) || undefined,
      },

      getters: {
        getOfficeLocations(): OfficeLocationModel[] {
          return this.officeLocations;
        },
        getOfficeLocation(): OfficeLocationModel | undefined {
          return this.officeLocation;
        },
        getIsLoadingOfficeLocations(): boolean {
          return this.isLoadingOfficeLocations;
        },
        getIsLoadingOfficeLocation(): boolean {
          return this.isLoadingOfficeLocation;
        },
        getOfficeLocationNames(): string[] {
          return this.officeLocations.map(
            (officeLocation) => officeLocation.officeLocationName,
          );
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
        getIdToName(name: string): number | undefined {
          return this.officeLocations.find(
            (officeLocation) => officeLocation.officeLocationName == name,
          )?.id;
        },
        getNameToId(id: number): string | undefined {
          return this.officeLocations.find(
            (officeLocation) => officeLocation.id == id,
          )?.officeLocationName;
        },
        setLoadingOfficeLocations(status: boolean) {
          this.isLoadingOfficeLocations = status;
        },
        setLoadingOfficeLocation(status: boolean) {
          this.isLoadingOfficeLocation = status;
        },
        setOfficeLocations(officeLocations: OfficeLocationModel[]) {
          this.officeLocations = officeLocations;
        },
        refreshAuth(): void {
          this.initApi();
        },
        setOfficeLocation(officeLocation: OfficeLocationModel) {
          this.officeLocation = officeLocation;
        },
        setCursor(cursor: string | null) {
          this.cursor = cursor;
        },
        setFetchingMore(status: boolean) {
          this.isFetchingMore = status;
        },
        async create(
          officeLocationCreate: CreateOfficeLocationModel,
        ): Promise<number> {
          try {
            this.setLoadingOfficeLocation(true);
            const res = await this.callApi('officeLocationsPut', {
              createOfficeLocationRequest: officeLocationCreate,
            });
            return res.id;
          } finally {
            this.setLoadingOfficeLocation(false);
          }
        },
        async fetch(officeLocationId: number): Promise<void> {
          try {
            this.setLoadingOfficeLocation(true);
            const officeLocationGet: OfficeLocationModel = await this.callApi(
              'officeLocationsIdGet',
              {
                id: officeLocationId,
              },
            );
            this.setOfficeLocation(officeLocationGet);
          } finally {
            this.setLoadingOfficeLocation(false);
          }
        },
        async fetchAll(reset = true): Promise<void> {
          try {
            if (reset) {
              this.setLoadingOfficeLocations(true);
              this.setCursor(null);
              this.setOfficeLocations([]);
            } else {
              this.setFetchingMore(true);
            }

            const payload: { limit: number | undefined; cursor?: string } = {
              limit: this.limit,
            };

            if (!reset && this.cursor) {
              payload.cursor = this.cursor;
            }

            const officeLocationsGet: OfficeLocationListModel =
              await this.callApi('officeLocationsGet', payload);

            if (reset) {
              this.setOfficeLocations(officeLocationsGet.resources);
            } else {
              this.setOfficeLocations([
                ...this.officeLocations,
                ...officeLocationsGet.resources,
              ]);
            }

            this.setPermissions(officeLocationsGet.permissions);
            this.setCursor(officeLocationsGet.cursor ?? null);
          } finally {
            this.setLoadingOfficeLocations(false);
            this.setFetchingMore(false);
          }
        },
        async delete(officeLocationId: number): Promise<void> {
          try {
            this.setLoadingOfficeLocation(true);
            await this.callApi('officeLocationsIdDelete', {
              id: officeLocationId,
            });
            await this.fetchAll();
          } finally {
            this.setLoadingOfficeLocation(false);
          }
        },
        async update(
          officeLocationId: OfficeLocationModel['id'],
          payload: OfficeLocationEditModel,
        ): Promise<void> {
          try {
            this.setLoadingOfficeLocation(true);
            await this.callApi('officeLocationsIdPatch', {
              id: officeLocationId,
              updateOfficeLocationRequest: payload,
            });
            this.fetchAll();
            this.fetch(officeLocationId);
          } finally {
            this.setLoadingOfficeLocation(false);
          }
        },
        nullOfficeLocation() {
          this.officeLocation = undefined;
        },
      },
    },
    useApiStore(OfficeLocationsApi, pinia),
  )(pinia);
};

type OfficeLocationStore = ReturnType<typeof useOfficeLocationStore>;
export type { OfficeLocationStore };
