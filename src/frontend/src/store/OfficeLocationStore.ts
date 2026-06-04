import { OfficeLocationsApi } from '@/api/generated';
import type { OfficeLocationModel } from '@/models/OfficeLocation/OfficeLocationModel';
import { type PiniaStore, useStore } from 'pinia-generic';
import { piniaInstance } from './piniaInstance';
import type { Pinia } from 'pinia';
import type { CreateOfficeLocationModel } from '@/models/OfficeLocation/CreateOfficeLocationModel';
import type { OfficeLocationEditModel } from '@/models/OfficeLocation';
import { type ApiStore, useApiStore } from './ApiStore';

type StoreState = {
  officeLocations: OfficeLocationModel[];
  officeLocation: OfficeLocationModel | undefined;

  isLoadingOfficeLocation: boolean;
  isLoadingOfficeLocations: boolean;
};

type StoreGetters = {
  getOfficeLocations: () => OfficeLocationModel[];
  getOfficeLocation: () => OfficeLocationModel | undefined;

  getOfficeLocationNames: () => string[];
  getIsLoadingOfficeLocations: () => boolean;
  getIsLoadingOfficeLocation: () => boolean;
};

type StoreActions = {
  refreshAuth: () => void;
  fetchAll: () => Promise<void>;
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
        async fetchAll(): Promise<void> {
          try {
            this.setLoadingOfficeLocations(true);
            const officeLocationsGet: OfficeLocationModel[] =
              await this.callApi('officeLocationsGet', undefined);
            this.setOfficeLocations(officeLocationsGet);
          } finally {
            this.setLoadingOfficeLocations(false);
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
