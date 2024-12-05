import { TVShow } from '@/types/tvShow';
import { Video, VideoResponse } from '@/types/video';
import Image from 'next/image';
import { useState } from 'react';
import { Card, CardContent } from '../ui/card';
import { Badge } from '../ui/badge';
import { Calendar, Clock, Star, Play } from 'lucide-react';
import { TV_GENRES } from '@/lib/constants';
import { genreColors } from '@/lib/genreColors';
import { formatDate, getYearFromDate } from '@/lib/utils';
import { Button } from '../ui/button';
import VideoDialog from '../ui/video-dialog';

interface TVShowCardProps {
  tvShow: TVShow;
}

export default function TVShowCard({ tvShow }: TVShowCardProps) {
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [trailerKey, setTrailerKey] = useState<string>();

  const rating = tvShow.rating?.toString() || 'N/A';
  const year = tvShow.firstAirDate
    ? getYearFromDate(tvShow.firstAirDate)
    : 'N/A';
  const reviewDate = tvShow.reviewDate ? formatDate(tvShow.reviewDate) : 'N/A';

  const genres = tvShow.genreIds
    ? tvShow.genreIds
        .split(',')
        .map((id) => TV_GENRES[id])
        .filter(Boolean)
        .slice(0, 3)
    : [];

  const posterUrl = tvShow.posterPath
    ? `https://image.tmdb.org/t/p/w500${tvShow.posterPath}`
    : '/images/movie-placeholder.png';

  const handleTrailerClick = async () => {
    try {
      const response = await fetch(`/api/tvshows/${tvShow.id}/videos`);
      const data = (await response.json()) as VideoResponse;

      const trailer = data.results?.find(
        (video: Video) =>
          video.type.toLowerCase() === 'trailer' &&
          video.site.toLowerCase() === 'youtube'
      );

      if (trailer) {
        setTrailerKey(trailer.key);
        setIsDialogOpen(true);
      } else {
        alert('No trailer available for this TV show');
      }
    } catch (err) {
      console.error('Failed to load trailer:', err);
      alert('Failed to load the trailer');
    }
  };

  return (
    <>
      <Card className="group relative flex flex-col h-full transition-all duration-300 rounded-lg overflow-hidden">
        <div className="relative aspect-[2/3] w-full overflow-hidden rounded-t-lg">
          <Image
            src={posterUrl}
            alt={tvShow.name || 'TV Show poster'}
            fill
            className="object-cover transition-transform duration-300 group-hover:scale-105"
            sizes="(max-width: 768px) 50vw, (max-width: 1200px) 33vw, 25vw"
            priority
            onError={(e) => {
              const target = e.target as HTMLImageElement;
              target.src = '/images/movie-placeholder.png';
            }}
          />
          {/* Rating Badge */}
          <div className="absolute top-2 right-2 sm:top-3 sm:right-3 flex items-center gap-1 bg-black/80 backdrop-blur-sm rounded-full px-2 py-1 sm:px-3 sm:py-1.5 shadow-lg">
            <Star className="h-3 w-3 sm:h-4 sm:w-4 text-yellow-400 fill-yellow-400" />
            <span className="font-bold text-xs sm:text-sm text-white">
              {rating}
            </span>
          </div>
          {/* Gradient Overlay */}
          <div className="absolute inset-0 bg-gradient-to-t from-black/90 via-black/50 to-transparent" />
          {/* Genre Pills */}
          <div className="absolute bottom-2 left-2 right-2 sm:bottom-4 sm:left-4 sm:right-4 flex flex-wrap gap-1.5 sm:gap-2">
            {genres.map((genre) => (
              <Badge
                key={genre}
                className={`${
                  genreColors[genre] || 'bg-gray-500/90'
                } text-white text-[10px] sm:text-xs font-medium px-2 py-0.5 sm:px-2.5 sm:py-1 shadow-lg transition-transform hover:scale-105`}
              >
                {genre}
              </Badge>
            ))}
          </div>
          {/* Trailer Button */}
          <Button
            variant="secondary"
            size="icon"
            className="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 opacity-0 group-hover:opacity-100 transition-opacity duration-300"
            onClick={handleTrailerClick}
          >
            <Play className="h-6 w-6" />
          </Button>
        </div>
        <CardContent className="flex flex-col flex-grow p-3 sm:p-4">
          <h3 className="font-bold text-base sm:text-lg mb-2 sm:mb-3 group-hover:text-primary transition-colors">
            {tvShow.name}
          </h3>
          <div className="flex items-center gap-3 sm:gap-4 text-xs sm:text-sm text-muted-foreground">
            <div className="flex items-center gap-1 sm:gap-1.5">
              <Calendar className="h-3 w-3 sm:h-4 sm:w-4" />
              <span>{year}</span>
            </div>
            <div className="flex items-center gap-1 sm:gap-1.5">
              <Clock className="h-3 w-3 sm:h-4 sm:w-4" />
              <span>{reviewDate}</span>
            </div>
          </div>
          {tvShow.comment && (
            <p className="mt-2 sm:mt-3 text-xs sm:text-sm text-muted-foreground italic line-clamp-2">
              &quot;{tvShow.comment}&quot;
            </p>
          )}
        </CardContent>
      </Card>

      <VideoDialog
        isOpen={isDialogOpen}
        onClose={() => setIsDialogOpen(false)}
        videoKey={trailerKey}
        title={tvShow.name}
      />
    </>
  );
}
