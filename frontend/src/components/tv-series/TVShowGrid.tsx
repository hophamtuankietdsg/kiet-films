import { TVShow } from '@/types/tvShow';
import Link from 'next/link';
import React from 'react';
import { Button } from '../ui/button';
import TVShowCard from './TVShowCard';
import TVShowCardSkeleton from '../skeleton/TVShowCardSkeleton';

interface TVShowGridProps {
  tvShows: TVShow[];
  isLoading?: boolean;
}

const TVShowGrid = ({ tvShows, isLoading }: TVShowGridProps) => {
  if (isLoading) {
    return (
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 gap-3 sm:gap-4">
          {Array.from({ length: 8 }).map((_, index) => (
            <TVShowCardSkeleton key={index} />
          ))}
        </div>
      </div>
    );
  }

  if (tvShows.length === 0) {
    return (
      <div className="max-w-2xl mx-auto text-center px-4">
        <p className="text-muted-foreground mb-4">No rated TV shows yet.</p>
        <Button asChild>
          <Link href="/tv-search">
            Go to search page to rate some TV shows!
          </Link>
        </Button>
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 gap-3 sm:gap-4">
        {tvShows.map((tvShow) => (
          <TVShowCard key={tvShow.id} tvShow={tvShow} />
        ))}
      </div>
    </div>
  );
};

export default TVShowGrid;
