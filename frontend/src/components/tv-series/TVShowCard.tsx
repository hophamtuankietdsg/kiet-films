import { TVShow } from '@/types/tvShow';
import Image from 'next/image';
import React from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '../ui/card';
import { Badge } from '../ui/badge';
import { Star } from 'lucide-react';

interface TVShowCardProps {
  tvShow: TVShow;
}

export default function TVShowCard({ tvShow }: TVShowCardProps) {
  // Lấy năm từ chuỗi ngày dd/MM/yyyy
  const getYear = (dateStr: string) => {
    return dateStr.split('/')[2];
  };

  // Format rating
  const rating =
    typeof tvShow.rating === 'number' ? tvShow.rating.toFixed(1) : 'N/A';

  // Lấy năm từ firstAirDate
  const year = tvShow.firstAirDate ? getYear(tvShow.firstAirDate) : 'N/A';

  // Lấy ngày review (chỉ lấy phần ngày)
  const reviewDate = tvShow.reviewDate
    ? tvShow.reviewDate.split(' ')[0]
    : 'N/A';

  return (
    <Card className="overflow-hidden h-full">
      <div className="relative aspect-[2/3] sm:aspect-[2/3]">
        <Image
          src={`https://image.tmdb.org/t/p/w500${tvShow.posterPath}`}
          alt={tvShow.name}
          fill
          className="object-cover"
          sizes="(max-width: 640px) 45vw, (max-width: 1024px) 30vw, 25vw"
          priority
        />
      </div>
      <CardHeader className="p-2 sm:p-3">
        <CardTitle className="line-clamp-1 sm:line-clamp-2 text-sm sm:text-lg">
          {tvShow.name}
        </CardTitle>
      </CardHeader>
      <CardContent className="space-y-2 p-2 sm:p-3">
        <div className="flex items-center gap-1 sm:gap-2">
          <Star className="h-4 w-4 sm:h-5 sm:w-5 text-yellow-400 fill-yellow-400" />
          <span className="font-bold text-xs sm:text-sm">{rating}</span>
        </div>
        <div className="flex flex-wrap gap-1 sm:gap-2">
          <Badge
            variant="secondary"
            className="text-[10px] sm:text-xs px-1 py-0 h-4 sm:h-5"
          >
            {year}
          </Badge>
          <Badge
            variant="secondary"
            className="text-[10px] sm:text-xs px-1 py-0 h-4 sm:h-5"
          >
            {reviewDate}
          </Badge>
        </div>
        {tvShow.comment && (
          <p className="text-[10px] sm:text-sm text-muted-foreground italic line-clamp-2">
            &quot;{tvShow.comment}&quot;
          </p>
        )}
      </CardContent>
    </Card>
  );
}
