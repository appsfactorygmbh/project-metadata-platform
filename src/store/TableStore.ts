import { projectsService } from '../services/ProjectService';
import { Project } from 'models/TableModel';
import { defineStore } from 'pinia';

export const TableStore = defineStore({
  id: 'table',
  state: () => ({
    table: [] as Project[],
  }),
  getters: {
    getTable():Project[] {
      return this.table
    }
  },
  actions: {
    async fetchTable() {
      this.table = (await projectsService.fetchProjects()) ?? [];
      return this.table;
    },
  },
});
