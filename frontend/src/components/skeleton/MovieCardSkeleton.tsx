import { Card, CardContent } from '../ui/card';
import { Skeleton } from '../ui/skeleton';

export default function MovieCardSkeleton() {
  return (
    <Card className="flex flex-col h-full">
      <Skeleton className="aspect-[2/3] rounded-t-lg" />
      <CardContent className="p-3 sm:p-4">
        <Skeleton className="h-6 w-3/4 mb-3" />
        <div className="flex gap-3">
          <Skeleton className="h-4 w-16" />
          <Skeleton className="h-4 w-16" />
        </div>
        <Skeleton className="h-4 w-full mt-3" />
      </CardContent>
    </Card>
  );
}
