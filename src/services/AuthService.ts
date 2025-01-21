import { type AuthDriver, type HttpDriver } from 'vue-auth3';
import type { ArgsType } from '@/models/utils/ArgsType';
import { type SavedTokenOptions, getSavedTokens } from '@/utils/api';
import { exportTokens, importTokens } from '@/utils/api/tokenHandler';

type RequestConfig = Omit<ArgsType<HttpDriver['request']>[0], 'data'> & {
  _target?: string;
};

class AuthService {
  #options: SavedTokenOptions = {
    storage: 'localStorage',
    key: 'auth_token',
  };

  #getToken = (type: 'access' | 'refresh') => {
    const tokens = getSavedTokens(this.#options);
    const { accessToken, refreshToken } = importTokens(tokens);
    return type === 'access' ? accessToken : refreshToken;
  };

  setOptions(options: SavedTokenOptions) {
    this.#options = options;
  }

  get loginRequest(): RequestConfig {
    return {
      url: import.meta.env.VITE_BACKEND_URL + '/Auth/basic',
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        accept: 'text/plain',
      },
      responseType: 'json',
      _target: 'login',
    };
  }

  get refreshRequest(): RequestConfig {
    return {
      url: import.meta.env.VITE_BACKEND_URL + '/Auth/refresh',
      method: 'GET',
      headers: {
        accept: 'text/plain',
        'Content-Type': 'application/json',
        Authorization: `Refresh ${this.#getToken('refresh')}`,
      },
      responseType: 'json',
      _target: 'refresh',
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
      _target: 'register',
    };
  }

  get fetchUserRequest(): RequestConfig {
    return {
      url: import.meta.env.VITE_BACKEND_URL + '/Users/me',
      method: 'GET',
      headers: {
        accept: 'text/plain',
        'Content-Type': 'application/json',
      },
      responseType: 'json',
      _target: 'fetchUser',
    };
  }

  get authDriver(): AuthDriver {
    return {
      request(__, { headers, ...rest }, token) {
        const [accessToken, refreshToken] = (token || '|').split('|');
        if (
          headers['_target']?.toLowerCase().startsWith('refresh') ||
          headers['Authorization']?.startsWith('Refresh')
        ) {
          headers['Authorization'] = `Refresh ${refreshToken}`;
        } else {
          headers['Authorization'] = `Bearer ${accessToken}`;
        }
        return { headers, ...rest };
      },
      response(auth, res) {
        if (res.status === 400) {
          throw new Error('Invalid refresh token');
        }
        let { accessToken, refreshToken } = res.data;
        if (!accessToken && !refreshToken) {
          const importedTokens = importTokens(auth?.token());
          accessToken = importedTokens.accessToken;
          refreshToken = importedTokens.refreshToken;
        }

        if (accessToken && refreshToken) {
          return exportTokens({ accessToken, refreshToken });
        }

        return null;
      },
    };
  }
}

const authService = new AuthService();
export type { AuthService };
export { authService };
