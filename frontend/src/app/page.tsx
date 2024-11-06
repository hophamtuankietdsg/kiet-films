'use client';

import { getRatedMovies } from '@/lib/api';
import MovieGrid from '@/components/MovieGrid';
import { useEffect, useState } from 'react';
import MovieFilters from '@/components/MovieFilters';
import { Movie } from '@/types/movie';

export const dynamic = 'force-dynamic';

export default function Home() {
  const [movies, setMovies] = useState<Movie[]>([]);
  const [filteredMovies, setFilteredMovies] = useState<Movie[]>([]);
  const [searchQuery, setSearchQuery] = useState('');
  const [sortBy, setSortBy] = useState('default');

  useEffect(() => {
    const fetchMovies = async () => {
      const data = await getRatedMovies();
      setMovies(data);
      setFilteredMovies(data);
    };
    fetchMovies();
  }, []);

  useEffect(() => {
    let result = [...movies];

    // Apply search filter
    if (searchQuery) {
      result = result.filter((movie) =>
        movie.title.toLowerCase().includes(searchQuery.toLowerCase())
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

    setFilteredMovies(result);
  }, [movies, searchQuery, sortBy]);

  return (
    <div className="container mx-auto py-8 sm:px-6 lg:px-8">
      <h1 className="text-3xl font-bold mb-8 text-center">My Rated Movies</h1>
      <MovieFilters
        searchQuery={searchQuery}
        onSearchChange={setSearchQuery}
        sortBy={sortBy}
        onSortChange={setSortBy}
      />
      <MovieGrid movies={filteredMovies} />
    </div>
  );
}
