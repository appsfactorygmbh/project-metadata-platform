import { getFetchErrorMessage } from './getErrorMessage';

export const handleFetchError = async (err: unknown): Promise<never> => {
  const msg = await getFetchErrorMessage(err);
  console.log('Error Message:', msg);
  throw new Error(msg);
};
