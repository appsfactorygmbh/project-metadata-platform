import type { BaseAPI } from '@/api/generated';
import { type CallApiFetch, callApiFetch } from './callApiFetch';

export type CallApiType<
  Api extends BaseAPI,
  Endpoint extends keyof Api = keyof Api,
  // @ts-expect-error otherwise type is not inferred correctly
  Args extends Parameters<Api[Endpoint]>[0] = Parameters<Api[Endpoint]>[0],
> = CallApiFetch<Api, Endpoint, Args>;

// @ts-ignore
export const callApi = callApiFetch;
