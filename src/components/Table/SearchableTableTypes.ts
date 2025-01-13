import type { ArrayElement } from '@/models/utils';
import type { ColumnsType } from 'ant-design-vue/es/table';

export type SearchableColumn<
  T = Record<string, string | number | Date | undefined>,
> = ArrayElement<ColumnsType<T>> & {
  dataIndex: string;
  hidden?: boolean;
  searchable?: boolean;
  sortMethod?: 'string' | 'number';
};

export type SearchableColumns = SearchableColumn[];
