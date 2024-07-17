import { createAuth } from 'vue-auth3';
import { authService } from './services/AuthService';
import router from '@/router';
import driverHttpAxios from 'vue-auth3/drivers/http/axios';

const auth = createAuth({
  plugins: {
    router,
  },
  drivers: {
    auth: authService.authDriver,
    http: driverHttpAxios,
  },
  refreshToken: {
    ...authService.refreshRequest,
    enabled: false, // refresh token in goto page
    enabledInBackground: true, // refresh token in background
  },
  authRedirect: {
    path: '/login',
  },
  notFoundRedirect: {
    path: '/404',
  },
  loginData: {
    ...authService.loginRequest,
    fetchUser: false,
    remember: true,
    staySignedIn: false,
  },
});

export default auth;
