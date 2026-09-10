import type { Currencies, TimeFrame } from '@/api/generated';

export type CreateGlobalBillingFormData = {
  billingKind: string;
  currency: Currencies | undefined;
  targetMargin: number | undefined;
  timeFrame: TimeFrame | undefined;
  inputsDisabled: boolean;
};
