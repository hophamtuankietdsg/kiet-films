import { TVShow } from '@/types/tvShow';
import Image from 'next/image';
import React from 'react';
import { Card, CardContent } from '../ui/card';
import { Badge } from '../ui/badge';
import { Calendar, Clock, Star } from 'lucide-react';
import { TV_GENRES } from '@/lib/constants';
import { genreColors } from '@/lib/genreColors';

interface TVShowCardProps {
  tvShow: TVShow;
}

export default function TVShowCard({ tvShow }: TVShowCardProps) {
  const getYear = (dateStr: string) => dateStr.split('/')[2];
  const rating =
    typeof tvShow.rating === 'number' ? tvShow.rating.toFixed(1) : 'N/A';
  const year = tvShow.firstAirDate ? getYear(tvShow.firstAirDate) : 'N/A';
  const reviewDate = tvShow.reviewDate
    ? tvShow.reviewDate.split(' ')[0]
    : 'N/A';

  const genres = tvShow.genreIds
    .split(',')
    .map((id) => TV_GENRES[id])
    .filter(Boolean)
    .slice(0, 3);

  return (
    <Card className="group overflow-hidden h-full transition-all duration-300 hover:shadow-xl">
      <div className="relative aspect-[2/3]">
        <Image
          src={`https://image.tmdb.org/t/p/w500${tvShow.posterPath}`}
          alt={tvShow.name}
          fill
          className="object-cover transition-transform duration-300 group-hover:scale-105"
          sizes="(max-width: 640px) 100vw, (max-width: 1024px) 50vw, 33vw"
          priority
        />
        {/* Rating Badge */}
        <div className="absolute top-3 right-3 flex items-center gap-1 bg-black/80 backdrop-blur-sm rounded-full px-3 py-1.5 shadow-lg">
          <Star className="h-4 w-4 text-yellow-400 fill-yellow-400" />
          <span className="font-bold text-sm text-white">{rating}</span>
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
          {tvShow.name}
        </h3>
        <div className="flex items-center gap-4 text-sm text-muted-foreground">
          <div className="flex items-center gap-1.5">
            <Calendar className="h-4 w-4" />
            <span>{year}</span>
          </div>
          <div className="flex items-center gap-1.5">
            <Clock className="h-4 w-4" />
            <span>{reviewDate}</span>
          </div>
        </div>
        {tvShow.comment && (
          <p className="mt-3 text-sm text-muted-foreground italic line-clamp-2">
            &quot;{tvShow.comment}&quot;
          </p>
        )}
      </CardContent>
    </Card>
  );
}
