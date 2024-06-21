export type ButtonModel = {
  name: string;
  onClick: ClickType;
  icon: object;
  disabled: boolean;
  tooltip: string;
};

type ClickType = () => void;
