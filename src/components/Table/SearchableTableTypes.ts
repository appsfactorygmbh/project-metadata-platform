import type { ProjectModel } from '@/models/Project';
import type { ColumnsType } from 'ant-design-vue/es/table';

type ArrayElement<ArrayType extends readonly unknown[]> =
  ArrayType extends readonly (infer ElementType)[] ? ElementType : never;

export type SearchableColumn<T = ProjectModel> = ArrayElement<
  ColumnsType<T>
> & {
  dataIndex: string;
  hidden?: boolean;
  searchable?: boolean;
  sortMethod?: 'string' | 'number';
};

export type SearchableColumns = SearchableColumn[];
