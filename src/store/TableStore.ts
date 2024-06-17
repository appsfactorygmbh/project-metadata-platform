import { projectsService } from '../services/TableService';
import type { Project } from '@/models/TableModel';
import { defineStore } from 'pinia';

export const TableStore = defineStore('table', {
  state: () => {
    return {
      table: [] as Project[],
      isLoading: false as boolean,
    };
  },
  getters: {
    getTable(): Project[] {
      return this.table;
    },
  },
  actions: {
    setTable(table: Project[]) {
      this.table = table;
    },
    setLoading(status: boolean) {
      this.isLoading = status;
    },

    async fetchTable() {
      this.setLoading(true);
      const table = (await projectsService.fetchProjects()) ?? [];
      this.setTable(table);
      this.setLoading(false);
    },
  },
});
