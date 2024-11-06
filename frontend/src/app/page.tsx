import { getRatedMovies } from '@/lib/api';
import MovieGrid from '@/components/MovieGrid';

export default async function Home() {
  const movies = await getRatedMovies();

  return (
    <div className="container mx-auto py-8 sm:px-6 lg:px-8">
      <h1 className="text-3xl font-bold mb-8 text-center">My Rated Movies</h1>
      <MovieGrid movies={movies} />
    </div>
  );
}
