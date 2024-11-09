import { Configuration } from '@/api/generated';

export const getApiConfiguration = (accessToken: string) => {
  return new Configuration({
    basePath: import.meta.env.VITE_BACKEND_URL,
    accessToken,
    headers: {
      Accept: 'application/json',
    },
  });
};
