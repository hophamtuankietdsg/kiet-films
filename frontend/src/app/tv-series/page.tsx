import { getRatedTVShows } from '@/lib/api';
import MediaListClient from '@/components/MediaListClient';
import { Suspense } from 'react';

export const dynamic = 'force-dynamic';
export const revalidate = 0;

export default async function TVSeriesPage() {
  const tvShows = await getRatedTVShows();
  const visibleTVShows = tvShows.filter((show) => !show.isHidden);

  return (
    <div className="container mx-auto py-8 sm:px-6 lg:px-8">
      <h1 className="text-3xl font-bold mb-8 text-center">My Rated TV Shows</h1>
      <Suspense
        fallback={<MediaListClient items={[]} type="tv" isLoading={true} />}
      >
        <MediaListClient items={visibleTVShows} type="tv" />
      </Suspense>
    </div>
  );
}
