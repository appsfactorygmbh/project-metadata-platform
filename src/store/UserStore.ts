import { userService } from '@/services/UserService';
import type { UserModel, CreateUserModel } from '@/models/User';
import { defineStore } from 'pinia';

type StoreState = {
  users: UserModel[];
  user: UserModel | null;
  isLoadingCreate: boolean;
  isLoadingUsers: boolean;
  createdSuccessfully: boolean;
};

export const useUserStore = defineStore('user', {
  state: (): StoreState => {
    return {
      users: [],
      user: null,
      isLoadingCreate: false,
      isLoadingUsers: false,
      createdSuccessfully: false,
    };
  },
  getters: {
    getUsers(): UserModel[] {
      return this.users;
    },
    getUser(): UserModel | null {
      return this.user;
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
    getCreatedSuccessfully(): boolean {
      return this.createdSuccessfully;
    },
  },
  actions: {
    setUsers(users: UserModel[]): void {
      this.users = users;
    },
    setUser(user: UserModel): void {
      this.user = user;
    },
    setIsLoadingCreate(isLoadingCreate: boolean): void {
      this.isLoadingCreate = isLoadingCreate;
    },
    setIsLoadingUsers(isLoadingUsers: boolean): void {
      this.isLoadingUsers = isLoadingUsers;
    },
    setCreatedSuccessfully(createdSuccessfully: boolean): void {
      this.createdSuccessfully = createdSuccessfully;
    },

    async fetchUsers(): Promise<void> {
      try {
        this.setIsLoadingUsers(true);
        const users: UserModel[] = (await userService.fetchUsers()) ?? [];
        this.setUsers(users);
      } finally {
        this.setIsLoadingUsers(false);
      }
    },

    async fetchUser(id: number): Promise<void> {
      try {
        const user = (await userService.fetchUser(id)) ?? {
          id: -1,
          name: '',
          username: '',
          email: '',
        };
        this.setUser(user);
      } finally {
        this.setIsLoadingUsers(false);
      }
    },

    async createUser(newUser: CreateUserModel): Promise<void> {
      try {
        this.setIsLoadingCreate(true);
        this.setCreatedSuccessfully(false);
        const response = await userService.createUser(newUser);
        if (response && response.ok) {
          this.fetchUsers();
          this.setCreatedSuccessfully(true);
        } else {
          this.setCreatedSuccessfully(false);
        }
      } finally {
        this.setIsLoadingCreate(false);
      }
    },
  },
});

type UserStore = ReturnType<typeof useUserStore>;
export type { UserStore };
