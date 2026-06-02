import { type ProjectStore } from './ProjectStore.ts';
import { type PluginStore } from './PluginStore';
import { type ProjectEditStore } from './ProjectEditStore/ProjectEditStore.ts';
import { type UserStore } from './UserStore';
import { useLogsStore } from './LogsStore';
import { useProjectRouting } from '@/utils/hooks/useProjectRouting.ts';
import { useLocalLogStore } from './LocalLogStore';
import type { InjectionKey } from 'vue';
import type { GlobalPluginStore } from './GlobalPluginStore.ts';
import type { AuthStore } from './AuthStore.ts';
import { useUserRouting } from '@/utils/hooks/useUserRouting.ts';
import type { TeamStore } from './TeamStore.ts';
import type { useTeamRouting } from '@/utils/hooks/useTeamRouting.ts';
import type { ApiTokenStore } from './ApiTokenStore.ts';
import type { useApiTokenRouting } from '@/utils/hooks/useApiTokenRouting.ts';
import type { CompanyStore } from './CompanyStore.ts';
import type { BusinessUnitStore } from './BusinessUnitStore.ts';
import type { OfficeLocationStore } from './OfficeLocationStore.ts';
import type { DepartmentStore } from './DepartmentStore.ts';
import type { useCompanyRouting } from '@/utils/hooks/useCompanyRouting.ts';
import type { useDepartmentRouting } from '@/utils/hooks/useDepartmentRouting.ts';
import type { useBusinessUnitRouting } from '@/utils/hooks/useBusinessUnitRouting .ts';
import type { useOfficeLocationRouting } from '@/utils/hooks/useOfficeLocationRouting.ts';

const projectStoreSymbol = Symbol() as InjectionKey<ProjectStore>;

const pluginStoreSymbol = Symbol() as InjectionKey<PluginStore>;

const projectEditStoreSymbol = Symbol() as InjectionKey<ProjectEditStore>;

const userStoreSymbol = Symbol() as InjectionKey<UserStore>;

const globalPluginStoreSymbol = Symbol() as InjectionKey<GlobalPluginStore>;

const teamStoreSymbol = Symbol() as InjectionKey<TeamStore>;

const authStoreSymbol = Symbol() as InjectionKey<AuthStore>;

const apiTokenStoreSymbol = Symbol() as InjectionKey<ApiTokenStore>;

const companyStoreSymbol = Symbol() as InjectionKey<CompanyStore>;

const departmentStoreSymbol = Symbol() as InjectionKey<DepartmentStore>;

const businessUnitStoreSymbol = Symbol() as InjectionKey<BusinessUnitStore>;

const officeLocationStoreSymbol = Symbol() as InjectionKey<OfficeLocationStore>;

const logsStoreSymbol = Symbol() as InjectionKey<
  ReturnType<typeof useLogsStore>
>;

const projectRoutingSymbol = Symbol() as InjectionKey<
  ReturnType<typeof useProjectRouting>
>;

const localLogStoreSymbol = Symbol() as InjectionKey<
  ReturnType<typeof useLocalLogStore>
>;

const userRoutingSymbol = Symbol() as InjectionKey<
  ReturnType<typeof useUserRouting>
>;

const teamRoutingSymbol = Symbol() as InjectionKey<
  ReturnType<typeof useTeamRouting>
>;

const apiTokenRoutingSymbol = Symbol() as InjectionKey<
  ReturnType<typeof useApiTokenRouting>
>;

const companyRoutingSymbol = Symbol() as InjectionKey<
  ReturnType<typeof useCompanyRouting>
>;

const departmentRoutingSymbol = Symbol() as InjectionKey<
  ReturnType<typeof useDepartmentRouting>
>;

const businessUnitRoutingSymbol = Symbol() as InjectionKey<
  ReturnType<typeof useBusinessUnitRouting>
>;

const officeLocationRoutingSymbol = Symbol() as InjectionKey<
  ReturnType<typeof useOfficeLocationRouting>
>;

export {
  projectStoreSymbol,
  pluginStoreSymbol,
  globalPluginStoreSymbol,
  projectEditStoreSymbol,
  userStoreSymbol,
  authStoreSymbol,
  logsStoreSymbol,
  projectRoutingSymbol,
  localLogStoreSymbol,
  userRoutingSymbol,
  teamStoreSymbol,
  teamRoutingSymbol,
  apiTokenStoreSymbol,
  apiTokenRoutingSymbol,
  companyStoreSymbol,
  companyRoutingSymbol,
  businessUnitStoreSymbol,
  businessUnitRoutingSymbol,
  departmentStoreSymbol,
  departmentRoutingSymbol,
  officeLocationStoreSymbol,
  officeLocationRoutingSymbol
};
