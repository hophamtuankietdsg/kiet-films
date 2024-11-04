import MovieCard from '@/components/MovieCard';
import { Movie } from '@/types/movie';

async function getRatedMovies(): Promise<Movie[]> {
  const res = await fetch('http://localhost:5012/api/movies/rated', {
    cache: 'no-store',
  });

  if (!res.ok) {
    throw new Error('Failed to fetch rated movies');
  }

  return res.json();
}

export default async function Home() {
  const movies = await getRatedMovies();

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold text-gray-900 dark:text-white mb-8">
        My Rated Movies
      </h1>

      {movies.length === 0 ? (
        <p className="text-gray-600 dark:text-gray-400">
          No rated movies yet. Go to search page to rate some movies!
        </p>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
          {movies.map((movie) => (
            <MovieCard key={movie.id} movie={movie} />
          ))}
        </div>
      )}
    </div>
  );
}
