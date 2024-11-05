import { API_URL } from './constants';
import { Movie } from '@/types/movie';

export async function getRatedMovies(): Promise<Movie[]> {
  try {
    const baseUrl = API_URL.production; // Luôn dùng URL production

    const res = await fetch(`${baseUrl}/api/movies/rated`, {
      method: 'GET',
      cache: 'no-cache',
      headers: {
        Accept: 'application/json',
      },
    });

    if (!res.ok) {
      console.error('Response not OK:', await res.text());
      throw new Error('Failed to fetch rated movies');
    }

    const data = await res.json();
    return data;
  } catch (error) {
    console.error('Error fetching rated movies:', error);
    return [];
  }
}
