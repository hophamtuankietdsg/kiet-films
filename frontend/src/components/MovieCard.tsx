import { Movie } from '@/types/movie';
import Image from 'next/image';
import React from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Badge } from './ui/badge';
import { Star } from 'lucide-react';

interface MovieCardProps {
  movie: Movie;
}

export default function MovieCard({ movie }: MovieCardProps) {
  return (
    <Card className="overflow-hidden h-full w-full">
      <div className="relative h-[200px] sm:h-[300px] md:h-[400px]">
        <Image
          src={`https://image.tmdb.org/t/p/w500${movie.posterPath}`}
          alt={movie.title}
          fill
          className="object-cover"
          sizes="(max-width: 640px) 50vw, (max-width: 768px) 33vw, 25vw"
        />
      </div>
      <CardHeader className="space-y-2 p-3 sm:p-4 md:p-6">
        <CardTitle className="line-clamp-2 text-base sm:text-lg">
          {movie.title}
        </CardTitle>
      </CardHeader>
      <CardContent className="space-y-3 sm:space-y-4 p-3 sm:p-4 md:p-6">
        <div className="flex items-center gap-2">
          <Star className="h-4 w-4 sm:h-5 sm:w-5 text-yellow-400 fill-yellow-400" />
          <span className="font-bold text-sm sm:text-base">
            {movie.rating.toFixed(1)}
          </span>
        </div>
        <div className="flex flex-wrap gap-1.5 sm:gap-2">
          <Badge variant="secondary" className="text-xs sm:text-sm">
            Released: {new Date(movie.releaseDate).getFullYear()}
          </Badge>
          <Badge variant="secondary" className="text-xs sm:text-sm">
            {new Date(movie.reviewDate).toLocaleDateString()}
          </Badge>
        </div>
        {movie.comment && (
          <p className="text-xs sm:text-sm text-muted-foreground italic line-clamp-2 sm:line-clamp-33">
            &quot;{movie.comment}&quot;
          </p>
        )}
      </CardContent>
    </Card>
  );
}
