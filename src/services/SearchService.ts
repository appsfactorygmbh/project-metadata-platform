import type { SearchType } from '../models/SearchModel';

export async function searchProjects(
  searchQuery: string = '',
): Promise<SearchType[]> {
  const url = `${import.meta.env.VITE_BACKEND_URL}/Projects?search=${searchQuery}`;
  try {
    
    //Fetching
    console.log('Fetching URL:', url);
    const response = await fetch(url);
    if (!response.ok) {
      throw new Error('Network response was not ok');
    }

    //Response
    const data: SearchType[] = await response.json();
    console.log('Response data:', data);
    return data;
  } catch (err) {
    console.error('Error searching projects:', err);
    return [];
  }
}
