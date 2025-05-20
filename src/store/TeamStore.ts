import { TeamsApi } from '@/api/generated';
import type { TeamModel } from '@/models/Team/TeamModel';
import { useStore, type PiniaStore } from 'pinia-generic';
import { piniaInstance } from './piniaInstance';
import type { Pinia } from 'pinia';
import type { CreateTeamModel } from '@/models/Team/CreateTeamModel';
import type { TeamEditModel } from '@/models/Team';
import { useApiStore, type ApiStore } from './ApiStore';

type StoreState = {
  teams: TeamModel[];
  team: TeamModel;
  // in sync with team
  linkedProjects: number[];
  isLoadingTeams: boolean;
  loadedTeamsSuccessfully: boolean;
};

type StoreGetters = {
  getTeams: () => TeamModel[];
  getTeam: () => TeamModel;
  getLinkedProjects: () => number[];
  getTeamNames: () => string[];
  getIsLoadingTeams: () => boolean;
};

type StoreActions = {
  refreshAuth: () => void;
  fetchAll: () => Promise<void>;
  fetch: (teamId: number) => Promise<void>;
  fetchLinkedProjects: (teamId: number) => Promise<void>;
  create: (teamCreate: CreateTeamModel) => Promise<void>;
  update: (
    teamId: TeamModel['id'],
    payload: TeamEditModel,
  ) => Promise<TeamModel>;
  delete: (teamId: number) => Promise<void>;
  setLoadingTeams: (status: boolean) => void;
  setTeams: (teams: TeamModel[]) => void;
  setTeam: (team: TeamModel) => void;
  getIdToName: (name: string) => number | undefined;
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
        loadedTeamsSuccessfully: false,
      },

      getters: {
        getTeams(): TeamModel[] {
          return this.teams;
        },
        getTeam(): TeamModel {
          return this.team;
        },
        getIsLoadingTeams(): boolean {
          return this.isLoadingTeams;
        },
        getTeamNames(): string[] {
          return this.teams.map((team) => team.teamName);
        },
        getLinkedProjects(): number[] {
          return this.linkedProjects;
        },
      },

      actions: {
        getIdToName(name: string): number | undefined {
          return this.teams.find((team) => team.teamName == name)?.id;
        },
        setLoadingTeams(status: boolean) {
          this.isLoadingTeams = status;
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
        async fetch(teamId: number): Promise<void> {
          try {
            this.setLoadingTeams(true);
            const teamGet: TeamModel = await this.callApi('teamsIdGet', {
              id: teamId,
            });
            this.fetchLinkedProjects(teamId);
            this.setTeam(teamGet);
          } finally {
            this.setLoadingTeams(false);
          }
        },
        async fetchAll(): Promise<void> {
          try {
            this.setLoadingTeams(true);
            const teamsGet: TeamModel[] = await this.callApi(
              'teamsGet',
              undefined,
            );
            this.setTeams(teamsGet);
          } finally {
            this.setLoadingTeams(false);
          }
        },
        async delete(teamId: number): Promise<void> {
          try {
            this.setLoadingTeams(true);
            await this.callApi('teamsTeamIdDelete', { teamId: teamId });
            this.fetchAll();
          } finally {
            this.setLoadingTeams(false);
          }
        },
        async update(teamId: TeamModel['id'],payload: TeamEditModel): Promise<TeamModel>{
        try {
            const team = await this.callApi('teamsTeamIdPatch', { teamId: teamId, patchTeamRequest: payload});
            this.fetchAll();
            return team;
          } finally {
            this.setLoadingTeams(false);
          }
      },
      async fetchLinkedProjects(teamId: number): Promise<void>{
         try {
            this.linkedProjects= (await this.callApi('teamsTeamIdLinkedProjectsGet', { teamId: teamId })).projectIds;
          } finally {

          }
      }
      },
    },
    useApiStore(TeamsApi, pinia),
  )(pinia);
};

type TeamStore = ReturnType<typeof useTeamStore>;
export type { TeamStore };
