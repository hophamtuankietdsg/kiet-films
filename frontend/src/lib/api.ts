import { TVShow } from '@/types/tvShow';
import { API_URL } from './constants';
import { Movie } from '@/types/movie';

export async function getRatedMovies(): Promise<Movie[]> {
  try {
    const baseUrl = API_URL.production;

    const res = await fetch(`${baseUrl}/api/movies/rated`, {
      method: 'GET',
      next: {
        revalidate: 3600,
      },
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

export async function toggleMovieVisibility(
  movieId: number
): Promise<{ isHidden: boolean }> {
  try {
    const baseUrl = API_URL.production;
    const res = await fetch(`${baseUrl}/api/movies/${movieId}/visibility`, {
      method: 'PATCH',
      headers: {
        Accept: 'application/json',
      },
    });

    if (!res.ok) {
      throw new Error('Failed to toggle movie visibility');
    }

    return await res.json();
  } catch (error) {
    console.error('Error toggleing movie visibility', error);
    throw error;
  }
}

export async function getRatedTVShows(): Promise<TVShow[]> {
  try {
    const baseUrl = API_URL.production;

    const res = await fetch(`${baseUrl}/api/tvshows/rated`, {
      method: 'GET',
      next: {
        revalidate: 3600,
      },
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
    console.error('Error fetching rated TV Shows:', error);
    return [];
  }
}
