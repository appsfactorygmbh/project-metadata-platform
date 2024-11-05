import { type AuthDriver, type HttpDriver } from 'vue-auth3';
import type { ArgsType } from '@/models/utils/ArgsType';
import { extractToken } from '@/utils/api';

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
        Authorization: 'Refresh',
      },
      responseType: 'json',
    };
  }

  get registerRequest(): RequestConfig {
    return {
      url: import.meta.env.VITE_BACKEND_URL + '/Auth/register',
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        accept: 'text/plain',
      },
      responseType: 'json',
    };
  }

  get authDriver(): AuthDriver {
    return {
      request(__, { headers, ...rest }, token) {
        const [accessToken, refreshToken] = (token || '|').split('|');
        if (headers['Authorization']?.startsWith('Refresh')) {
          headers['Authorization'] = `Refresh ${refreshToken}`;
        } else {
          headers['Authorization'] = `Bearer ${accessToken}`;
        }
        return { headers, ...rest };
      },
      response(auth, res) {
        let { accessToken, refreshToken } = res.data;
        if (!accessToken && !refreshToken) {
          [accessToken, refreshToken] = (auth.token() ?? '|').split('|');
        }

        if (accessToken && refreshToken) {
          console.log('Tokens', {
            accessToken,
            refreshToken,
          });
          return extractToken(accessToken) + '|' + extractToken(refreshToken);
        }

        return null;
      },
    };
  }
}

const authService = new AuthService();
export type { AuthService };
export { authService };
