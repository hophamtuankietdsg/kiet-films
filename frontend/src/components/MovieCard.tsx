import { Movie } from '@/types/movie';
import { Star } from 'lucide-react';
import Image from 'next/image';
import React from 'react';

interface MovieCardProps {
  movie: Movie;
}

export default function MovieCard({ movie }: MovieCardProps) {
  return (
    <div className="bg-white dark:bg-gray-800 rounded-lg shadow-md overflow-hidden">
      <div className="relative h-[400px]">
        <Image
          src={`https://image.tmdb.org/t/p/w500${movie.posterPath}`}
          alt={movie.title}
          fill
          className="object-cover"
        />
      </div>
      <div className="p-4">
        <h2 className="text-xl font-bold text-gray-900 dark:text-white">
          {movie.title}
        </h2>
      </div>
      <div className="flex items-center gap-1">
        <Star className="h-5 w-5 text-yellow-400 fill-yellow-400" />
        <span className="text-gray-700 dark:text-gray-300">{movie.rating}</span>
      </div>
      <p className="text-sm text-gray-600 dark:text-gray-400">
        Release Date: {movie.releaseDate}
      </p>
      <p className="text-sm text-gray-600 dark:text-gray-400">
        Reviewed: {movie.reviewDate}
      </p>
      {movie.comment && (
        <p className="text-sm text-gray-700 dark:text-gray-300 italic">
          &quot;{movie.comment}&quot;
        </p>
      )}
    </div>
  );
}
