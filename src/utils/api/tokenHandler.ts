import { extractToken } from './extractToken';

type Tokens = {
  accessToken: string | null;
  refreshToken: string | null;
};

type RawExport = string;

/*
  Imports tokens from raw export
  Example:
  "1234567890|1234567890" -> { accessToken: "1234567890", refreshToken: "1234567890" }
  "|" -> { accessToken: null, refreshToken: null }
  "Bearer 1234567890|Refresh 1234567890" -> { accessToken: "1234567890", refreshToken: "1234567890" }
*/
export const importTokens = (rawExport?: RawExport | null): Tokens => {
  if (!rawExport) return { accessToken: null, refreshToken: null };
  const [accessToken, refreshToken] = rawExport.split('|');
  return {
    accessToken: extractToken(accessToken),
    refreshToken: extractToken(refreshToken),
  };
};

/*
  Exports tokens to raw export
  Example:
  { accessToken: "1234567890", refreshToken: "1234567890" } -> "1234567890|1234567890"
  { accessToken: null, refreshToken: null } -> "|"
*/
export const exportTokens = (tokens: Tokens): RawExport => {
  return `${tokens.accessToken ?? ''}|${tokens.refreshToken ?? ''}`;
};
