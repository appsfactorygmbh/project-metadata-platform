type StorageType = 'localStorage' | 'sessionStorage' | 'cookieStorage';

export type SavedTokenOptions = {
  storage: StorageType;
  key: string;
};

export const getSavedTokens = ({ storage, key }: SavedTokenOptions): string => {
  let tokens = '';
  switch (storage) {
    case 'localStorage':
      tokens = localStorage.getItem(key) ?? '';
      break;
    case 'sessionStorage':
      tokens = sessionStorage.getItem(key) ?? '';
      break;
    case 'cookieStorage':
      tokens = getCookie(key) ?? '';
      break;
    default:
      throw new Error('Invalid storage type');
  }
  console.log('tokens', tokens);
  tokens = tokens.trim().replace(/["']/g, '');
  return tokens;
};

function getCookie(name: string) {
  const value = `; ${document.cookie}`;
  const parts = value.split(`; ${name}=`);
  if (parts.length === 2) return parts.pop()?.split(';').shift();
}
