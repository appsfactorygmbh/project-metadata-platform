// Auth/
export const TOKEN_EXPIRATION = 15; // minutes, backend max 15 minutes
export const REFRESH_TOKEN_EXPIRATION = 6 * 60; // minutes, backend max 6 hours
export const TOKEN_REFRESH_INTERVAL = 0.5; // minutes, max 15 minutes

// API
export const API_BASE_PATH =
  import.meta.env.VITE_BACKEND_URL ?? window.location.origin;
