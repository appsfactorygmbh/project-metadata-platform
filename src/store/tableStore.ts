import { tableService } from '../services/TableService';
import { TableEntry } from 'models/TableModel';
import { defineStore } from 'pinia';

export const TableStores = defineStore({
  id: 'table',
  state: () => ({
    table: [] as TableEntry[],
  }),
  actions: {
    async getTable() {
      this.table = (await tableService.fetchProjects()) ?? [];
      return this.table;
    },
  },
});
