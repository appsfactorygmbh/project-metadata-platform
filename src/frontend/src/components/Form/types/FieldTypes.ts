export type FieldValue = string | number | boolean | Date | undefined;

export type FieldRecord<T = FieldValue> = Record<
  string | number,
  FieldValue | T
>;

export type FieldRecordArray = Array<FieldRecord>;
