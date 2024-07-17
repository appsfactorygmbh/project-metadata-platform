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
      request(_, options, token) {
        const [accessToken, refreshToken] = (token || '|').split('|');
        console.log('request', accessToken, refreshToken);
        console.log('accessToken', accessToken);
        console.log('refreshToken', refreshToken);
        if (options.headers['Authorize'] === 'Refresh') {
          options.headers['Authorization'] = `Refresh ${refreshToken}`;
        } else {
          options.headers['Authorization'] = `Bearer ${accessToken}`;
        }
        return options;
      },
      response(auth, res) {
        let { accessToken, refreshToken } = res.data;
        // console.log('accessToken', accessToken);
        // console.log('refreshToken', refreshToken);
        if (!accessToken && !refreshToken) {
          [accessToken, refreshToken] = (auth.token() ?? '|').split('|');
        }
        console.log('accessToken', accessToken);
        console.log('refreshToken', refreshToken);

        if (accessToken && refreshToken) {
          return extractToken(accessToken) + '|' + extractToken(refreshToken);
        }

        return null;
      },
    };
  }
}

const extractToken = (token: string): string => {
  const i = token.split(/Bearer:?\s?/i);
  return i[i.length > 1 ? 1 : 0].trim();
};

const authService = new AuthService();
export type { AuthService };
export { authService };
