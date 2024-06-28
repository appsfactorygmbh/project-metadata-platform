import type { ColumnsType } from 'ant-design-vue/es/table';

type ArrayElement<ArrayType extends readonly unknown[]> =
  ArrayType extends readonly (infer ElementType)[] ? ElementType : never;

export type SearchableColumn<T = object> = ArrayElement<ColumnsType<T>> & {
  hidden?: boolean;
  searchable?: boolean;
  sortable?: boolean;
};

export type SearchableColumns = SearchableColumn[];
