import { Form } from 'ant-design-vue';
import { useFormStore, type FormStore } from '../FormStore';
import { createPinia, setActivePinia } from 'pinia';
import { flushPromises } from '@vue/test-utils';
import type { RulesObject } from '../types';

describe('FormStore', () => {
  const testForm = {
    name: 'John Doe',
    email: 'john.doe@gmail.com',
  };
  type TestForm = typeof testForm;

  beforeEach(() => {
    setActivePinia(createPinia());
  });

  it('should create a form store', () => {
    const formStore: FormStore<TestForm> = useFormStore<TestForm>('test');
    expect(formStore).toBeDefined();
  });

  it('should detect if the form is defined', () => {
    const formStore: FormStore<TestForm> = useFormStore<TestForm>('test');
    expect(formStore.formIsDefined).toBe(false);
    const emptyForm = Form.useForm(ref({}));
    formStore.setForm(emptyForm);
    expect(formStore.formIsDefined).toBe(true);
    expect(formStore.getForm).toEqual({
      ...emptyForm,
      modelRef: {},
      rulesRef: {},
    });
  });

  it('should set and get rules', () => {
    const formStore: FormStore<TestForm> = useFormStore<TestForm>('test');
    const rules = {
      name: [{ required: true, message: 'Name is required' }],
      email: [{ required: true, message: 'Email is required' }],
    };
    formStore.setRules(rules);
    expect(formStore.rulesRef).toEqual(rules);
    expect(formStore.form.rulesRef).toEqual(rules);
  });

  it('should set and get the modelRef', () => {
    const formStore: FormStore<TestForm> = useFormStore<TestForm>('test');
    const model = reactive(testForm);
    formStore.setModel(model);
    expect(formStore.modelRef).toEqual(model);
    expect(formStore.form.modelRef).toEqual(model);
    expect(formStore.getModel).toEqual(model);
  });

  it('should set and get the form', () => {
    const model = reactive(testForm);
    const form = Form.useForm(model);
    const formStore: FormStore<TestForm> = useFormStore<TestForm>('test', form);
    flushPromises().then(() => {
      expect(formStore.form).toEqual({
        ...form,
        rulesRef: {},
      });
    });
  });

  it('should set and get fields', () => {
    const formStore: FormStore<TestForm> = useFormStore<TestForm>('test');
    const modelRef = reactive(testForm);
    formStore.setModel(modelRef);
    flushPromises().then(() => {
      formStore.updateField('name', 'John Doe');
      formStore.updateField('email', 'john.doe@gmail.com');
      expect(formStore.getFieldValue('name')).toBe('John Doe');
      expect(formStore.getFieldValue('email')).toBe('john.doe@gmail.com');
    });
  });

  it('should reset fields', () => {
    const formStore: FormStore<TestForm> = useFormStore<TestForm>('test');
    const modelRef = reactive(testForm);
    formStore.setModel(modelRef);
    flushPromises().then(() => {
      formStore.resetFields();
      expect(formStore.modelRef).toEqual({});
      // useForm sets the modelRef undefined if it's empty
      expect(formStore.form.modelRef.value).toEqual(undefined);
    });
  });

  it('should update fields', () => {
    const formStore: FormStore<TestForm> = useFormStore<TestForm>('test');
    const modelRef = reactive(testForm);
    formStore.setModel(modelRef);
    flushPromises().then(() => {
      formStore.updateFields([
        { name: 'name', value: testForm.name },
        { name: 'email', value: testForm.email },
      ]);
      expect(formStore.getFieldValue('name')).toBe(testForm.name);
      expect(formStore.getFieldValue('email')).toBe(testForm.email);
    });
  });

  it('should get fields value', () => {
    const formStore: FormStore<TestForm> = useFormStore<TestForm>('test');
    const modelRef = reactive(testForm);
    formStore.setModel(modelRef);
    flushPromises().then(() => {
      expect(formStore.getFieldsValue).toEqual({
        name: testForm.name,
        email: testForm.email,
      });
    });
  });

  it('should validate and resolve correct forms', () => {
    const formStore: FormStore<TestForm> = useFormStore<TestForm>('test');
    const modelRef = reactive(testForm);
    const rules: RulesObject<TestForm> = reactive({
      name: [{ required: true, message: 'Name is required' }],
      email: [{ required: true, message: 'Email is required' }],
    });
    formStore.setModel(modelRef);
    formStore.setRules(rules);

    const validateSpy = vi.fn(formStore.form.validate);

    flushPromises().then(() => {
      validateSpy();
      flushPromises().then(() => {
        expect(validateSpy).toHaveResolved();
        expect(validateSpy()).resolves.toStrictEqual(testForm);
      });
    });
  });

  it('should validate and reject incorrect forms', () => {
    const formStore: FormStore<TestForm> = useFormStore<TestForm>('test');
    const modelRef = reactive({ name: '', email: testForm.email });
    const rules: RulesObject<TestForm> = reactive({
      name: [{ required: true, message: 'Name is required' }],
      email: [{ required: true, message: 'Email is required' }],
    });
    formStore.setModel(modelRef);
    formStore.setRules(rules);

    const validateSpy = vi.fn(() => formStore.form.validate());

    flushPromises().then(() => {
      validateSpy();
      flushPromises().then(() => {
        expect(validateSpy).not.toHaveResolved();
        expect(validateSpy).rejects.toStrictEqual({
          errorFields: [
            {
              errors: ['Name is required'],
              name: 'name',
              warnings: [],
            },
          ],
          outOfDate: false,
          values: {
            email: 'john.doe@gmail.com',
            name: '',
          },
        });
      });
    });
  });

  it("should set the onSubmit callback and call it when the form is submitted and it's valid", async () => {
    const formStore: FormStore<TestForm> = useFormStore<TestForm>('test');
    const modelRef = reactive(testForm);
    formStore.setModel(modelRef);
    const onSubmit = vi.fn(() => 0);
    formStore.setOnSubmit(onSubmit);
    const submitSpy = vi.fn(formStore.submit);

    await flushPromises();

    expect(formStore.onSubmit).toBe(onSubmit);
    expect(formStore.form).toBeDefined();
    expect(onSubmit).not.toHaveBeenCalled();

    submitSpy();
    expect(submitSpy).not.toThrowError();
    expect(submitSpy).toReturnWith(Promise.resolve());

    await flushPromises();

    expect(onSubmit).toHaveBeenCalled();
    expect(onSubmit).toHaveBeenCalledWith(testForm);
  });
});
