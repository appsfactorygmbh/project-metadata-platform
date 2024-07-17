// import { type HttpDriver } from 'vue-auth3';

// type RequestConfig = HttpDriver['request'];

class AuthService {
  loginRequest = () => {
    return {
      url: import.meta.env.VITE_BACKEND_URL + '/Auth/basic',
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        accept: 'text/plain',
      },
    };
  };
}

const authService = new AuthService();
export type { AuthService };
export { authService };
