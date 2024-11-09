import type { BaseAPI } from '@/api/generated';
import { handleError } from './handleError';

export const callApi = async <
  Api extends BaseAPI,
  Endpoint extends keyof Api,
  Args extends Parameters<
    Api[Endpoint] extends (...args: unknown[]) => unknown
      ? Api[Endpoint]
      : never
  >[0],
>(
  apiCall: Endpoint,
  args: Args extends undefined ? never : Args,
  api?: Api,
  // @ts-expect-error wants an undefined but handleError always throws an error so we can ignore it
): Promise<ReturnType<Api[Endpoint]>> => {
  try {
    if (!api) throw new Error('No Api provided');
    // @ts-expect-error complains about the type of api[apiCall] but it's correct
    return (await api[apiCall](args as Args)) as ReturnType<Api[Endpoint]>;
  } catch (err) {
    await handleError(err);
  }
};
