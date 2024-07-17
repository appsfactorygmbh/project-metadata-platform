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
      },
      responseType: 'text',
    };
  }

  get authDriver(): AuthDriver {
    return {
      request(auth, options, token) {
        options.headers['Authorization'] = 'Bearer ' + token;

        return options;
      },
      response(auth, res) {
        console.log('authDriver response', res);
        const token = res.data.accessToken;

        if (token) {
          const i = token.split(/Bearer:?\s?/i);

          return i[i.length > 1 ? 1 : 0].trim();
        }

        return null;
      },
    };
  }
}

const authService = new AuthService();
export type { AuthService };
export { authService };
