import type { HttpDriver } from 'vue-auth3';
import type { ArgsType } from '@/models/utils/ArgsType';

type RequestConfig = Omit<ArgsType<HttpDriver['request']>[0], 'data'>;

class AuthService {
  loginRequest = (): RequestConfig => {
    return {
      url: import.meta.env.VITE_BACKEND_URL + '/Auth/basic',
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        accept: 'text/plain',
      },
      responseType: 'text',
    };
  };
}

const authService = new AuthService();
export type { AuthService };
export { authService };
