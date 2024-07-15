import type { ValidateInfo } from 'ant-design-vue/es/form/useForm';
import type { CustomRule } from '../types';

export const getValidateInfos = async <
  T extends Record<string | number, unknown>,
  Validator extends CustomRule<T>['validator'],
  Args extends Parameters<Validator>,
>(
  rule: CustomRule<T>,
  value: Args[1],
): Promise<ValidateInfo> => {
  const validateObj: ValidateInfo = {
    autoLink: false,
    help: [],
    required: rule.required ?? false,
    validateStatus: 'success',
  };
  try {
    const result = rule.validator(rule, value, () => {});
    console.log('result', result);
    if (result instanceof Promise) {
      return await result
        .then(() => {
          return { ...validateObj };
        })
        .catch((error) => {
          return {
            ...validateObj,
            validateStatus: 'error',
            help: [error ?? rule.message],
          };
        });
    }
    return { ...validateObj };
  } catch (error) {
    return {
      ...validateObj,
      validateStatus: 'error',
      help: [error ?? rule.message],
    };
  }
};
