import { Movie } from '@/types/movie';
import Image from 'next/image';
import React from 'react';
import { Card, CardContent } from './ui/card';
import { Badge } from './ui/badge';
import { Star, Calendar, Clock } from 'lucide-react';
import { MOVIE_GENRES } from '@/lib/constants';
import { genreColors } from '@/lib/genreColors';

interface MovieCardProps {
  movie: Movie;
}

export default function MovieCard({ movie }: MovieCardProps) {
  const genres = movie.genreIds
    .split(',')
    .map((id) => MOVIE_GENRES[id])
    .filter(Boolean)
    .slice(0, 3);

  return (
    <Card className="group overflow-hidden h-full transition-all duration-300 hover:shadow-xl">
      <div className="relative aspect-[2/3]">
        <Image
          src={`https://image.tmdb.org/t/p/w500${movie.posterPath}`}
          alt={movie.title}
          fill
          className="object-cover transition-transform duration-300 group-hover:scale-105"
          sizes="(max-width: 640px) 100vw, (max-width: 1024px) 50vw, 33vw"
          priority
        />
        {/* Rating Badge */}
        <div className="absolute top-3 right-3 flex items-center gap-1 bg-black/80 backdrop-blur-sm rounded-full px-3 py-1.5 shadow-lg">
          <Star className="h-4 w-4 text-yellow-400 fill-yellow-400" />
          <span className="font-bold text-sm text-white">
            {movie.rating.toFixed(1)}
          </span>
        </div>
        {/* Gradient Overlay */}
        <div className="absolute inset-0 bg-gradient-to-t from-black/80 via-transparent to-transparent" />
        {/* Genre Pills */}
        <div className="absolute bottom-4 left-4 right-4 flex flex-wrap gap-2">
          {genres.map((genre) => (
            <Badge
              key={genre}
              className={`${
                genreColors[genre] || 'bg-gray-500/90'
              } text-white text-xs font-medium px-2.5 py-1 shadow-lg transition-transform hover:scale-105`}
            >
              {genre}
            </Badge>
          ))}
        </div>
      </div>
      <CardContent className="p-4">
        <h3 className="font-bold text-lg mb-3 line-clamp-1 group-hover:text-primary transition-colors">
          {movie.title}
        </h3>
        <div className="flex items-center gap-4 text-sm text-muted-foreground">
          <div className="flex items-center gap-1.5">
            <Calendar className="h-4 w-4" />
            <span>{new Date(movie.releaseDate).getFullYear()}</span>
          </div>
          <div className="flex items-center gap-1.5">
            <Clock className="h-4 w-4" />
            <span>{new Date(movie.reviewDate).toLocaleDateString()}</span>
          </div>
        </div>
        {movie.comment && (
          <p className="mt-3 text-sm text-muted-foreground italic line-clamp-2">
            &quot;{movie.comment}&quot;
          </p>
        )}
      </CardContent>
    </Card>
  );
}
