import { projectsService } from '../services/TableService';
import { Project } from 'models/TableModel';
import { defineStore } from 'pinia';

export const TableStore = defineStore({
  id: 'table',
  state: () => ({
    table: [] as Project[],
  }),
  actions: {
    async getTable() {
      this.table = (await projectsService.fetchProjects()) ?? [];
      return this.table;
    },
  },
});
