import type { FloatButtonProps } from 'ant-design-vue';

export type FloatButtonModel = FloatButtonProps & {
  name: string;
  icon: object;
  size: 'small' | 'middle' | 'large';
  specialType?: 'danger' | 'success';
  color?: { color: string };
  status?: 'activated' | 'disabled' | 'deactivated';
  isLink?: boolean;
  destination?: string;
};
