import { Film } from 'lucide-react';
import Link from 'next/link';

export default function Navbar() {
  return (
    <nav className="bg-white dark:bg-gray-800 shadow">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex items-center justify-between h-16">
          <div className="flex items-center">
            <Link href="/" className="flex items-center gap-2">
              <Film className="h-8 w-8 text-blue-500" />
              <span className="text-xl font-bold text-gray-900 dark:text-white">
                Movie Rating
              </span>
            </Link>
          </div>
          <div className="flex items-baseline space-x-2 sm:space-x-4">
            <Link
              href="/"
              className="px-2 sm:px-3 py-2 rounded-md text-xs sm:text-sm font-medium text-gray-600 dark:text-gray-300 hover:text-blue-500 hover:bg-gray-100 dark:hover:text-blue-400 dark:hover:bg-gray-700 transition-colors"
            >
              Movies
            </Link>
            <Link
              href="/tv-series"
              className="px-2 sm:px-3 py-2 rounded-md text-xs sm:text-sm font-medium text-gray-600 dark:text-gray-300 hover:text-blue-500 hover:bg-gray-100 dark:hover:text-blue-400 dark:hover:bg-gray-700 transition-colors"
            >
              TV Series
            </Link>
          </div>
        </div>
      </div>
    </nav>
  );
}
