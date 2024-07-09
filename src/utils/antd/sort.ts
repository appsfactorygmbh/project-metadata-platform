// function getNestedProp(obj: object, prop: string) {
//   return prop.split('.').reduce((o, key) => o && o[key], obj);
// }

type NestedElements = object | string | number | Date | undefined;

type NestedElementsObject = {
  [key: string]: NestedElements;
};

type NestedAnyObject = {
  [key: string]: NestedAnyObject | NestedElements;
};

export function getNestedProp<T extends NestedAnyObject, Prop extends string>(
  obj: T,
  prop: Prop,
): NestedElements {
  return prop
    .split('.')
    .reduce<NestedElements>(
      (o, key) => o && (o as NestedElementsObject)[key],
      obj,
    );
}

export function orderAsc(prop: string) {
  return function (
    a: Record<string, NestedElements>,
    b: Record<string, NestedElements>,
  ) {
    const aValue = getNestedProp(a, prop);
    const bValue = getNestedProp(b, prop);

    if (!aValue && !bValue) return 0;
    else if (aValue && (!bValue || aValue > bValue)) {
      return 1;
    } else if (bValue && (!aValue || aValue < bValue)) {
      return -1;
    }
    return 0;
  };
}

export function orderDesc(prop: string) {
  return function (
    a: Record<string, NestedElements>,
    b: Record<string, NestedElements>,
  ) {
    const aValue = getNestedProp(a, prop);
    const bValue = getNestedProp(b, prop);

    if (!aValue && !bValue) return 0;
    else if (bValue && (!aValue || aValue < bValue)) {
      return 1;
    } else if (aValue && (!bValue || aValue > bValue)) {
      return -1;
    }
    return 0;
  };
}

export const stringSorter = <T>(a: T, b: T, prop: keyof T) =>
  String(a[prop]).localeCompare(String(b[prop]));

export const numberSorter = <T>(
  a: T,
  b: T,
  prop: keyof T,
  type: 'int' | 'float' = 'int',
) => {
  if (!a[prop] && !b[prop]) return 0;
  if (!a[prop]) return 1;
  if (!b[prop]) return -1;
  if (type === 'float')
    return parseFloat(String(a[prop])) - parseFloat(String(b[prop]));
  else return parseInt(String(a[prop])) - parseInt(String(b[prop]));
};

export const dateSorter = <T>(a: T, b: T, prop: keyof T) =>
  Date.parse(String(a[prop])) - Date.parse(String(b[prop]));
