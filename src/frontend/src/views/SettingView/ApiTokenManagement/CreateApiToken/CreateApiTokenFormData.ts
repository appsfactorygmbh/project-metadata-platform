import type { TokenScopes } from '@/api/generated';

export type CreateApiTokenFormData = {
  name: string;
  scopes: Array<TokenScopes>;
  inputsDisabled: boolean;
};
