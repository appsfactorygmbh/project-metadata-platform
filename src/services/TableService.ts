import { TableEntry } from "../models/TableModel";

class TableService {
    fetchProjects = async () => {
        try {
            const response = await fetch(import.meta.env.VITE_BACKEND_URL + "/projects");
        
            const data: TableEntry[] = await response.json();
            console.log(data);
            
            return data;
        
        } catch (err) {
            console.error('Error fetching data: ' + err);
        }
    }
}

const tableService = new TableService();
export {tableService};