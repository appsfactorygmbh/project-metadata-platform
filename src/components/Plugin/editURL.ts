export const cutAfterTLD = (url: string): string => {
  const regex = /^(https?:\/\/)?([^/]+(\.[a-z]{2,}))/i;
  const match = url.match(regex);
  if (match) {
    return match[0];
  }
  return url;
};

export const createFaviconURL = (tld: string) => {
  // return 'https://www.google.com/s2/favicons?domain=${tld}&sz=128';
  return 'https://www.google.com/s2/favicons?domain=' + tld + '&sz=128';
};
