import type { Rule } from 'ant-design-vue/es/form';

export type RulesObject<T> = Record<keyof T, Rule[]>;

export type SpecialRule = {
  ruleTarget: 'arrayItem' | 'field';
  keyProp: string | number;
  validator: NonNullable<Rule['validator']>;
  message: Rule['message'];
  required?: Rule['required'];
};

export type SpecialRulesObject<T> = Partial<Record<keyof T, SpecialRule[]>>;
