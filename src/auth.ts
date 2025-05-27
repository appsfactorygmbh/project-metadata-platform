import {
  createAuth,
  defineHttpDriver,
  type Options as AuthOptions,
} from 'vue-auth3';
import { authService } from './services/AuthService';
import axios, { type AxiosError } from 'axios';
import { REFRESH_TOKEN_EXPIRATION, TOKEN_REFRESH_INTERVAL } from './constants';
import { appEventBus } from './utils/errors/eventBus';
import { InvalidRefreshTokenError } from './utils/errors/invalidRefreshTokenError';

// configure the axios client used for the auth handling
// to emit an event on the global event bus that the auth failed
// causes redirect to login page (configured in main.ts)
axios.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    const config = error.config;
    const response = error.response;

    if (response?.status === 400 && config?.url?.endsWith('/Auth/refresh')) {
      appEventBus.emit('criticalAuthFailure');
      return new InvalidRefreshTokenError();
    } else {
      return error;
    }
  },
);

type RequestOptionType =
  | 'drivers'
  | 'refreshToken'
  | 'loginData'
  | 'registerData'
  | 'fetchData';
type BaseAuthOptions = Omit<AuthOptions, RequestOptionType>;
type RequestAuthOptions = Pick<AuthOptions, RequestOptionType>;

const initAuth = () => {
  const baseOptions = {
    initSync: true,
    authRedirect: {
      path: '/login',
    },
    notFoundRedirect: {
      path: '/404',
    },
    forbiddenRedirect: {
      path: '/403',
    },
    rolesKey: 'auth_roles',
    rememberKey: 'auth_remember',
    userKey: 'auth_user',
    staySignedInKey: 'auth_stay_signed_in',
    tokenDefaultKey: 'auth_token',
    tokenImpersonateKey: 'auth_token_impersonate',
    stores: ['storage', 'cookie'],
    cookie: {
      secure: true,
      expires: REFRESH_TOKEN_EXPIRATION * 60 * 1000,
    },
  } satisfies BaseAuthOptions;

  authService.setOptions({
    storage: baseOptions.stores.includes('cookie')
      ? 'cookieStorage'
      : 'localStorage',
    key: baseOptions.tokenDefaultKey,
  });

  const requestOptions = {
    drivers: {
      auth: authService.authDriver,
      http: defineHttpDriver({ request: axios }),
    },
    refreshToken: {
      ...authService.refreshRequest,
      enabled: true,
      enabledInBackground: true,
      interval: TOKEN_REFRESH_INTERVAL,
    },
    fetchData: {
      ...authService.fetchUserRequest,
      enabled: true,
      cache: true,
      waitRefresh: true,
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
  } satisfies RequestAuthOptions;

  const options: AuthOptions = {
    ...baseOptions,
    ...requestOptions,
  };

  return createAuth(options);
};

export default initAuth;
