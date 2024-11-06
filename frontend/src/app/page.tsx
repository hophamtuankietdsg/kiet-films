import { getRatedMovies } from '@/lib/api';
import MovieGrid from '@/components/MovieGrid';

export default async function Home() {
  const movies = await getRatedMovies();

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-4xl font-bold text-primary mb-8">My Rated Movies</h1>
      <MovieGrid movies={movies} />
    </div>
  );
}
