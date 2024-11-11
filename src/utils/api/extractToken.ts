export const extractToken = (token: string): string => {
  const i = token.split(/(?:Bearer|Refresh):*\s*/i);
  return i[i.length > 1 ? 1 : 0].trim();
};
