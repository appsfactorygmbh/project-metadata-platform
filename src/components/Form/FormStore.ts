import { defineStore } from 'pinia';
import { type Ref } from 'vue';
import type {
  FormState,
  FormType,
  RulesObject,
  CustomRulesObject,
} from './types';
import type {
  FieldData,
  ValidateErrorEntity,
} from 'ant-design-vue/es/form/interface';
import { Form } from 'ant-design-vue';
import type { ArgsType } from '@/models/utils';
import _ from 'lodash';
import { initRef } from '@/utils/store';
import { getValidateErrors, validateArray, validateField } from './validation/';

const { useForm } = Form;

type FormStoreState<T extends FormState> = {
  form: Ref<FormType>;
  modelRef: Ref<T>;
  rulesRef: Ref<RulesObject<T>>;
  customRulesRef: Ref<CustomRulesObject<T>>;
  specialValidateInfos: Ref<FormType['validateInfos']>;
  onSubmit?: (values: T) => void;
  options?: ArgsType<typeof useForm>[2];
};

export const useFormStore = <T extends FormState>(
  name: string,
  form?: FormType,
  modelRef?: T,
  rulesRef?: RulesObject<T>,
  customRulesRef?: CustomRulesObject<T>,
  options?: ArgsType<typeof useForm>[2],
) =>
  defineStore(`${name}_form`, {
    state: (): FormStoreState<T> => ({
      modelRef: initRef(modelRef, {}),
      rulesRef: initRef(rulesRef, {}),
      customRulesRef: initRef(customRulesRef, {}),
      specialValidateInfos: initRef({}, {}),
      form: initRef(form, {}),
      options: options,
    }),
    actions: {
      setOnSubmit(onSubmit: (values: T) => void) {
        this.onSubmit = onSubmit;
      },
      setForm(form: FormType) {
        this.form = form;
      },
      updateForm() {
        this.form = useForm(this.modelRef, this.rulesRef, this.options);
      },
      setModel(modelRef: T) {
        this.modelRef = modelRef;
        this.updateForm();
      },
      setRules(rulesRef: RulesObject<T>) {
        this.rulesRef = rulesRef;
        this.updateForm();
      },
      setCustomRules(customRulesRef: CustomRulesObject<T>) {
        this.customRulesRef = customRulesRef;
        this.updateForm();
      },
      setOptions(options: ArgsType<typeof useForm>[2]) {
        this.options = options;
        this.updateForm();
      },
      updateField(key: keyof T, value: T[keyof T]) {
        this.modelRef[key] = value;
        this.updateForm();
      },
      updateFields(fields: FieldData[]) {
        console.log('fields', fields);
        fields.forEach(({ name, value }) => {
          this.modelRef[name as keyof T] = value as T[keyof T];
        });
        this.updateForm();
      },
      resetFields() {
        this.modelRef = {} as T;
        this.updateForm();
      },
      getFieldValue(key: keyof T): T[keyof T] {
        return this.modelRef[key];
      },
      async validateCustomRules(): Promise<void> {
        let errorsDetected = false;
        this.specialValidateInfos = {};
        for (const key in this.customRulesRef) {
          const rules = this.customRulesRef[key];
          if (!rules) continue;
          const value = this.modelRef[key];
          for (const rule of rules) {
            if (!rule.ruleTarget) continue;
            else if (rule.ruleTarget === 'field') {
              if (!value) continue;
              // @ts-expect-error type mismatch
              const [val, err] = await validateField(value, rule);
              _.merge(this.specialValidateInfos, { [key]: val });
              if (err) errorsDetected = true;
            } else if (
              rule.ruleTarget === 'arrayItem' &&
              Array.isArray(value)
            ) {
              const [val, err] = await validateArray(value, rule);
              _.merge(this.specialValidateInfos, val);
              if (err) errorsDetected = true;
            }
          }
        }
        const validateErrors = getValidateErrors(this.specialValidateInfos);
        return errorsDetected
          ? Promise.reject(validateErrors)
          : Promise.resolve();
      },
      validate(): ReturnType<typeof this.form.validate> {
        this.updateForm();
        const validateErrors: Partial<ValidateErrorEntity> = {};
        return this.validateCustomRules()
          .catch((rej) => {
            _.merge(validateErrors, rej);
            return Promise.resolve();
          })
          .then(() => this.form.validate())
          .catch((rej) => {
            validateErrors.outOfDate = rej.outOfDate;
            validateErrors.values = rej.values;
            if (!validateErrors.errorFields) {
              validateErrors.errorFields = [];
            }
            validateErrors.errorFields.push(...rej.errorFields);
            return Promise.reject({ ...validateErrors });
          })
          .then((res) => {
            if (!_.isEmpty(validateErrors)) {
              return Promise.reject({ ...validateErrors });
            }
            return Promise.resolve(res);
          });
      },
      clearValidate() {
        this.form.clearValidate();
      },
      async submit() {
        if (!this.rulesRef) {
          console.warn('Form rules are not defined before submitting.');
        }
        return this.validate().then(() => {
          if (this.onSubmit) {
            return this.onSubmit(this.modelRef);
          }
        });
      },
    },
    getters: {
      getFieldsValue(): T {
        return this.modelRef;
      },
      formIsDefined(): boolean {
        return !!this.form.validate;
      },
      getForm(): FormType {
        return this.form;
      },
      getModel(): T {
        return this.modelRef;
      },
      validateInfos(): FormType['validateInfos'] {
        return _.merge(this.form.validateInfos, this.specialValidateInfos);
      },
    },
  })();
export type FormStore<T extends FormState = FormState> = ReturnType<
  typeof useFormStore<T>
>;
