import { defineStore } from 'pinia';
import Cookies from 'js-cookie';

export const useCurrentUserStore = defineStore('currentUser', {
  state: () => ({
    currentUser: JSON.parse(Cookies.get('userData') || '{}'),
  }),
  actions: {
    setUser(userData: { username: string }) {
      this.currentUser = userData;
      Cookies.set('userData', JSON.stringify(userData), {
        expires: 1,
        secure: true,
        sameSite: 'Strict',
      });
    },
    clearUser() {
      this.currentUser = {};
      Cookies.remove('userData');
    },
  },
});

export type CurrentUserStore = ReturnType<typeof useCurrentUserStore>;
