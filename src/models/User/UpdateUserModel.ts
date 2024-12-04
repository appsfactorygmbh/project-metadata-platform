import type { GetUserResponse } from '@/api/generated';

export type UpdateUserModel = Partial<GetUserResponse> & {
  password?: string;
};
