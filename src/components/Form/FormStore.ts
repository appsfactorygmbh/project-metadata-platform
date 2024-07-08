import { defineStore } from 'pinia';
import { ref, type Ref } from 'vue';
import type { FormType } from './types';
import type { FieldData } from 'ant-design-vue/es/form/interface';
import { Form } from 'ant-design-vue';
import type { Rule } from 'ant-design-vue/es/form';
import type { ArgsType } from '@/models/utils';

const { useForm } = Form;

type FieldValue = string | number | boolean | Date | undefined;

type FieldRecord<T> = Record<string, FieldValue | T>;

type AnyArray = Array<FieldRecord<FieldValue>>;

type FormState = FieldRecord<AnyArray>;

export type RulesObject<T> = Record<keyof T, Rule[]>;

type FormStoreState<T extends FormState> = {
  form: Ref<FormType>;
  modelRef: Ref<T>;
  rulesRef: Ref<RulesObject<T>>;
  onSubmit?: (values: T) => void;
  options?: ArgsType<typeof useForm>[2];
};

export const useFormStore = <T extends FormState>(
  name: string,
  form?: FormType,
  modelRef?: T,
  rulesRef?: RulesObject<T>,
  options?: ArgsType<typeof useForm>[2],
) =>
  defineStore(`${name}_form`, {
    state: (): FormStoreState<T> => ({
      modelRef: modelRef
        ? (ref<T>(modelRef) as Ref<T>)
        : (ref<T>({} as T) as Ref<T>),
      rulesRef: rulesRef
        ? (ref<RulesObject<T>>(rulesRef) as Ref<RulesObject<T>>)
        : (ref<RulesObject<T>>({} as RulesObject<T>) as Ref<RulesObject<T>>),
      form: form
        ? (ref<FormType>(form) as Ref<FormType>)
        : (ref<FormType>({} as FormType) as Ref<FormType>),
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
      validate(): Promise<void> {
        this.updateForm();
        return this.form.validate();
      },
      clearValidate() {
        this.form.clearValidate();
      },
      async submit() {
        if (!this.rulesRef) {
          console.warn('Form rules are not defined before submitting.');
        }
        return this.form.validate().then(() => {
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
        return this.form.validateInfos;
      },
    },
  })();
export type FormStore<T extends FormState = FormState> = ReturnType<
  typeof useFormStore<T>
>;
