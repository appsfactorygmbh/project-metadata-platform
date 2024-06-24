import type { FloatButtonProps } from 'ant-design-vue';

export type FloatButtonModel = FloatButtonProps & {
  name: string;
  disabled: boolean;
  deactivated: boolean;
};
