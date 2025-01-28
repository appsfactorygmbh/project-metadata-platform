import { defineStore } from 'pinia';
import { type Ref, ref } from 'vue';
import _ from 'lodash';
import type { AnyObject } from 'ant-design-vue/es/_util/type';

type SearchStoreState<T> = {
  searchQuery: string;
  searchResults: Ref<T[]>;
  baseSet: Ref<T[]>;
  isLoading: boolean;
  onReset: (() => void) | undefined;
  filter: (items: T[]) => T[];
};

const excludedKeys = ['id', 'createdAt', 'updatedAt'];

export const useSearchStore = <T extends AnyObject>(name: string) =>
  defineStore(`${name}_search`, {
    state: (): SearchStoreState<T> => {
      return {
        searchQuery: '',
        baseSet: ref<T[]>([]) as Ref<T[]>,
        searchResults: ref<T[]>([]) as Ref<T[]>,
        isLoading: false,
        onReset: undefined,
        filter: (items: T[]): T[] => items,
      };
    },
    getters: {
      getSearchQuery(): string {
        return this.searchQuery;
      },
      getSearchResults(): T[] {
        return this.searchResults;
      },
      getIsLoading(): boolean {
        return this.isLoading;
      },
    },
    actions: {
      setSearchQuery(query: string) {
        this.searchQuery = query;
        this.applySearch();
      },
      setBaseSet(baseSet: T[]) {
        if (!_.isArray(baseSet)) {
          throw new Error('Base set must be an array');
        }
        this.baseSet = baseSet;
        this.applySearch();
      },
      setFilter(filter: (items: T[]) => T[]) {
        if (!_.isFunction(filter)) {
          throw new Error('Filter must be a function');
        }
        this.filter = filter;
        this.applySearch();
      },
      applySearch() {
        if (_.isEmpty(this.baseSet)) {
          this.searchResults = [];
          return;
        }

        this.isLoading = true;
        let searchResults: T[] = [];
        if (_.isEmpty(this.searchQuery)) {
          // filter only by custom filter function
          searchResults = this.baseSet;
        } else {
          const keys = Object.keys(this.baseSet[0]).filter(
            (key) => !excludedKeys.includes(key),
          );
          // filter by search query
          searchResults = this.baseSet.filter((item) =>
            keys.some((key) =>
              String(item[key as keyof T])
                .toLowerCase()
                .includes(this.searchQuery.toLowerCase()),
            ),
          );
        }
        // filter by custom filter function
        const filteredResults = this.filter(searchResults);
        this.searchResults = filteredResults;
        this.isLoading = false;
      },

      setOnReset(onReset: () => void) {
        this.onReset = onReset;
      },

      reset() {
        this.searchQuery = '';
        this.searchResults = this.baseSet;
        if (this.onReset) this.onReset();
      },
    },
  })();

type SearchStore<T extends AnyObject = AnyObject> = ReturnType<
  typeof useSearchStore<T>
>;
export type { SearchStore };
