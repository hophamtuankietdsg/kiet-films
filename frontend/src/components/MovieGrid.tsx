import { Movie } from '@/types/movie';
import Link from 'next/link';
import React from 'react';
import { Button } from './ui/button';
import MovieCard from './MovieCard';
import MovieCardSkeleton from './skeleton/MovieCardSkeleton';

interface MovieGridProps {
  movies: Movie[];
  isLoading?: boolean;
}

const MovieGrid = ({ movies, isLoading }: MovieGridProps) => {
  if (isLoading) {
    return (
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 gap-3 sm:gap-4">
          {Array.from({ length: 8 }).map((_, index) => (
            <MovieCardSkeleton key={index} />
          ))}
        </div>
      </div>
    );
  }

  if (movies.length === 0) {
    return (
      <div className="max-w-2xl mx-auto text-center px-4">
        <p className="text-muted-foreground mb-4">No rated movies yet.</p>
        <Button asChild>
          <Link href="/search">Go to search page to rate some movies!</Link>
        </Button>
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 gap-3 sm:gap-4">
        {movies.map((movie, index) => (
          <MovieCard
            key={`${movie.id}-${movie.reviewDate}-${index}`}
            movie={movie}
          />
        ))}
      </div>
    </div>
  );
};

export default MovieGrid;
