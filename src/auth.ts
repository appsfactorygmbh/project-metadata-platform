import { createAuth } from 'vue-auth3';
import { authService } from './services/AuthService';
import router from '@/router';
//@ts-expect-error types/file not found
import driverHttpAxios from 'vue-auth3/drivers/http/axios';

const auth = createAuth({
  plugins: {
    router,
  },
  fetchData: {
    enabled: true, // send a request to `/api/user` if the user information stored in the cookie is not visible
    cache: true, //save user information to localStorage for use
    enabledInBackground: true, // refresh user information in the background
  },
  drivers: {
    auth: authService.authDriver,
    http: driverHttpAxios,
  },
  refreshToken: {
    ...authService.refreshRequest,
    enabled: true, // refresh token in goto page
    enabledInBackground: true, // refresh token in background
    interval: 30, // 30 seconds of max 15 minutes
  },
  authRedirect: {
    path: '/login',
  },
  notFoundRedirect: {
    path: '/404',
  },
  forbiddenRedirect: {
    path: '/403',
  },
  loginData: {
    ...authService.loginRequest,
    fetchUser: true,
    remember: true,
    staySignedIn: false,
  },
  registerData: {
    ...authService.registerRequest,
    fetchUser: false,
  },
  rolesKey: 'auth_roles',
  rememberKey: 'auth_remember',
  userKey: 'auth_user',
  staySignedInKey: 'auth_stay_signed_in',
  tokenDefaultKey: 'auth_token',
  tokenImpersonateKey: 'auth_token_impersonate',
  stores: ['storage', 'cookie'], // ['storage', 'cookie']
  cookie: {
    secure: true,
    expires: 6 * 60 * 60 * 1000, // 6h of max 6 hours
  },
});

export default auth;
