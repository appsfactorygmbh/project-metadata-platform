import type { FloatButtonProps } from 'ant-design-vue';

export type FloatButtonModel = FloatButtonProps & {
  name: string;
  icon: object;
  specialType?: 'danger' | 'success';
  color?: { color: string };
  status?: 'activated' | 'disabled' | 'deactivated';
};
