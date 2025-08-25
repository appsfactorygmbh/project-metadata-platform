import type { PiniaStore, defineGenericStore } from 'pinia-generic';

export type GenericStore<
  TStore extends PiniaStore,
  TBaseStore extends PiniaStore = PiniaStore,
> = ReturnType<typeof defineGenericStore<TStore, TBaseStore>>;
