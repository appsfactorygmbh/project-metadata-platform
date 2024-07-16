import { createAuth } from 'vue-auth3';
import router from '@/router';
import driverAuthBearer from 'vue-auth3/drivers/auth/bearer';
import driverHttpFetch from 'vue-auth3/drivers/http/fetch';

const auth = createAuth({
  plugins: {
    router,
  },
  drivers: {
    auth: driverAuthBearer,
    http: driverHttpFetch,
  },
  refreshToken: {
    enabled: false, // refresh token in goto page
    enabledInBackground: true, // refresh token in background
  },
  loginData: {
    url: import.meta.env.VITE_BACKEND_URL + '/auth/basic',
    method: 'POST',
    fetchUser: false,
    remember: true,
    staySignedIn: false,
  },
});

export default auth;
