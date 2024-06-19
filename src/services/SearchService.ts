import type { SearchType } from '../models/SearchModel';

export async function searchProjects(
  searchQuery: string = '',
): Promise<SearchType[]> {
  try {
    const url = `${import.meta.env.VITE_BACKEND_URL}/Projects?search=${searchQuery}`;

    //Fetching
    console.log('Fetching URL:', url);
    const response = await fetch(url);

    //Response
    const data: SearchType[] = await response.json();
    console.log('Response data:', data);
    return data;
    
  } catch (err) {
    console.log('Error searching projects: ' + err);
    return [];
  }
}
