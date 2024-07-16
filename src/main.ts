import './style.css';

import { createApp } from 'vue';
import { createPinia } from 'pinia';

import App from './App.vue';
import router from './router';
import auth from './auth';

const app = createApp(App);

app.use(createPinia());
app.use(router);
app.use(auth);

app.mount('#app');
