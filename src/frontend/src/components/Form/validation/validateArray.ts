import type { validateInfos } from 'ant-design-vue/es/form/useForm';
import type { CustomRule } from '../types';
import { validateField } from './validateField';
import _ from 'lodash';

export const validateArray = async <T extends Record<string | number, unknown>>(
  arrayValue: T[],
  rule: CustomRule<T>,
) => {
  let errorsDetected: boolean = false;
  const validateInfos: validateInfos = {};
  if (!arrayValue || _.isEmpty(arrayValue))
    return [validateInfos, errorsDetected];
  for (const item of arrayValue) {
    const [val, err] = await validateField(item, rule);
    _.merge(validateInfos, val);
    if (err) errorsDetected = true;
  }

  return [validateInfos, errorsDetected];
};
