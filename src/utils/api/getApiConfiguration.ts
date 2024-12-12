import { Configuration } from '@/api/generated';
import { API_BASE_PATH } from '@/constants';

export const getApiConfiguration = (accessToken: string) => {
  return new Configuration({
    basePath: API_BASE_PATH,
    accessToken,
    headers: {
      Authorization: `Bearer ${accessToken}`,
    },
  });
};
