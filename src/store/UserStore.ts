import type {
  CreateUserModel,
  UpdateUserModel,
  UserListModel,
  UserModel,
} from '@/models/User';
import { type Pinia } from 'pinia';
import { type PiniaStore, useStore } from 'pinia-generic';
import { piniaInstance } from './piniaInstance';
import { type ApiStore, useApiStore } from './ApiStore';
import { UsersApi } from '@/api/generated';

type StoreState = {
  users: UserListModel[];
  user: UserModel | null;
  me: UserModel | null;
  isLoading: boolean;
  isLoadingCreate: boolean;
  isLoadingUsers: boolean;
  isLoadingDelete: boolean;
  isLoadingUpdate: boolean;
  createdSuccessfully: boolean;
  removedSuccessfully: boolean;
  updatedSuccessfully: boolean;
};

type StoreGetters = {
  getUsers: () => UserListModel[];
  getUser: () => UserModel | null;
  getMe: () => UserModel | null;
  getIsLoading: () => boolean;
  getIsLoadingCreate: () => boolean;
  getIsLoadingUsers: () => boolean;
  getIsLoadingDelete: () => boolean;
  getIsLoadingUpdate: () => boolean;
  getCreatedSuccessfully: () => boolean;
  getRemovedSuccessfully: () => boolean;
  getUpdatedSuccessfully: () => boolean;
};

type StoreActions = {
  setUsers: (users: UserListModel[]) => void;
  setUser: (user: UserModel | null) => void;
  setMe: (me: UserModel | null) => void;
  setIsLoading: (isLoading: boolean) => void;
  setIsLoadingCreate: (isLoadingCreate: boolean) => void;
  setIsLoadingUsers: (isLoadingUsers: boolean) => void;
  setIsLoadingDelete: (isLoadingDelete: boolean) => void;
  setIsLoadingUpdate: (isLoadingUpdate: boolean) => void;
  setCreatedSuccessfully: (createdSuccessfully: boolean) => void;
  setRemovedSuccessfully: (removedSuccessfully: boolean) => void;
  setUpdatedSuccessfully: (updatedSuccessfully: boolean) => void;
  fetchUsers: () => Promise<void>;
  fetchUser: (userId: string) => Promise<void>;
  fetchMe: () => Promise<void>;
  create: (newUser: CreateUserModel) => Promise<void>;
  update: (
    userId: UserModel['id'],
    userPatch: UpdateUserModel,
  ) => Promise<void>;
  delete: (userId: UserModel['id']) => Promise<void>;
};

type Store = PiniaStore<'user', StoreState, StoreGetters, StoreActions>;

export const useUserStore = (pinia: Pinia = piniaInstance): Store => {
  return useStore<Store, ApiStore<UsersApi>>(
    'user',
    {
      state: {
        users: [],
        user: null,
        me: null,
        isLoadingCreate: false,
        isLoadingUsers: false,
        isLoadingDelete: false,
        isLoadingUpdate: false,
        createdSuccessfully: false,
        removedSuccessfully: false,
        updatedSuccessfully: false,
      },
      getters: {
        getUsers(): UserListModel[] {
          return this.users;
        },
        getUser(): UserModel | null {
          return this.user;
        },
        getMe(): UserModel | null {
          return this.me;
        },
        getIsLoading(): boolean {
          return this.isLoadingCreate || this.isLoadingUsers;
        },
        getIsLoadingCreate(): boolean {
          return this.isLoadingCreate;
        },
        getIsLoadingUsers(): boolean {
          return this.isLoadingUsers;
        },
        getIsLoadingDelete(): boolean {
          return this.isLoadingDelete;
        },
        getIsLoadingUpdate(): boolean {
          return this.isLoadingUpdate;
        },
        getCreatedSuccessfully(): boolean {
          return this.createdSuccessfully;
        },
        getRemovedSuccessfully(): boolean {
          return this.removedSuccessfully;
        },
        getUpdatedSuccessfully(): boolean {
          return this.updatedSuccessfully;
        },
      },
      actions: {
        setUsers(users: UserListModel[]): void {
          this.users = users;
        },
        setUser(user: UserModel | null): void {
          this.user = user;
        },
        setMe(me: UserModel | null): void {
          this.me = me;
        },
        setIsLoadingCreate(isLoadingCreate: boolean): void {
          this.isLoadingCreate = isLoadingCreate;
        },
        setIsLoadingUsers(isLoadingUsers: boolean): void {
          this.isLoadingUsers = isLoadingUsers;
        },
        setIsLoadingDelete(isLoadingDelete: boolean): void {
          this.isLoadingDelete = isLoadingDelete;
        },
        setIsLoadingUpdate(isLoadingUpdate: boolean): void {
          this.isLoadingUpdate = isLoadingUpdate;
        },
        setCreatedSuccessfully(createdSuccessfully: boolean): void {
          this.createdSuccessfully = createdSuccessfully;
        },
        setRemovedSuccessfully(removedSuccessfully: boolean): void {
          this.removedSuccessfully = removedSuccessfully;
        },
        setUpdatedSuccessfully(updatedSuccessfully: boolean): void {
          this.updatedSuccessfully = updatedSuccessfully;
        },

        async fetchUsers(): Promise<void> {
          this.setIsLoadingUsers(true);
          try {
            const users: UserListModel[] =
              (await this.callApi('usersGet', {})) ?? [];
            this.setUsers(users);
          } finally {
            this.setIsLoadingUsers(false);
          }
        },

        async fetchUser(userId: string): Promise<void> {
          this.setIsLoadingUsers(true);
          try {
            const user =
              (await this.callApi('usersUserIdGet', { userId })) ?? null;
            this.setUser(user);
          } finally {
            this.setIsLoadingUsers(false);
          }
        },

        async fetchMe(): Promise<void> {
          this.setIsLoadingUsers(true);
          try {
            const user = (await this.callApi('usersMeGet', {})) ?? null;
            this.setMe(user);
          } finally {
            this.setIsLoadingUsers(false);
          }
        },

        async create(newUser: CreateUserModel): Promise<void> {
          try {
            this.setIsLoadingCreate(true);
            this.setCreatedSuccessfully(false);
            await this.callApi('usersPut', {
              createUserRequest: newUser,
            });
            this.fetchUsers();
            this.setCreatedSuccessfully(true);
          } catch (e) {
            this.setCreatedSuccessfully(false);
            throw e;
          } finally {
            this.setIsLoadingCreate(false);
          }
        },

        async update(
          userId: string,
          userUpdate: UpdateUserModel,
        ): Promise<void> {
          try {
            this.setIsLoadingUpdate(true);
            this.setUpdatedSuccessfully(false);
            await this.callApi('usersUserIdPatch', {
              userId,
              patchUserRequest: {
                email: userUpdate.email ?? null,
                password: userUpdate.password ?? null,
              },
            });
            this.fetchUsers();
            this.setUpdatedSuccessfully(true);
          } catch (e) {
            this.setUpdatedSuccessfully(false);
            throw e;
          } finally {
            this.setIsLoadingUpdate(false);
          }
        },

        async delete(userId: string): Promise<void> {
          try {
            this.setIsLoadingDelete(true);
            this.setRemovedSuccessfully(false);
            await this.callApi('usersUserIdDelete', { userId });
            this.setRemovedSuccessfully(true);
            this.fetchUsers();
          } catch (e) {
            this.setRemovedSuccessfully(false);
            throw e;
          } finally {
            this.setIsLoadingDelete(false);
          }
        },
      },
    },
    useApiStore(UsersApi, pinia),
  )(pinia);
};

type UserStore = ReturnType<typeof useUserStore>;
export type { UserStore };
