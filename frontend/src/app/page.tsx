import { getRatedMovies } from '@/lib/api';
import MovieGrid from '@/components/MovieGrid';
import { Suspense } from 'react';

export const dynamic = 'force-dynamic';

export default async function Home() {
  const movies = await getRatedMovies();

  return (
    <div className="container mx-auto py-8 sm:px-6 lg:px-8">
      <h1 className="text-3xl font-bold mb-8 text-center">My Rated Movies</h1>
      <Suspense fallback={<div>Loading...</div>}>
        <MovieGrid movies={movies} />
      </Suspense>
    </div>
  );
}
