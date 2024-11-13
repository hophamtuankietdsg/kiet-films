'use client';

import { useState, useEffect } from 'react';
import MovieFilters from './MovieFilters';
import MovieGrid from './MovieGrid';
import TVShowGrid from './tv-series/TVShowGrid';
import type { Movie } from '@/types/movie';
import type { TVShow } from '@/types/tvShow';

interface MediaListClientProps {
  items: (Movie | TVShow)[];
  type: 'movie' | 'tv';
  isLoading?: boolean;
}

export default function MediaListClient({
  items,
  type,
  isLoading = false,
}: MediaListClientProps) {
  const [filteredItems, setFilteredItems] = useState(items);
  const [searchQuery, setSearchQuery] = useState('');
  const [sortBy, setSortBy] = useState('release-desc');

  useEffect(() => {
    let result = [...items];

    // Search
    if (searchQuery) {
      result = result.filter((item) => {
        const title =
          type === 'movie' ? (item as Movie).title : (item as TVShow).name;
        return title.toLowerCase().includes(searchQuery.toLowerCase());
      });
    }

    // Sort
    result.sort((a, b) => {
      const parseDate = (dateStr: string) => {
        const [day, month, year] = dateStr.split('/');
        return new Date(parseInt(year), parseInt(month) - 1, parseInt(day));
      };

      const getDate = (item: Movie | TVShow) =>
        type === 'movie'
          ? (item as Movie).releaseDate
          : (item as TVShow).firstAirDate;

      switch (sortBy) {
        case 'release-desc':
          return (
            parseDate(getDate(b)).getTime() - parseDate(getDate(a)).getTime()
          );
        case 'release-asc':
          return (
            parseDate(getDate(a)).getTime() - parseDate(getDate(b)).getTime()
          );
        case 'rating-desc':
          return b.rating - a.rating;
        case 'rating-asc':
          return a.rating - b.rating;
        case 'review-date':
          return (
            parseDate(b.reviewDate).getTime() -
            parseDate(a.reviewDate).getTime()
          );
        default:
          return (
            parseDate(getDate(b)).getTime() - parseDate(getDate(a)).getTime()
          );
      }
    });

    setFilteredItems(result);
  }, [items, searchQuery, sortBy, type]);

  return (
    <>
      <MovieFilters
        searchQuery={searchQuery}
        onSearchChange={setSearchQuery}
        sortBy={sortBy}
        onSortChange={setSortBy}
      />
      {type === 'movie' ? (
        <MovieGrid movies={filteredItems as Movie[]} isLoading={isLoading} />
      ) : (
        <TVShowGrid tvShows={filteredItems as TVShow[]} isLoading={isLoading} />
      )}
    </>
  );
}
