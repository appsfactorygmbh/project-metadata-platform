import { defineStore } from 'pinia';
import { ref, type Ref } from 'vue';
import _ from 'lodash';

type SearchStoreState<T> = {
  searchQuery: string;
  searchResults: Ref<T[]>;
  baseSet: Ref<T[]>;
  isLoading: boolean;
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
        _.debounce(this.applySearch, 300);
      },
      setBaseSet(baseSet: T[]) {
        this.baseSet = baseSet;
        _.debounce(this.applySearch, 300);
      },
      applySearch() {
        if (_.isEmpty(this.baseSet)) return;
        this.isLoading = true;
        const keys = Object.keys(this.baseSet[0]);
        const results = this.baseSet.filter((item) =>
          keys.some((key) => {
            if (excludedKeys.includes(key)) return false;
            String(item[key as keyof T])
              .toLowerCase()
              .includes(this.searchQuery.toLowerCase());
          }),
        );
        this.searchResults = results;
        this.isLoading = false;
      },
    },
  })();

type SearchStore = ReturnType<typeof useSearchStore>;
export type { SearchStore };
