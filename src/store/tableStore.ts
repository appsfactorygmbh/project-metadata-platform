import { tableService } from "../services/TableService";
import { TableEntry } from "../models/TableModel";
import { defineStore } from "pinia";

export const TableStores = defineStore({
    id: "table",
    state: () => ({
        table: [] as TableEntry[]
    }),
    getters: {
        
    },
    actions: {
        getTable(): TableEntry[] {
            return this.table;
        },
        setTable(table: TableEntry[]) {
            this.table = table;
        },
        async fetchTable () {
            this.setTable(await tableService.fetchProjects() ?? []);
        },
    },

})

class TableStore {
    currentTable: TableEntry[] = [];

    getTable = async () => {
        this.currentTable = await tableService.fetchProjects() ?? [];
        return this.currentTable;
    };
}

const tableStore = new TableStore();
export {tableStore};