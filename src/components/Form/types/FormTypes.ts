import { Form } from 'ant-design-vue';
import type { FieldRecord, FieldRecordArray } from './FieldType';

// ReturnType of the useForm function from ant-design-vue
export type FormType = ReturnType<typeof Form.useForm>;

export type FormSubmitType = (fields: FormType['modelRef']['value']) => void;

export type FormState = FieldRecord<FieldRecordArray>;
