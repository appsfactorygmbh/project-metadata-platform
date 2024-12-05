import type { BaseAPI } from '@/api/generated';
import { type CallApiFetch, callApiFetch } from './callApiFetch';

// Type does not work as expected
// TODO: Remove this
export type CallApiType<
  Api extends BaseAPI,
  Endpoint extends keyof Api = keyof Api,
  // @ts-expect-error otherwise type is not inferred correctly
  Args extends Parameters<Api[Endpoint]>[0] = Parameters<Api[Endpoint]>[0],
> = CallApiFetch<Api, Endpoint, Args>;

export const callApi = callApiFetch;
