import { Movie } from '@/types/movie';
import Link from 'next/link';
import React from 'react';
import { Button } from './ui/button';
import MovieCard from './MovieCard';

interface MovieGridProps {
  movies: Movie[];
}

const MovieGrid = ({ movies }: MovieGridProps) => {
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
    <div className="max-w-7l mx-auto px-4 sm:px-6 lg:px-8">
      <div className="grid grid-cols-2 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4 sm:gap-6">
        {movies.map((movie) => (
          <div key={movie.id} className="w-full max-w-sm mx-auto">
            <MovieCard movie={movie} />
          </div>
        ))}
      </div>
    </div>
  );
};

export default MovieGrid;
