import { defineStore } from 'pinia';
import { ref, type Ref } from 'vue';
import type { FormType } from './types';
import type { FieldData } from 'ant-design-vue/es/form/interface';
import { Form } from 'ant-design-vue';
import type { Rule } from 'ant-design-vue/es/form';
import _ from 'lodash';
import type { ArgsType } from '@/utils/types';

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
      setRules(rulesRef: RulesObject<T>) {
        this.rulesRef = rulesRef;
        if (this.formIsDefined) {
          this.form.rulesRef.value = rulesRef;
        } else {
          this.form = useForm(this.modelRef, rulesRef);
        }
        console.log('rules', this.form.rulesRef);
      },
      // setOptions(options: ArgsType<typeof useForm>[2]) {
      //   this.options = options;
      //   if (this.formIsDefined) {
      //     this.form.options = options;
      //   } else {
      //     this.form = useForm(this.modelRef, this.rulesRef, options);
      //   }
      // },
      updateField(key: keyof T, value: T[keyof T]) {
        this.modelRef[key] = value;
        this.form.modelRef = this.modelRef;
      },
      updateFields(fields: FieldData[]) {
        console.log('fields', fields);
        fields.forEach(({ name, value }) => {
          this.modelRef[name as keyof T] = value as T[keyof T];
        });
        this.form.modelRef = this.modelRef;
      },
      resetFields() {
        this.modelRef = {} as T;
        this.form.modelRef.value = {} as T;
      },
      getFieldValue(key: keyof T): T[keyof T] {
        return this.modelRef[key];
      },
      validate(): Promise<void> {
        this.form = useForm(this.modelRef, this.rulesRef);
        // this.form.modelRef = this.modelRef;
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
