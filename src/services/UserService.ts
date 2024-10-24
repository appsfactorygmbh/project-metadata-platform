import type { UserModel, CreateUserModel } from '@/models/User';
import { ApiService } from './ApiService';

class UserService extends ApiService {
  fetchUsers = async (): Promise<UserModel[] | null> => {
    const url = `/Users`;
    try {
      const response = await this.fetch(url);
      if (!response.ok) {
        throw new Error('Network response was not ok');
      }

      const data: UserModel[] = await response.json();

      return data;
    } catch (err) {
      console.error('Error fetching users: ' + err);
      return null;
    }
  };

  fetchUser = async (userId: number): Promise<UserModel | null> => {
    try {
      const response = await this.fetch('/Users/' + userId.toString(), {
        headers: {
          Accept: 'text/plain',
          'Access-Control-Allow-Origin': '*',
          cors: 'cors',
        },
      });

      const data: UserModel = await response.json();
      return data;
    } catch (err) {
      console.error('Error fetching user: ' + err);
      return null;
    }
  };

  createUser = async (newUser: CreateUserModel): Promise<Response | null> => {
    try {
      const response = await this.fetch('/Users', {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(newUser),
        mode: 'cors',
      });
      return response;
    } catch (error) {
      console.error('Error:', error);
      return null;
    }
  };

  deleteUser = async (userId: number): Promise<Response | null> => {
    try {
      const response = await this.fetch('/Users/' + userId.toString(), {
        method: 'DELETE',
        mode: 'cors',
      });
      return response;
    } catch (err) {
      console.error('Error deleting user: ' + err);
      return null;
    }
  };
}

const userService = new UserService();
export { userService };
export type { UserService };
