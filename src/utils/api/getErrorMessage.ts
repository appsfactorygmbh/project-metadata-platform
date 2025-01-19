import { ResponseError } from '@/api/generated';

export const getFetchErrorMessage = async (
  error: unknown,
  defaultMessage: string | undefined = 'Unbekannter Fehler',
): Promise<string> => {
  if (error instanceof ResponseError) {
    try {
      console.log(error.response);
      const res = await error.response.json();
      return res;
    } catch (e) {
      console.error('Error in json():', e);
    }
  }
  if (error instanceof Error) {
    return error.message;
  }
  if (!error) return defaultMessage;
  return JSON.stringify(error);
};
