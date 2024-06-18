import type { SearchType } from 'models/SearchModel';

export async function searchProjects(searchQuery: string = ''): Promise<any> {
    try {
        const response = await fetch (
            `${import.meta.env.VITE_BACKEND_URL}/Projects?search=${searchQuery}`
        )
        const data: SearchType[] = await response.json();
        return data;
    } catch (err) {
        console.log('Error searching projects: ' + err);
        return [];
    }
}