import { projectsService } from '@/services/ProjectService.ts';
import type { ProjectType } from '@/models/TableModel';
import { defineStore } from 'pinia';

export const TableStore = defineStore('table', {
  state: () => {
    return {
      table: [] as ProjectType[],
      isLoading: false as boolean,
    };
  },
  getters: {
    getTable(): ProjectType[] {
      return this.table;
    },
  },
  actions: {
    setTable(table: ProjectType[]) {
      this.table = table;
    },
    setLoading(status: boolean) {
      this.isLoading = status;
    },

    async fetchTable() {
      this.setLoading(true);
      const table: ProjectType[] =
        (await projectsService.fetchProjects()) ?? [];
      this.setTable(table);
      this.setLoading(false);
    },
  },
});
