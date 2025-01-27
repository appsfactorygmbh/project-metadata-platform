/*
  Isolates token from string
  Example:
  "Bearer 1234567890" -> "1234567890"
  "Refresh 1234567890" -> "1234567890"
*/
export const extractToken = (token: string): string => {
  const i = token.split(/(?:Bearer|Refresh):*\s*/i);
  return i[i.length > 1 ? 1 : 0].trim();
};
