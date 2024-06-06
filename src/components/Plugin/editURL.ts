// removes path after domain name
export const cutAfterTLD = (url: string): string => {
  const regex = /^(https?:\/\/)?([^/]+(\.[a-z]{2,}))/i;
  const match = url.match(regex);
  if (match) {
    return match[0];
  }
  return "appsfactory.de"
};

// use google entpoint to get favicon of website
export const createFaviconURL = (tld: string) => {
  return 'https://www.google.com/s2/favicons?domain=' + tld + '&sz=128';
};
