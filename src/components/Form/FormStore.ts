import { defineStore } from 'pinia';
import { ref, type Ref } from 'vue';
import type { FormType } from './types';
import type { FieldData, RuleObject } from 'ant-design-vue/es/form/interface';
import { Form } from 'ant-design-vue';
import _ from 'lodash';

const { useForm } = Form;

type FieldValue = string | number | boolean | Date | undefined;

type FieldRecord<T> = Record<string, FieldValue | T>;

type AnyArray = Array<FieldRecord<FieldValue>>;

type FormState = FieldRecord<AnyArray>;

type FormStoreState<T extends FormState> = {
  form: Ref<FormType>;
  modelRef: Ref<T>;
  rulesRef: Ref<Record<string, RuleObject | RuleObject[]>>;
  onSubmit?: (values: T) => void;
};

export type RulesObject<T> = Record<keyof T, RuleObject | RuleObject[]>;

export const useFormStore = <T extends FormState>(
  name: string,
  form?: FormType,
  modelRef?: T,
  rulesRef?: RulesObject<T>,
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
    }),
    actions: {
      setOnSubmit(onSubmit: (values: T) => void) {
        this.onSubmit = onSubmit;
      },
      setForm(form: FormType) {
        this.form = form;
      },
      setModel(modelRef: T) {
        this.modelRef = modelRef;
        if (this.formIsDefined) {
          this.form.modelRef.value = modelRef;
        } else if (_.keys(this.rulesRef).length > 0) {
          this.form = useForm(modelRef, this.rulesRef);
        } else {
          this.form = useForm(modelRef);
        }
      },
      setRules(rulesRef: Record<string, RuleObject | RuleObject[]>) {
        this.rulesRef = rulesRef;
        if (this.formIsDefined) {
          this.form.rulesRef.value = rulesRef;
        } else {
          this.form = useForm(this.modelRef, rulesRef);
        }
        console.log('rules', this.form.rulesRef);
      },
      updateField(key: keyof T, value: T[keyof T]) {
        this.modelRef[key] = value;
        this.form.modelRef.value[key] = value;
      },
      updateFields(fields: FieldData[]) {
        console.log('fields', fields);
        fields.forEach(({ name, value }) => {
          this.modelRef[name as keyof T] = value as T[keyof T];
          this.form.modelRef.value[name as keyof T] = value;
        });
      },
      resetFields() {
        this.modelRef = {} as T;
        this.form.modelRef.value = {} as T;
      },
      getFieldValue(key: keyof T): T[keyof T] {
        return this.modelRef[key];
      },
      validate(): Promise<void> {
        this.form.modelRef.value = this.modelRef;
        return this.form.validate();
      },
      submit() {
        form?.validate().then(() => {
          if (this.onSubmit) {
            this.onSubmit(this.modelRef);
          }
        });
      },
    },
    getters: {
      getFieldsValue(): T {
        return this.modelRef;
      },
      state(): T {
        return this.form.modelRef.value;
      },
      formIsDefined(): boolean {
        return !!this.form.validate;
      },
      getForm(): FormType {
        return this.form;
      },
      getRef(): T {
        return this.modelRef;
      },
    },
  })();
export type FormStore<T extends FormState = FormState> = ReturnType<
  typeof useFormStore<T>
>;
