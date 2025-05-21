import './style.css';

import { createApp } from 'vue';
import { createPinia } from 'pinia';

import App from './App.vue';
import router from './router';
import initAuth from './auth';
import { appEventBus } from './utils/errors/eventBus';

const app = createApp(App);
const pinia = createPinia();

app.use(pinia);
app.use(router);
app.use(initAuth());

// logout if a criticalAuthFailure happend
appEventBus.on('criticalAuthFailure', () => {
  const authInstance = app.config.globalProperties.$auth;
  if (authInstance && typeof authInstance.logout === 'function') {
    authInstance.logout({ makeRequest: false });
  }
  if (router.currentRoute.value.path !== '/login') {
    router.push('/login')
  }
});

app.mount('#app');
