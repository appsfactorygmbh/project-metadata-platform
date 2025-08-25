import type { ArrayElement } from '@/models/utils';
import type { Rule } from 'ant-design-vue/es/form';

export type RulesObject<T, RuleType = Rule> = Partial<
  Record<keyof T, RuleType[]>
>;

export type CustomRule<T, Target = 'arrayItem' | 'field' | undefined> = {
  ruleTarget: Target;
  keyProp: T extends Array<unknown> ? keyof ArrayElement<T> : T;
  validator: NonNullable<Rule['validator']>;
  message: Rule['message'];
  required?: Rule['required'];
};

export type CustomRulesObject<T, K extends keyof T = keyof T> = RulesObject<
  T,
  CustomRule<T[K]>
>;
