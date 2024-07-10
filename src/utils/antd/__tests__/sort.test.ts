import {
  orderAsc,
  orderDesc,
  getNestedProp,
  numberSorter,
  stringSorter,
  dateSorter,
} from '../sort';
import { describe, assert, it } from 'vitest';

describe('utilts/antd/sort.ts', function () {
  const arr = [{ a: 2 }, { a: 1 }, { a: 3 }, { a: 4 }, { a: 4 }];

  describe('getNestedProp', function () {
    it('should get nested prop', function () {
      const obj = { a: { b: { c: 1 } } };
      const prop = 'a.b.c';
      const value = getNestedProp(obj, prop);
      assert.equal(value, 1);
    });
  });

  describe('orderAsc', function () {
    it('should order arr asc', function () {
      arr.sort(orderAsc('a'));
      assert.deepEqual(arr, [{ a: 1 }, { a: 2 }, { a: 3 }, { a: 4 }, { a: 4 }]);
    });
    it('should order undefined values to the start', function () {
      const arr = [
        { a: 2 },
        { a: 1 },
        { a: 3 },
        { a: undefined },
        { a: 4 },
        { a: 4 },
      ];
      arr.sort(orderAsc('a'));
      assert.deepEqual(arr, [
        { a: undefined },
        { a: 1 },
        { a: 2 },
        { a: 3 },
        { a: 4 },
        { a: 4 },
      ]);
    });
  });

  describe('orderDesc', function () {
    it('should order arr desc', function () {
      arr.sort(orderDesc('a'));
      assert.deepEqual(arr, [{ a: 4 }, { a: 4 }, { a: 3 }, { a: 2 }, { a: 1 }]);
    });

    it('should order undefined values to the end', function () {
      const arr = [
        { a: 2 },
        { a: 1 },
        { a: 3 },
        { a: undefined },
        { a: 4 },
        { a: 4 },
      ];
      arr.sort(orderDesc('a'));
      assert.deepEqual(arr, [
        { a: 4 },
        { a: 4 },
        { a: 3 },
        { a: 2 },
        { a: 1 },
        { a: undefined },
      ]);
    });
  });

  describe('stringSorter', function () {
    it('should order string values', function () {
      const arr = [{ a: 'c' }, { a: 'a' }, { a: 'b' }];
      arr.sort((a, b) => stringSorter(a, b, 'a'));
      assert.deepEqual(arr, [{ a: 'a' }, { a: 'b' }, { a: 'c' }]);
    });

    it('should order undefined values to the end', function () {
      const arr = [
        { a: 'c' },
        { a: 'a' },
        { a: 'b' },
        { a: undefined },
        { a: 'd' },
        { a: 'd' },
      ];
      arr.sort((a, b) => stringSorter(a, b, 'a'));
      assert.deepEqual(arr, [
        { a: 'a' },
        { a: 'b' },
        { a: 'c' },
        { a: 'd' },
        { a: 'd' },
        { a: undefined },
      ]);
    });
  });

  describe('numberSorter', function () {
    it('should order int values', function () {
      const arr = [{ a: 3 }, { a: 1 }, { a: 2 }];
      arr.sort((a, b) => numberSorter(a, b, 'a', 'int'));
      assert.deepEqual(arr, [{ a: 1 }, { a: 2 }, { a: 3 }]);
    });

    it('should order undefined values to the end', function () {
      const arr = [
        { a: 3 },
        { a: 1 },
        { a: 2 },
        { a: undefined },
        { a: 4 },
        { a: 4 },
      ];
      arr.sort((a, b) => numberSorter(a, b, 'a'));
      assert.deepEqual(arr, [
        { a: 1 },
        { a: 2 },
        { a: 3 },
        { a: 4 },
        { a: 4 },
        { a: undefined },
      ]);
    });

    it('should sort float numbers', function () {
      const arr = [{ a: 3.5 }, { a: 1.5 }, { a: 2.5 }];
      arr.sort((a, b) => numberSorter(a, b, 'a', 'float'));
      assert.deepEqual(arr, [{ a: 1.5 }, { a: 2.5 }, { a: 3.5 }]);
    });
  });

  describe('dateSorter', function () {
    it('should order date values', function () {
      const arr = [
        { a: new Date('2020-01-01') },
        { a: new Date('2019-01-01') },
        { a: new Date('2021-01-01') },
      ];
      arr.sort((a, b) => dateSorter(a, b, 'a'));
      assert.deepEqual(arr, [
        { a: new Date('2019-01-01') },
        { a: new Date('2020-01-01') },
        { a: new Date('2021-01-01') },
      ]);
    });
  });
});
