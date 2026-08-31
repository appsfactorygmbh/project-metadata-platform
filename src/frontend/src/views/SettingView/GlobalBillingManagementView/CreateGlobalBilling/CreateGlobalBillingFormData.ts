import type { TimeFrame } from '@/api/generated';

export type CreateGlobalBillingFormData = {
  billingKind: string;
  currency: string | undefined;
  budgetLimit: number | undefined;
  hostingFee: number | undefined;
  targetMargin: number | undefined;
  timeFrame: TimeFrame | undefined;
  inputsDisabled: boolean;
};
