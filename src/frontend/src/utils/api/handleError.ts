import { ResponseError } from '@/api/generated';
import { getFetchErrorMessage } from './getErrorMessage';
import { appEventBus } from '../errors/eventBus';

export const handleFetchError = async (err: unknown): Promise<never> => {
  if (err instanceof ResponseError) {
    if (err.response.status === 401) {
      console.log('REFRESH BECAUSE 401');
      appEventBus.emit('criticalAuthFailure');
    }
  }
  const msg = await getFetchErrorMessage(err);
  throw new Error(msg);
};
