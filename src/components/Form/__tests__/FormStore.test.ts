import { Form } from 'ant-design-vue';
import { useFormStore, type FormStore, type RulesObject } from '../FormStore';
import { createPinia, setActivePinia } from 'pinia';
import { flushPromises } from '@vue/test-utils';

describe('FormStore', () => {
  const testForm = {
    name: 'John Doe',
    email: 'john.doe@gmail.com',
  };
  type TestForm = typeof testForm;

  let formStore: FormStore<TestForm>;

  beforeEach(() => {
    setActivePinia(createPinia());
  });

  it('should create a form store', () => {
    formStore = useFormStore<TestForm>('test');
    expect(formStore).toBeDefined();
  });

  it('should detect if the form is defined', () => {
    formStore = useFormStore<TestForm>('test');
    expect(formStore.formIsDefined).toBe(false);
    formStore.setForm(Form.useForm(ref({})));
    expect(formStore.formIsDefined).toBe(true);
  });

  it('should set and get rules', () => {
    formStore = useFormStore<TestForm>('test');
    const rules = {
      name: [{ required: true, message: 'Name is required' }],
      email: [{ required: true, message: 'Email is required' }],
    };
    formStore.setRules(rules);
    expect(formStore.rulesRef).toEqual(rules);
    expect(formStore.form.rulesRef).toEqual(rules);
  });

  it('should set and get the modelRef', () => {
    formStore = useFormStore<TestForm>('test');
    const model = reactive(testForm);
    formStore.setModel(model);
    expect(formStore.modelRef).toEqual(model);
    expect(formStore.form.modelRef).toEqual(model);
  });

  it('should set and get the form', () => {
    const model = reactive(testForm);
    const form = Form.useForm(model);
    formStore = useFormStore<TestForm>('test', form);
    flushPromises().then(() => {
      expect(form).toEqual(form);
    });
  });

  it('should set and get fields', () => {
    formStore = useFormStore<TestForm>('test');
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
    formStore = useFormStore<TestForm>('test');
    const modelRef = reactive(testForm);
    formStore.setModel(modelRef);
    flushPromises().then(() => {
      formStore.resetFields();
      expect(formStore.modelRef).toEqual({});
      expect(formStore.form.modelRef.value).toEqual({});
    });
  });

  it('should update fields', () => {
    formStore = useFormStore<TestForm>('test');
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
    formStore = useFormStore<TestForm>('test');
    const modelRef = reactive(testForm);
    formStore.setModel(modelRef);
    flushPromises().then(() => {
      expect(formStore.getFieldsValue).toEqual({
        name: testForm.name,
        email: testForm.email,
      });
    });
  });

  it('should validate', () => {
    formStore = useFormStore<TestForm>('test');
    const modelRef = reactive(testForm);
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
        expect(validateSpy).toHaveResolved();
        expect(validateSpy).rejects.toThrowError();
        expect(formStore.form.validate).resolves.toBeCalled();
      });
    });

    flushPromises().then(() => {
      formStore.updateField('name', '');

      flushPromises().then(() => {
        expect(formStore.validate()).rejects.toThrowError();
      });
    });
  });
});
