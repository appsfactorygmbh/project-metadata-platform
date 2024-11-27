import { userService } from '@/services/UserService';
import type { CreateUserModel, UserListModel, UserModel } from '@/models/User';
import { defineStore } from 'pinia';

type StoreState = {
  users: UserListModel[];
  user: UserModel | null;
  isLoadingCreate: boolean;
  isLoadingUsers: boolean;
  isLoadingDelete: boolean;
  createdSuccessfully: boolean;
  removedSuccessfully: boolean;
};

export const useUserStore = defineStore('user', {
  state: (): StoreState => {
    return {
      users: [],
      user: null,
      isLoadingCreate: false,
      isLoadingUsers: false,
      isLoadingDelete: false,
      createdSuccessfully: false,
      removedSuccessfully: false,
    };
  },
  getters: {
    getUsers(): UserListModel[] {
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
    getisLoadingDelete(): boolean {
      return this.isLoadingDelete;
    },
    getCreatedSuccessfully(): boolean {
      return this.createdSuccessfully;
    },
    getRemovedSuccessfully(): boolean {
      return this.removedSuccessfully;
    },
  },
  actions: {
    setUsers(users: UserListModel[]): void {
      this.users = users;
    },
    setUser(user: UserModel | null): void {
      this.user = user;
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
    setCreatedSuccessfully(createdSuccessfully: boolean): void {
      this.createdSuccessfully = createdSuccessfully;
    },
    setRemovedSuccessfully(removedSuccessfully: boolean): void {
      this.removedSuccessfully = removedSuccessfully;
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

    // TODO: need backend support
    // async deleteUser(userId: number): Promise<void> {
    //   try {
    //     this.setIsLoadingDelete(true);
    //     this.setRemovedSuccessfully(false);
    //     const responyse = await userService.deleteUser(userId);
    //     if (response && response.ok) {
    //       this.setRemovedSuccessfully(true);
    //       this.fetchUsers();
    //     }
    //   } finally {
    //     this.setIsLoadingDelete(false);
    //   }
    // },
  },
});

type UserStore = ReturnType<typeof useUserStore>;
export type { UserStore };
