import { getRatedMovies } from '@/lib/api';
import MediaListClient from '@/components/MediaListClient';

export const dynamic = 'force-dynamic';

export default async function Home() {
  const movies = await getRatedMovies();
  const visibleMovies = movies.filter((movie) => !movie.isHidden);

  return (
    <div className="container mx-auto py-8 sm:px-6 lg:px-8">
      <h1 className="text-3xl font-bold mb-8 text-center">My Rated Movies</h1>
      <MediaListClient items={visibleMovies} type="movie" />
    </div>
  );
}
