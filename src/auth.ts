import { createAuth } from 'vue-auth3';
import { authService } from './services/AuthService';
import router from '@/router';
import driverAuthBearer from 'vue-auth3/drivers/auth/bearer';
import driverHttpAxios from 'vue-auth3/drivers/http/axios';

const auth = createAuth({
  plugins: {
    router,
  },
  drivers: {
    auth: driverAuthBearer,
    http: driverHttpAxios,
  },
  refreshToken: {
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
    ...authService.loginRequest(),
    fetchUser: false,
    remember: true,
    staySignedIn: false,
  },
  // cookie: {},
});

export default auth;
