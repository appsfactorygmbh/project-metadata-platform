import { type AuthDriver, type HttpDriver } from 'vue-auth3';
import type { ArgsType } from '@/models/utils/ArgsType';

type RequestConfig = Omit<ArgsType<HttpDriver['request']>[0], 'data'>;

class AuthService {
  get loginRequest(): RequestConfig {
    return {
      url: import.meta.env.VITE_BACKEND_URL + '/Auth/basic',
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        accept: 'text/plain',
      },
      responseType: 'json',
    };
  }

  get refreshRequest(): RequestConfig {
    return {
      url: import.meta.env.VITE_BACKEND_URL + '/Auth/refresh',
      method: 'GET',
      headers: {
        accept: 'text/plain',
        'Content-Type': 'application/json',
        Authorize: 'Refresh',
      },
      responseType: 'json',
    };
  }

  get authDriver(): AuthDriver {
    return {
      request(_, options, token) {
        const [accessToken, refreshToken] = (token || '|').split('|');
        if (options.headers['Authorization'] === 'Refresh') {
          options.headers['Authorization'] = `Refresh ${refreshToken}`;
        } else {
          options.headers['Authorization'] = `Bearer ${accessToken}`;
        }
        return options;
      },
      response(_, res) {
        const { token, refreshToken } = res.data;

        if (token && refreshToken) {
          return extractToken(token) + '|' + extractToken(refreshToken);
        }

        return null;
      },
    };
  }
}

const extractToken = (token: string): [string, string] => {
  const i = token.split(/Bearer:?\s?/i);
  return [i[i.length > 1 ? 1 : 0].trim(), ''];
};

const authService = new AuthService();
export type { AuthService };
export { authService };
