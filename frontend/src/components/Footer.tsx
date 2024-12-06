import { Github, Linkedin } from 'lucide-react';
import Link from 'next/link';

export default function Footer() {
  return (
    <footer className="bg-white dark:bg-gray-800 shadow-inner mt-auto">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="flex flex-col items-center justify-between gap-4 sm:flex-row">
          <div className="flex flex-col items-center sm:items-start">
            <h2 className="text-lg font-semibold text-gray-900 dark:text-white">
              Movie Rating
            </h2>
            <p className="text-sm text-gray-600 dark:text-gray-300">
              © 2024 Movie Rating. All rights reserved.
            </p>
          </div>

          <div className="flex items-center space-x-4">
            <Link
              href="https://github.com/hophamtuankietdsg/kiet-films"
              target="_blank"
              rel="noopener noreferrer"
              className="text-gray-600 hover:text-blue-500 dark:text-gray-300 dark:hover:text-blue-400 transition-colors"
            >
              <Github className="h-6 w-6" />
            </Link>
            <Link
              href="https://www.linkedin.com/in/kiethoptuan/"
              target="_blank"
              rel="noopener noreferrer"
              className="text-gray-600 hover:text-blue-500 dark:text-gray-300 dark:hover:text-blue-400 transition-colors"
            >
              <Linkedin className="h-6 w-6" />
            </Link>
          </div>
        </div>
      </div>
    </footer>
  );
}
