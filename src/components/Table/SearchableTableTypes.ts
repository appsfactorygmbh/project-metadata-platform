import type { ColumnsType } from 'ant-design-vue/es/table';

type ArrayElement<ArrayType extends readonly unknown[]> =
  ArrayType extends readonly (infer ElementType)[] ? ElementType : never;

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export type SearchableColumn<T = any> = ArrayElement<ColumnsType<T>> & {
  hidden?: boolean;
  searchable?: boolean;
  sortMethod?: 'string' | 'number';
};

export type SearchableColumns = SearchableColumn[];
