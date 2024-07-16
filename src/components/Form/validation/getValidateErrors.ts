import type { ArrayElement } from '@/models/utils';
import type {
  InternalNamePath,
  ValidateErrorEntity,
} from 'ant-design-vue/es/form/interface';
import type { validateInfos } from 'ant-design-vue/es/form/useForm';

type ValidateError<T = ArrayElement<ValidateErrorEntity['errorFields']>> =
  ValidateErrorEntity<undefined> & {
    errorFields: (T & { warnings: [] })[];
  };

/**
 * Get validate error from validateInfos
 * @param validateInfos
 * @returns ValidateError
 */

export const getValidateErrors = (
  validateInfos: validateInfos,
): ValidateError => {
  const validateError: ValidateError = {
    values: undefined,
    errorFields: [],
    outOfDate: false,
  };
  for (const key in validateInfos) {
    if (validateInfos[key].validateStatus === 'error') {
      validateError.errorFields.push({
        name: key as unknown as InternalNamePath,
        errors: [validateInfos[key].help[0]],
        warnings: [],
      });
    }
  }
  return validateError;
};
