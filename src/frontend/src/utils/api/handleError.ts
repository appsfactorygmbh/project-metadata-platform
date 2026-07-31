import { ResponseError } from '@/api/generated';
import { getFetchErrorMessage } from './getErrorMessage';
import { appEventBus } from '../errors/eventBus';

export const handleFetchError = async (err: unknown): Promise<never> => {
  if (err instanceof ResponseError) {
    if (err.response.status === 401) {
      console.log('REFRESH BECAUSE 401');
      appEventBus.emit('criticalAuthFailure');
    }
    if (err.response.status === 403) {
      throw new Error('This action is unauthorized.');
    }
  }
  const msg = await getFetchErrorMessage(err);
  throw new Error(msg);
};
