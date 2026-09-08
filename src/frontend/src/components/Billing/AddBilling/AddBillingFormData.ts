import type { Currencies } from '@/api/generated';
import type { TimeFrame } from '@/api/generated/models/TimeFrame';

export type AddBillingFormData = {
  billingId: number | undefined;
  contractIds: string[];
  currency: Currencies | undefined;
  budgetLimit: number | undefined;
  hostingFee: number | undefined;
  targetMargin: number | undefined;
  timeFrame: TimeFrame | undefined;
  date: Date | undefined;
  notes: string | undefined;
  inputsDisabled: boolean;
};
