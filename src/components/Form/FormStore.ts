import { defineStore } from 'pinia';
import { type Ref } from 'vue';
import type {
  FieldRecord,
  FormState,
  FormType,
  RulesObject,
  SpecialRulesObject,
} from './types';
import type { FieldData } from 'ant-design-vue/es/form/interface';
import { Form } from 'ant-design-vue';
import type { ArgsType } from '@/models/utils';
import _ from 'lodash';
import { initRef } from '@/utils/store';
import { getValidateInfos } from './validation';

const { useForm } = Form;

type FormStoreState<T extends FormState> = {
  form: Ref<FormType>;
  modelRef: Ref<T>;
  rulesRef: Ref<RulesObject<T>>;
  specialRulesRef: Ref<SpecialRulesObject<T>>;
  specialValidateInfos: Ref<FormType['validateInfos']>;
  onSubmit?: (values: T) => void;
  options?: ArgsType<typeof useForm>[2];
};

export const useFormStore = <T extends FormState>(
  name: string,
  form?: FormType,
  modelRef?: T,
  rulesRef?: RulesObject<T>,
  specialRulesRef?: SpecialRulesObject<T>,
  options?: ArgsType<typeof useForm>[2],
) =>
  defineStore(`${name}_form`, {
    state: (): FormStoreState<T> => ({
      modelRef: initRef(modelRef, {}),
      rulesRef: initRef(rulesRef, {}),
      specialRulesRef: initRef(specialRulesRef, {}),
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
      setSpecialRules(specialRulesRef: SpecialRulesObject<T>) {
        this.specialRulesRef = specialRulesRef;
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
      async validateSpecialRules(): Promise<void> {
        // if (1 == 1) return Promise.resolve();
        let errorsDetected = false;
        for (const key in this.specialRulesRef) {
          const rules = this.specialRulesRef[key];
          if (!rules) continue;
          const value = this.modelRef[key];
          for (const rule of rules) {
            if (rule.ruleTarget === 'field') {
              this.specialValidateInfos[key] = await getValidateInfos(
                rule,
                value,
              );
              if (this.specialValidateInfos[key].validateStatus === 'error')
                errorsDetected = true;
            } else if (rule.ruleTarget === 'arrayItem') {
              const arrayValue = value as FieldRecord[];
              for (const item of arrayValue) {
                console.log('item', item);
                this.specialValidateInfos[
                  item[rule.keyProp] as keyof typeof item
                ] = await getValidateInfos(rule, item);
                if (
                  this.specialValidateInfos[
                    item[rule.keyProp] as keyof typeof item
                  ].validateStatus === 'error'
                )
                  errorsDetected = true;
              }
            }
          }
        }
        return errorsDetected ? Promise.reject(new Error()) : Promise.resolve();
      },
      validate(): Promise<void> {
        this.updateForm();
        return this.validateSpecialRules().then(() => this.form.validate());
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
