import type { ProjectModel } from '@/models/Project';
import type { ArrayElement } from '@/models/utils';
import type { ColumnsType } from 'ant-design-vue/es/table';

export type SearchableColumn<
  T = Record<string, string | number | Date | ProjectModel | undefined>,
> = ArrayElement<ColumnsType<T>> & {
  dataIndex: string;
  hidden?: boolean;
  searchable?: boolean;
  sortMethod?: 'string' | 'number';
  hasTags?: boolean;
  getTagColor?: (record: T) => string;
};

export type SearchableColumns = SearchableColumn[];
