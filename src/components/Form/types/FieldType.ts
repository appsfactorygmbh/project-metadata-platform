export type FieldValue = string | number | boolean | Date | undefined;

export type FieldRecord<T = FieldValue> = Record<string, FieldValue | T>;

export type FieldRecordArray = Array<FieldRecord>;
