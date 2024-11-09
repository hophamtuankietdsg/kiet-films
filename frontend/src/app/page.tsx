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
  const [sortBy, setSortBy] = useState('release-desc');

  useEffect(() => {
    const fetchMovies = async () => {
      const data = await getRatedMovies();
      const visibleMovie = data.filter((movie) => !movie.isHidden);
      setMovies(visibleMovie);
      setFilteredMovies(visibleMovie);
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
    result.sort((a, b) => {
      switch (sortBy) {
        case 'release-desc': {
          const [dayA, monthA, yearA] = a.releaseDate.split('/');
          const [dayB, monthB, yearB] = b.releaseDate.split('/');
          const dateA = new Date(
            parseInt(yearA),
            parseInt(monthA) - 1,
            parseInt(dayA)
          );
          const dateB = new Date(
            parseInt(yearB),
            parseInt(monthB) - 1,
            parseInt(dayB)
          );
          return dateB.getTime() - dateA.getTime();
        }
        case 'release-asc': {
          const [dayA, monthA, yearA] = a.releaseDate.split('/');
          const [dayB, monthB, yearB] = b.releaseDate.split('/');
          const dateA = new Date(
            parseInt(yearA),
            parseInt(monthA) - 1,
            parseInt(dayA)
          );
          const dateB = new Date(
            parseInt(yearB),
            parseInt(monthB) - 1,
            parseInt(dayB)
          );
          return dateA.getTime() - dateB.getTime();
        }
        case 'rating-desc':
          return b.rating - a.rating;
        case 'rating-asc':
          return a.rating - b.rating;
        case 'review-date': {
          const [dayA, monthA, yearA] = a.reviewDate.split('/');
          const [dayB, monthB, yearB] = b.reviewDate.split('/');
          const dateA = new Date(
            parseInt(yearA),
            parseInt(monthA) - 1,
            parseInt(dayA)
          );
          const dateB = new Date(
            parseInt(yearB),
            parseInt(monthB) - 1,
            parseInt(dayB)
          );
          return dateB.getTime() - dateA.getTime();
        }
        default: {
          const [dayA, monthA, yearA] = a.releaseDate.split('/');
          const [dayB, monthB, yearB] = b.releaseDate.split('/');
          const dateA = new Date(
            parseInt(yearA),
            parseInt(monthA) - 1,
            parseInt(dayA)
          );
          const dateB = new Date(
            parseInt(yearB),
            parseInt(monthB) - 1,
            parseInt(dayB)
          );
          return dateB.getTime() - dateA.getTime();
        }
      }
    });

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
