import { DepartmentsApi } from '@/api/generated';
import type {
  DepartmentListModel,
  DepartmentModel,
} from '@/models/Department/DepartmentModel';
import { type PiniaStore, useStore } from 'pinia-generic';
import { piniaInstance } from './piniaInstance';
import type { Pinia } from 'pinia';
import type { CreateDepartmentModel } from '@/models/Department/CreateDepartmentModel';
import type { DepartmentEditModel } from '@/models/Department';
import { type ApiStore, useApiStore } from './ApiStore';
import type { ResourceActions } from '@/models/utils';

type StoreState = {
  departments: DepartmentModel[];
  department: DepartmentModel | undefined;

  isLoadingDepartment: boolean;
  isLoadingDepartments: boolean;
  cursor: string | null;
  isFetchingMore: boolean;
  limit: number | undefined;
};

type StoreGetters = {
  getDepartments: () => DepartmentModel[];
  getDepartment: () => DepartmentModel | undefined;
  getPermissions: () => ResourceActions[];
  getDepartmentNames: () => string[];
  getIsLoadingDepartments: () => boolean;
  getIsLoadingDepartment: () => boolean;
  getCursor: () => string | null;
  getIsFetchingMore: () => boolean;
};

type StoreActions = {
  refreshAuth: () => void;
  fetchAll: (reset?: boolean) => Promise<void>;
  setCursor: (cursor: string | null) => void;
  setFetchingMore: (status: boolean) => void;
  fetch: (departmentId: number) => Promise<void>;
  create: (departmentCreate: CreateDepartmentModel) => Promise<number>;
  update: (
    departmentId: DepartmentModel['id'],
    payload: DepartmentEditModel,
  ) => Promise<void>;
  delete: (departmentId: number) => Promise<void>;
  setLoadingDepartments: (status: boolean) => void;
  setLoadingDepartment: (status: boolean) => void;
  setDepartments: (departments: DepartmentModel[]) => void;
  setDepartment: (department: DepartmentModel) => void;
  nullDepartment: () => void;
  getIdToName: (name: string) => number | undefined;
  getNameToId: (id: number) => string | undefined;
};

type Store = PiniaStore<'department', StoreState, StoreGetters, StoreActions>;

export const useDepartmentStore = (pinia: Pinia = piniaInstance): Store => {
  return useStore<Store, ApiStore<DepartmentsApi>>(
    'department',
    {
      state: {
        departments: [],
        department: undefined,

        isLoadingDepartments: false,
        isLoadingDepartment: false,
        cursor: null,
        isFetchingMore: false,
        limit: Number(import.meta.env.VITE_LIST_PAGINATION_LIMIT) || undefined,
      },

      getters: {
        getDepartments(): DepartmentModel[] {
          return this.departments;
        },
        getDepartment(): DepartmentModel | undefined {
          return this.department;
        },
        getIsLoadingDepartments(): boolean {
          return this.isLoadingDepartments;
        },
        getIsLoadingDepartment(): boolean {
          return this.isLoadingDepartment;
        },
        getDepartmentNames(): string[] {
          return this.departments.map(
            (department) => department.departmentName,
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
          return this.departments.find(
            (department) => department.departmentName == name,
          )?.id;
        },
        getNameToId(id: number): string | undefined {
          return this.departments.find((department) => department.id == id)
            ?.departmentName;
        },
        setLoadingDepartments(status: boolean) {
          this.isLoadingDepartments = status;
        },
        setLoadingDepartment(status: boolean) {
          this.isLoadingDepartment = status;
        },
        setDepartments(departments: DepartmentModel[]) {
          this.departments = departments;
        },
        setCursor(cursor: string | null) {
          this.cursor = cursor;
        },
        setFetchingMore(status: boolean) {
          this.isFetchingMore = status;
        },
        refreshAuth(): void {
          this.initApi();
        },
        setDepartment(department: DepartmentModel) {
          this.department = department;
        },
        async create(departmentCreate: CreateDepartmentModel): Promise<number> {
          try {
            this.setLoadingDepartment(true);
            const res = await this.callApi('departmentsPut', {
              createDepartmentRequest: departmentCreate,
            });
            return res.id;
          } finally {
            this.setLoadingDepartment(false);
          }
        },
        async fetch(departmentId: number): Promise<void> {
          try {
            this.setLoadingDepartment(true);
            const departmentGet: DepartmentModel = await this.callApi(
              'departmentsIdGet',
              {
                id: departmentId,
              },
            );
            this.setDepartment(departmentGet);
          } finally {
            this.setLoadingDepartment(false);
          }
        },
        async fetchAll(reset = true): Promise<void> {
          try {
            if (reset) {
              this.setLoadingDepartments(true);
              this.setCursor(null);
              this.setDepartments([]);
            } else {
              this.setFetchingMore(true);
            }

            const payload: { limit: number | undefined; cursor?: string } = {
              limit: this.limit,
            };

            if (!reset && this.cursor) {
              payload.cursor = this.cursor;
            }

            const departmentsGet: DepartmentListModel = await this.callApi(
              'departmentsGet',
              payload,
            );

            if (reset) {
              this.setDepartments(departmentsGet.resources);
            } else {
              this.setDepartments([
                ...this.departments,
                ...departmentsGet.resources,
              ]);
            }

            this.setPermissions(departmentsGet.permissions);
            this.setCursor(departmentsGet.cursor ?? null);
          } finally {
            this.setLoadingDepartments(false);
            this.setFetchingMore(false);
          }
        },
        async delete(departmentId: number): Promise<void> {
          try {
            this.setLoadingDepartment(true);
            await this.callApi('departmentsIdDelete', { id: departmentId });
            await this.fetchAll();
          } finally {
            this.setLoadingDepartment(false);
          }
        },
        async update(
          departmentId: DepartmentModel['id'],
          payload: DepartmentEditModel,
        ): Promise<void> {
          try {
            this.setLoadingDepartment(true);
            await this.callApi('departmentsIdPatch', {
              id: departmentId,
              updateDepartmentRequest: payload,
            });
            this.fetchAll();
            this.fetch(departmentId);
          } finally {
            this.setLoadingDepartment(false);
          }
        },
        nullDepartment() {
          this.department = undefined;
        },
      },
    },
    useApiStore(DepartmentsApi, pinia),
  )(pinia);
};

type DepartmentStore = ReturnType<typeof useDepartmentStore>;
export type { DepartmentStore };
