import { TeamsApi } from '@/api/generated';
import type { TeamListModel, TeamModel } from '@/models/Team/TeamModel';
import { type PiniaStore, useStore } from 'pinia-generic';
import { piniaInstance } from './piniaInstance';
import type { Pinia } from 'pinia';
import type { CreateTeamModel } from '@/models/Team/CreateTeamModel';
import type { TeamEditModel } from '@/models/Team';
import { type ApiStore, useApiStore } from './ApiStore';
import type { ResourceActions } from '@/models/utils';

type StoreState = {
  teams: TeamModel[];
  team: TeamModel | undefined;
  // in sync with team
  linkedProjects: string[];
  isLoadingTeam: boolean;
  isLoadingTeams: boolean;
  cursor: string | null;
  isFetchingMore: boolean;
  limit: number | undefined;
};

type StoreGetters = {
  getTeams: () => TeamModel[];
  getTeam: () => TeamModel | undefined;
  getLinkedProjects: () => string[];
  getTeamNames: () => string[];
  getPermissions: () => ResourceActions[];
  getIsLoadingTeams: () => boolean;
  getIsLoadingTeam: () => boolean;
  getCursor: () => string | null;
  getIsFetchingMore: () => boolean;
};

type StoreActions = {
  refreshAuth: () => void;
  fetchAll: (reset?: boolean) => Promise<void>;
  setCursor: (cursor: string | null) => void;
  setFetchingMore: (status: boolean) => void;
  fetch: (teamId: number) => Promise<void>;
  fetchLinkedProjects: (teamId: number) => Promise<void>;
  create: (teamCreate: CreateTeamModel) => Promise<number>;
  update: (teamId: TeamModel['id'], payload: TeamEditModel) => Promise<void>;
  delete: (teamId: number) => Promise<void>;
  setLoadingTeams: (status: boolean) => void;
  setLoadingTeam: (status: boolean) => void;
  setTeams: (teams: TeamModel[]) => void;
  setTeam: (team: TeamModel) => void;
  nullTeam: () => void;
  getIdToName: (name: string) => number | undefined;
  getNameToId: (id: number) => string | undefined;
};

type Store = PiniaStore<'team', StoreState, StoreGetters, StoreActions>;

export const useTeamStore = (pinia: Pinia = piniaInstance): Store => {
  return useStore<Store, ApiStore<TeamsApi>>(
    'team',
    {
      state: {
        teams: [],
        team: undefined,
        linkedProjects: [],
        isLoadingTeams: false,
        isLoadingTeam: false,
        cursor: null,
        isFetchingMore: false,
        limit: Number(import.meta.env.VITE_LIST_PAGINATION_LIMIT) || undefined,
      },

      getters: {
        getTeams(): TeamModel[] {
          return this.teams;
        },
        getTeam(): TeamModel | undefined {
          return this.team;
        },
        getIsLoadingTeams(): boolean {
          return this.isLoadingTeams;
        },
        getIsLoadingTeam(): boolean {
          return this.isLoadingTeam;
        },
        getTeamNames(): string[] {
          return this.teams.map((team) => team.teamName);
        },
        getLinkedProjects(): string[] {
          return this.linkedProjects;
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
          return this.teams.find((team) => team.teamName == name)?.id;
        },
        getNameToId(id: number): string | undefined {
          return this.teams.find((team) => team.id == id)?.teamName;
        },
        setLoadingTeams(status: boolean) {
          this.isLoadingTeams = status;
        },
        setLoadingTeam(status: boolean) {
          this.isLoadingTeam = status;
        },
        setTeams(teams: TeamModel[]) {
          this.teams = teams;
        },
        refreshAuth(): void {
          this.initApi();
        },
        setTeam(team: TeamModel) {
          this.team = team;
        },
        setCursor(cursor: string | null) {
          this.cursor = cursor;
        },
        setFetchingMore(status: boolean) {
          this.isFetchingMore = status;
        },
        async create(teamCreate: CreateTeamModel): Promise<number> {
          try {
            this.setLoadingTeam(true);
            const res = await this.callApi('teamsPut', {
              createTeamRequest: teamCreate,
            });
            return res.id;
          } finally {
            this.setLoadingTeam(false);
          }
        },
        async fetch(teamId: number): Promise<void> {
          try {
            this.setLoadingTeam(true);
            const teamGet: TeamModel = await this.callApi('teamsIdGet', {
              id: teamId,
            });
            await this.fetchLinkedProjects(teamId);
            this.setTeam(teamGet);
          } finally {
            this.setLoadingTeam(false);
          }
        },
        async fetchAll(reset = true): Promise<void> {
          try {
            if (reset) {
              this.setLoadingTeams(true);
              this.setCursor(null);
              this.setTeams([]);
            } else {
              this.setFetchingMore(true);
            }

            const payload: { limit: number | undefined; cursor?: string } = {
              limit: this.limit,
            };

            if (!reset && this.cursor) {
              payload.cursor = this.cursor;
            }

            const teamsGet: TeamListModel = await this.callApi(
              'teamsGet',
              payload,
            );

            if (reset) {
              this.setTeams(teamsGet.resources);
            } else {
              this.setTeams([...this.teams, ...teamsGet.resources]);
            }

            this.setPermissions(teamsGet.permissions);
            this.setCursor(teamsGet.cursor ?? null);
          } finally {
            this.setLoadingTeams(false);
            this.setFetchingMore(false);
          }
        },
        async delete(teamId: number): Promise<void> {
          try {
            this.setLoadingTeam(true);
            await this.callApi('teamsTeamIdDelete', { teamId: teamId });
            await this.fetchAll();
          } finally {
            this.setLoadingTeam(false);
          }
        },
        async update(
          teamId: TeamModel['id'],
          payload: TeamEditModel,
        ): Promise<void> {
          try {
            this.setLoadingTeam(true);
            await this.callApi('teamsTeamIdPatch', {
              teamId: teamId,
              patchTeamRequest: payload,
            });
            this.fetchAll();
            this.fetch(teamId);
          } finally {
            this.setLoadingTeam(false);
          }
        },
        async fetchLinkedProjects(teamId: number): Promise<void> {
          try {
            this.linkedProjects = (
              await this.callApi('teamsTeamIdLinkedProjectsGet', {
                teamId: teamId,
              })
            ).projectSlugs;
          } catch (e) {
            console.error('failed retrieving linked projects for team');
            throw e;
          }
        },
        nullTeam() {
          this.team = undefined;
        },
      },
    },
    useApiStore(TeamsApi, pinia),
  )(pinia);
};

type TeamStore = ReturnType<typeof useTeamStore>;
export type { TeamStore };
