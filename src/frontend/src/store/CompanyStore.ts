import { CompaniesApi } from '@/api/generated';
import type {
  CompanyListModel,
  CompanyModel,
} from '@/models/Company/CompanyModel';
import { type PiniaStore, useStore } from 'pinia-generic';
import { piniaInstance } from './piniaInstance';
import type { Pinia } from 'pinia';
import type { CreateCompanyModel } from '@/models/Company/CreateCompanyModel';
import type { CompanyEditModel } from '@/models/Company';
import { type ApiStore, useApiStore } from './ApiStore';

type StoreState = {
  companies: CompanyModel[];
  company: CompanyModel | undefined;
  // in sync with company
  linkedProjects: string[];
  isLoadingCompany: boolean;
  isLoadingCompanies: boolean;
};

type StoreGetters = {
  getCompanies: () => CompanyModel[];
  getCompany: () => CompanyModel | undefined;
  getLinkedProjects: () => string[];
  getCompanyNames: () => string[];
  getIsLoadingCompanies: () => boolean;
  getIsLoadingCompany: () => boolean;
};

type StoreActions = {
  refreshAuth: () => void;
  fetchAll: () => Promise<void>;
  fetch: (companyId: number) => Promise<void>;
  fetchLinkedProjects: (companyId: number) => Promise<void>;
  create: (companyCreate: CreateCompanyModel) => Promise<number>;
  update: (
    companyId: CompanyModel['id'],
    payload: CompanyEditModel,
  ) => Promise<void>;
  delete: (companyId: number) => Promise<void>;
  setLoadingCompanies: (status: boolean) => void;
  setLoadingCompany: (status: boolean) => void;
  setCompanies: (companies: CompanyModel[]) => void;
  setCompany: (company: CompanyModel) => void;
  nullCompany: () => void;
  getIdToName: (name: string) => number | undefined;
  getNameToId: (id: number) => string | undefined;
};

type Store = PiniaStore<'company', StoreState, StoreGetters, StoreActions>;

export const useCompanyStore = (pinia: Pinia = piniaInstance): Store => {
  return useStore<Store, ApiStore<CompaniesApi>>(
    'company',
    {
      state: {
        companies: [],
        company: undefined,
        linkedProjects: [],
        isLoadingCompanies: false,
        isLoadingCompany: false,
      },

      getters: {
        getCompanies(): CompanyModel[] {
          return this.companies;
        },
        getCompany(): CompanyModel | undefined {
          return this.company;
        },
        getIsLoadingCompanies(): boolean {
          return this.isLoadingCompanies;
        },
        getIsLoadingCompany(): boolean {
          return this.isLoadingCompany;
        },
        getCompanyNames(): string[] {
          return this.companies.map((company) => company.companyName);
        },
        getLinkedProjects(): string[] {
          return this.linkedProjects;
        },
      },

      actions: {
        getIdToName(name: string): number | undefined {
          return this.companies.find((company) => company.companyName == name)
            ?.id;
        },
        getNameToId(id: number): string | undefined {
          return this.companies.find((company) => company.id == id)
            ?.companyName;
        },
        setLoadingCompanies(status: boolean) {
          this.isLoadingCompanies = status;
        },
        setLoadingCompany(status: boolean) {
          this.isLoadingCompany = status;
        },
        setCompanies(companies: CompanyModel[]) {
          this.companies = companies;
        },
        refreshAuth(): void {
          this.initApi();
        },
        setCompany(company: CompanyModel) {
          this.company = company;
        },
        async create(companyCreate: CreateCompanyModel): Promise<number> {
          try {
            this.setLoadingCompany(true);
            const res = await this.callApi('companiesPut', {
              createCompanyRequest: companyCreate,
            });
            return res.id;
          } finally {
            this.setLoadingCompany(false);
          }
        },
        async fetch(companyId: number): Promise<void> {
          try {
            this.setLoadingCompany(true);
            const companyGet: CompanyModel = await this.callApi(
              'companiesIdGet',
              {
                id: companyId,
              },
            );
            await this.fetchLinkedProjects(companyId);
            this.setCompany(companyGet);
          } finally {
            this.setLoadingCompany(false);
          }
        },
        async fetchAll(): Promise<void> {
          try {
            this.setLoadingCompanies(true);
            const companiesGet: CompanyListModel = await this.callApi(
              'companiesGet',
              undefined,
            );
            this.setCompanies(companiesGet.resources);
            this.setPermissions(companiesGet.permissions);
          } finally {
            this.setLoadingCompanies(false);
          }
        },
        async delete(companyId: number): Promise<void> {
          try {
            this.setLoadingCompany(true);
            await this.callApi('companiesIdDelete', { id: companyId });
            await this.fetchAll();
          } finally {
            this.setLoadingCompany(false);
          }
        },
        async update(
          companyId: CompanyModel['id'],
          payload: CompanyEditModel,
        ): Promise<void> {
          try {
            this.setLoadingCompany(true);
            await this.callApi('companiesIdPatch', {
              id: companyId,
              updateCompanyRequest: payload,
            });
            this.fetchAll();
            this.fetch(companyId);
          } finally {
            this.setLoadingCompany(false);
          }
        },
        async fetchLinkedProjects(companyId: number): Promise<void> {
          try {
            this.linkedProjects = (
              await this.callApi('companiesIdLinkedProjectsGet', {
                id: companyId,
              })
            ).projectSlugs;
          } catch (e) {
            console.error('failed retrieving linked projects for company');
            throw e;
          }
        },
        nullCompany() {
          this.company = undefined;
        },
      },
    },
    useApiStore(CompaniesApi, pinia),
  )(pinia);
};

type CompanyStore = ReturnType<typeof useCompanyStore>;
export type { CompanyStore };
