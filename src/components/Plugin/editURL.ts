export const cutAfterTLD = (url: string): string => {
    const regex = /^(https?:\/\/)?([^\/]+(\.[a-z]{2,}))/i;
    const match = url.match(regex);
    if (match) {
      return match[0];
    }
    return url;
}