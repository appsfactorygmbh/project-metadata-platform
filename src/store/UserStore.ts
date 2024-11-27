import { userService } from '@/services/UserService';
import type {
  CreateUserModel,
  UpdateUserModel,
  UserListModel,
  UserModel,
} from '@/models/User';
import { defineStore } from 'pinia';

type StoreState = {
  users: UserListModel[];
  user: UserModel | null;
  me: UserModel | null;
  isLoadingCreate: boolean;
  isLoadingUsers: boolean;
  isLoadingDelete: boolean;
  isLoadingUpdate: boolean;
  createdSuccessfully: boolean;
  removedSuccessfully: boolean;
  updatedSuccessfully: boolean;
};

export const useUserStore = defineStore('user', {
  state: (): StoreState => {
    return {
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
    };
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
    getisLoadingDelete(): boolean {
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
      try {
        this.setIsLoadingUsers(true);
        const users: UserListModel[] = (await userService.fetchUsers()) ?? [];
        this.setUsers(users);
      } finally {
        this.setIsLoadingUsers(false);
      }
    },

    async fetchUser(userId: string): Promise<void> {
      try {
        const user = (await userService.fetchUser(userId)) ?? null;
        this.setUser(user);
      } finally {
        this.setIsLoadingUsers(false);
      }
    },

    async fetchMe(): Promise<void> {
      try {
        const user = (await userService.fetchMe()) ?? null;
        this.setMe(user);
      } finally {
        this.setIsLoadingUsers(false);
      }
    },

    async createUser(newUser: CreateUserModel): Promise<void> {
      try {
        this.setIsLoadingCreate(true);
        this.setCreatedSuccessfully(false);
        const response = await userService.createUser(newUser);
        if (response) {
          this.fetchUsers();
          this.setCreatedSuccessfully(true);
        } else {
          this.setCreatedSuccessfully(false);
        }
      } finally {
        this.setIsLoadingCreate(false);
      }
    },

    async patchUser(userId: string, userPatch: UpdateUserModel): Promise<void> {
      try {
        this.setIsLoadingUpdate(true);
        this.setUpdatedSuccessfully(false);
        const response = await userService.updateUser(userId, userPatch);
        if (response) {
          this.fetchUsers();
          this.setUpdatedSuccessfully(true);
        } else {
          this.setUpdatedSuccessfully(false);
        }
      } catch {
        this.setUpdatedSuccessfully(false);
      } finally {
        this.setIsLoadingUpdate(false);
      }
    },

    async deleteUser(userId: string): Promise<void> {
      try {
        this.setIsLoadingDelete(true);
        this.setRemovedSuccessfully(false);
        const response = await userService.deleteUser(userId);
        if (response) {
          this.setRemovedSuccessfully(true);
          this.fetchUsers();
        }
      } finally {
        this.setIsLoadingDelete(false);
      }
    },
  },
});

type UserStore = ReturnType<typeof useUserStore>;
export type { UserStore };
