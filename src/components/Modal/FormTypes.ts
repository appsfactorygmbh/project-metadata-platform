import {Form} from "ant-design-vue";

export type FormType = ReturnType<typeof Form.useForm>;
export type FormSubmitType = (fields: (FormType["modelRef"]["value"])) => void;
