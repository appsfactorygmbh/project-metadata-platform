import type { FloatButtonProps } from 'ant-design-vue';

export type FloatButtonModel = FloatButtonProps & {
  name: string;
  icon: object;
  status: 'activated' | 'disabled' | 'deactivated';
};
