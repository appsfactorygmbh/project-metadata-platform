import type { CreateUserModel } from '@/models/User';
import { ApiService } from './ApiService';
import type { UsersApi } from '@/api/generated';

class UserService extends ApiService<UsersApi> {
  fetchUsers = async () => {
    const response = await this.callApi('usersGet', {});
    if (!response) return;
    return response;
  };

  fetchUser = async (userId: string) => {
    const user = await this.callApi('usersUserIdGet', {
      userId,
    });
    if (!user) return;
    return user;
  };

  createUser = async (newUser: CreateUserModel) => {
    const response = await this.callApi('usersPut', {
      createUserRequest: newUser,
    });
    if (!response) return;
    return response;
  };

  // TODO: need backend support
  // deleteUser = async (userId: string) => {
  //   const response = await this.callApi("", {
  //     userId,
  //   });
  //   if (!response) return;
  //   return response;
  // };

  updateUser = async (userId: string, updatedUser: CreateUserModel) => {
    const response = await this.callApi('usersUserIdPatch', {
      userId,
      patchUserRequest: updatedUser,
    });
    if (!response) return;
    return response;
  };
}

const userService = new UserService('UserService');
export { userService };
export type { UserService };
