import type { GetUserResponse } from '@/api/generated';

export type UserListModel = Pick<GetUserResponse, 'id' | 'email'>;

export type UserModel = GetUserResponse;
