export const initRef = <T>(
  param: T | undefined,
  defaultValue: T | Record<string, never>,
): Ref<T> =>
  param !== undefined
    ? (ref<T>(param) as Ref<T>)
    : (ref<T>(defaultValue as T) as Ref<T>);
