import { useEffect, useState } from 'react';

interface MediaItem {
  id: number;
  rating: number;
  reviewDate: string;
  isHidden: boolean;
  title?: string;
  name?: string;
  releaseDate?: string;
  firstAirDate?: string;
}

interface UseMediaListProps<T extends MediaItem> {
  fetchData: () => Promise<T[]>;
  titleField: keyof Pick<T, 'title' | 'name'>;
  dateField: keyof Pick<T, 'releaseDate' | 'firstAirDate'>;
}

export function useMediaList<T extends MediaItem>({
  fetchData,
  titleField,
  dateField,
}: UseMediaListProps<T>) {
  const [items, setItems] = useState<T[]>([]);
  const [filteredItems, setFilteredItems] = useState<T[]>([]);
  const [searchQuery, setSearchQuery] = useState('');
  const [sortBy, setSortBy] = useState('release-desc');
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const fetchItems = async () => {
      setIsLoading(true);
      try {
        const data = await fetchData();
        const visibleItems = data.filter((item) => !item.isHidden);
        setItems(visibleItems);
        setFilteredItems(visibleItems);
      } finally {
        setIsLoading(false);
      }
    };
    fetchItems();
  }, [fetchData]);

  useEffect(() => {
    let result = [...items];

    if (searchQuery) {
      result = result.filter((item) => {
        const title = item[titleField];
        return (
          typeof title === 'string' &&
          title.toLowerCase().includes(searchQuery.toLowerCase())
        );
      });
    }

    result.sort((a, b) => {
      const parseDate = (dateStr: string | undefined) => {
        if (!dateStr) return new Date(0);
        const [day, month, year] = dateStr.split('/');
        return new Date(parseInt(year), parseInt(month) - 1, parseInt(day));
      };

      switch (sortBy) {
        case 'release-desc':
          return (
            parseDate(b[dateField] as string).getTime() -
            parseDate(a[dateField] as string).getTime()
          );
        case 'release-asc':
          return (
            parseDate(a[dateField] as string).getTime() -
            parseDate(b[dateField] as string).getTime()
          );
        case 'rating-desc':
          return b.rating - a.rating;
        case 'rating-asc':
          return a.rating - b.rating;
        case 'review-date':
          return (
            parseDate(b.reviewDate).getTime() -
            parseDate(a.reviewDate).getTime()
          );
        default:
          return (
            parseDate(b[dateField] as string).getTime() -
            parseDate(a[dateField] as string).getTime()
          );
      }
    });

    setFilteredItems(result);
  }, [items, searchQuery, sortBy, titleField, dateField]);

  return {
    items: filteredItems,
    isLoading,
    searchQuery,
    setSearchQuery,
    sortBy,
    setSortBy,
  };
}
