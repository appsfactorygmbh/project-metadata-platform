import { getErrorMessage } from './getErrorMessage';

export const handleError = async (err: unknown): Promise<never> => {
  const msg = await getErrorMessage(err);
  console.log('Error Message:', msg);
  throw new Error(msg);
};
