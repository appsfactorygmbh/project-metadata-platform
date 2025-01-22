import { ResponseError } from '@/api/generated';

export const getFetchErrorMessage = async (
  error: unknown,
  defaultMessage: string | undefined = 'Unbekannter Fehler',
): Promise<string> => {
  if (error instanceof ResponseError) {
    return await error.response?.text();
  }
  if (error instanceof Error) {
    return error.message;
  }
  if (!error) return defaultMessage;
  return JSON.stringify(error);
};
