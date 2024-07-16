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
});

export default auth;
