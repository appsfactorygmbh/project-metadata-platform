import type { PmpScimUser, PmpUserExtension } from '@/api/generated';

export type UserListModel = Pick<PmpScimUser, 'externalId' | 'userName'> & {
  isScimProvisioned: PmpUserExtension['isScimProvisioned'];
};

export type UserModel = PmpScimUser;
