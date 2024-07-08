export type ArgsType<T> = T extends (...args: infer U) => unknown ? U : never;
