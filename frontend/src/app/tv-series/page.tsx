'use client';

import { getRatedTVShows } from '@/lib/api';
import TVShowGrid from '@/components/tv-series/TVShowGrid';
import { useEffect, useState } from 'react';
import MovieFilters from '@/components/MovieFilters';
import { TVShow } from '@/types/tvShow';

export const dynamic = 'force-dynamic';

export default function TVSeriesPage() {
  const [tvShows, setTVShows] = useState<TVShow[]>([]);
  const [filteredTVShows, setFilteredTVShows] = useState<TVShow[]>([]);
  const [searchQuery, setSearchQuery] = useState('');
  const [sortBy, setSortBy] = useState('default');

  useEffect(() => {
    const fetchTVShows = async () => {
      const data = await getRatedTVShows();
      console.log('TV Shows data:', data);
      const visibleTVShows = data.filter((show) => !show.isHidden);
      setTVShows(visibleTVShows);
      setFilteredTVShows(visibleTVShows);
    };
    fetchTVShows();
  }, []);

  useEffect(() => {
    let result = [...tvShows];

    // Apply search filter
    if (searchQuery) {
      result = result.filter((show) =>
        show.name.toLowerCase().includes(searchQuery.toLowerCase())
      );
    }

    // Apply sorting
    if (sortBy !== 'default') {
      result.sort((a, b) => {
        switch (sortBy) {
          case 'rating-desc':
            return b.rating - a.rating;
          case 'rating-asc':
            return a.rating - b.rating;
          default:
            return 0;
        }
      });
    }

    setFilteredTVShows(result);
  }, [tvShows, searchQuery, sortBy]);

  return (
    <div className="container mx-auto py-8 sm:px-6 lg:px-8">
      <h1 className="text-3xl font-bold mb-8 text-center">My Rated TV Shows</h1>
      <MovieFilters
        searchQuery={searchQuery}
        onSearchChange={setSearchQuery}
        sortBy={sortBy}
        onSortChange={setSortBy}
      />
      <TVShowGrid tvShows={filteredTVShows} />
    </div>
  );
}
