import { getRatedTVShows } from '@/lib/api';
import MediaListClient from '@/components/MediaListClient';

export const dynamic = 'force-dynamic';

export default async function TVSeriesPage() {
  const tvShows = await getRatedTVShows();
  const visibleTVShows = tvShows.filter((show) => !show.isHidden);

  return (
    <div className="container mx-auto py-8 sm:px-6 lg:px-8">
      <h1 className="text-3xl font-bold mb-8 text-center">My Rated TV Shows</h1>
      <MediaListClient items={visibleTVShows} type="tv" />
    </div>
  );
}
