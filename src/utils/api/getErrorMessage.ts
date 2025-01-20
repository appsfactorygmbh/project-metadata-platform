import { ResponseError } from '@/api/generated';

export const getFetchErrorMessage = async (
  error: unknown,
  defaultMessage: string | undefined = 'Unbekannter Fehler',
): Promise<string> => {
  if (error instanceof ResponseError) {
    const res = await error.response?.json();
    return JSON.stringify(res);
  }
  if (error instanceof Error) {
    return error.message;
  }
  if (!error) return defaultMessage;
  return JSON.stringify(error);
};
