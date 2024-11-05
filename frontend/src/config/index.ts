export const config = {
  apiUrl: process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5012',
  isProduction: process.env.NODE_ENV === 'production',
};
