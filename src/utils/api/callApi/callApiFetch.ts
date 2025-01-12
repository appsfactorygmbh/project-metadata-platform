import type { BaseAPI } from '@/api/generated';
import { handleFetchError } from '../handleError';
import type { EnsureFunction } from './types';

export type FetchResponse<Api extends BaseAPI, Endpoint extends keyof Api> =
  // @ts-expect-error function type is not inferred correctly
  ReturnType<Api[Endpoint]>;

// Type does not work as expected
// TODO: Remove this
export type CallApiFetch<
  Api extends BaseAPI,
  Endpoint extends keyof Api = keyof Api,
  // @ts-expect-error otherwise type is not inferred correctly
  Args extends Parameters<Api[Endpoint]>[0] = Parameters<Api[Endpoint]>[0],
> = (
  apiCall: Endpoint,
  args: Args extends undefined ? never : Args,
) => FetchResponse<Api, Endpoint>;

export const callApiFetch = async <
  Api extends BaseAPI,
  Endpoint extends keyof Api,
  Args extends Parameters<EnsureFunction<Api[Endpoint]>>[0],
>(
  apiCall: Endpoint,
  args: Args extends undefined ? never : Args,
  api?: Api,
  // @ts-expect-error Response type is always an Promise
): FetchResponse<Api, Endpoint> => {
  try {
    if (!api) throw new Error('No Api provided');
    // @ts-expect-error complains about the type of api[apiCall] but it's correct
    return (await api[apiCall](args as Args)) as FetchResponse<Api, Endpoint>;
  } catch (err) {
    console.log('error: ', err);
    await handleFetchError(err);
    // @ts-expect-error complains about the return type, but handleError always throws an error so we can ignore it
    return undefined;
  }
};
