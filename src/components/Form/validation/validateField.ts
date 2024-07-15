import type { validateInfos } from 'ant-design-vue/es/form/useForm';
import { getValidateInfos } from './getValidateInfos';
import type { CustomRule } from '../types';

export const validateField = async <T extends Record<string | number, unknown>>(
  item: T,
  rule: CustomRule<T>,
) => {
  let errorsDetected: boolean = false;
  const validateInfos: validateInfos = {};
  console.log('item', item);
  // @ts-expect-error type error
  const itemKey = item[rule.keyProp];
  validateInfos[itemKey as keyof validateInfos] = await getValidateInfos(
    rule,
    item,
  );
  if (validateInfos[itemKey as keyof validateInfos].validateStatus === 'error')
    errorsDetected = true;

  return [validateInfos, errorsDetected];
};
