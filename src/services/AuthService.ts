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
        Authorization: "Refresh"
      },
      responseType: 'json',
    };
  }

  get authDriver(): AuthDriver {
    return {
      request(auth, options, token) {
        if (options.headers['Authorization'] === 'Refresh') {
          options.headers['Authorization'] = 'Refresh ' + token.split('|')[1];
          return options;
        }

        options.headers['Authorization'] = 'Bearer ' + token.split('|')[0];

        return options;
      },
      response(auth, res) {
        const token = res.data.accessToken;
        const refreshToken = res.data.refreshToken;

        if (token && refreshToken) {
          const i = token.split(/Bearer:?\s?/i);

          const result =  i[i.length > 1 ? 1 : 0].trim();

          return result + "|" + refreshToken;
        }

        return null;
      },
    };
  }
}

const authService = new AuthService();
export type { AuthService };
export { authService };
