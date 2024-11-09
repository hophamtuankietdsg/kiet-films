import { type ClassValue, clsx } from 'clsx';
import { twMerge } from 'tailwind-merge';

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

// Hàm format ngày tháng theo định dạng dd/MM/yyyy
export function formatDate(dateString: string): string {
  if (!dateString) return 'N/A';

  // Xử lý cả 2 định dạng có thể nhận được từ backend
  const parts = dateString.includes('-')
    ? dateString.split('-')
    : dateString.split('/');

  if (parts.length !== 3) return dateString;

  // Nếu ngày tháng đã đúng định dạng dd-MM-yyyy hoặc dd/MM/yyyy
  if (parts[0].length === 2) return dateString.replace(/-/g, '/');

  // Nếu ngày tháng ở định dạng yyyy-MM-dd
  const [year, month, day] = parts;
  return `${day.padStart(2, '0')}/${month.padStart(2, '0')}/${year}`;
}

// Hàm lấy năm từ chuỗi ngày tháng
export function getYearFromDate(dateString: string): string {
  if (!dateString) return 'N/A';

  // Xử lý cả 2 định dạng
  const parts = dateString.includes('-')
    ? dateString.split('-')
    : dateString.split('/');

  if (parts.length !== 3) return 'N/A';

  // Lấy năm (có thể ở vị trí đầu hoặc cuối)
  const year = parts[0].length === 4 ? parts[0] : parts[2];
  return year;
}
