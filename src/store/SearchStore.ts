import { defineStore } from 'pinia';
import { ref, type Ref } from 'vue';
import _ from 'lodash';
import type { AnyObject } from 'ant-design-vue/es/_util/type';

type SearchStoreState<T> = {
  searchQuery: string;
  searchResults: Ref<T[]>;
  baseSet: Ref<T[]>;
  isLoading: boolean;
  onReset: (() => void) | undefined;
};

const excludedKeys = ['id', 'createdAt', 'updatedAt'];

export const useSearchStore = <T extends object>(name: string) =>
  defineStore(`${name}_search`, {
    state: (): SearchStoreState<T> => {
      return {
        searchQuery: '',
        baseSet: ref<T[]>([]) as Ref<T[]>,
        searchResults: ref<T[]>([]) as Ref<T[]>,
        isLoading: false,
        onReset: undefined,
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
        this.baseSet = baseSet;
        this.applySearch();
      },
      applySearch() {
        if (_.isEmpty(this.baseSet)) return;
        if (_.isEmpty(this.searchQuery)) {
          this.searchResults = this.baseSet;
          return;
        }
        this.isLoading = true;
        const keys = Object.keys(this.baseSet[0]).filter(
          (key) => !excludedKeys.includes(key),
        );
        const results = this.baseSet.filter((item) =>
          keys.some((key) =>
            String(item[key as keyof T])
              .toLowerCase()
              .includes(this.searchQuery.toLowerCase()),
          ),
        );
        this.searchResults = results;
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

type SearchStore<T extends object = AnyObject> = ReturnType<
  typeof useSearchStore<T>
>;
export type { SearchStore };
