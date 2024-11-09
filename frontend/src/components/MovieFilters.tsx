import React from 'react';
import { Input } from './ui/input';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from './ui/select';

interface MovieFiltersProps {
  searchQuery: string;
  onSearchChange: (value: string) => void;
  sortBy: string;
  onSortChange: (value: string) => void;
}

const MovieFilters = ({
  searchQuery,
  onSearchChange,
  sortBy,
  onSortChange,
}: MovieFiltersProps) => {
  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 mb-6">
      <div className="flex items-center gap-4">
        <div className="min-w-0 flex-1">
          <Input
            placeholder="Search movies..."
            value={searchQuery}
            onChange={(e) => onSearchChange(e.target.value)}
            className="max-w-md"
          />
        </div>
        <Select
          value={sortBy}
          onValueChange={onSortChange}
          defaultValue="release-desc"
        >
          <SelectTrigger className="w-[140px] sm:w-[200px]">
            <SelectValue defaultValue="release-desc">
              Release Date (Newest)
            </SelectValue>
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="release-desc">Release Date (Newest)</SelectItem>
            <SelectItem value="release-asc">Release Date (Oldest)</SelectItem>
            <SelectItem value="rating-desc">Rating (High to Low)</SelectItem>
            <SelectItem value="rating-asc">Rating (Low to High)</SelectItem>
            <SelectItem value="review-date">Review Date</SelectItem>
          </SelectContent>
        </Select>
      </div>
    </div>
  );
};

export default MovieFilters;
