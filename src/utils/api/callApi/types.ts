import type { BaseAPI } from '@/api/generated';

export type EnsureFunction<T> = T extends (...args: unknown[]) => unknown
  ? T
  : never;

export type CallArgs<
  Api extends BaseAPI,
  Endpoint extends keyof Api,
> = Parameters<EnsureFunction<Api[Endpoint]>>[0];
