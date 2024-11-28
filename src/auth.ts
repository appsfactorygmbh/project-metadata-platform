import { createAuth, defineHttpDriver } from 'vue-auth3';
import { authService } from './services/AuthService';
import router from '@/router';
import axios from 'axios';
import { REFRESH_TOKEN_EXPIRATION, TOKEN_REFRESH_INTERVAL } from './constants';

const auth = createAuth({
  plugins: {
    router,
  },
  drivers: {
    auth: authService.authDriver,
    http: defineHttpDriver({ request: axios }),
  },
  refreshToken: {
    ...authService.refreshRequest,
    enabled: true, // refresh token in goto page
    enabledInBackground: true, // refresh token in background
    interval: TOKEN_REFRESH_INTERVAL, // in minutes
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
  fetchData: {
    ...authService.fetchUserRequest,
    enabled: false,
  },
  loginData: {
    ...authService.loginRequest,
    fetchUser: false,
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
    expires: REFRESH_TOKEN_EXPIRATION * 60 * 1000, // in milliseconds
  },
});

export default auth;
