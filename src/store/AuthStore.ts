import { defineStore } from 'pinia';
import { piniaInstance } from './piniaInstance';

type AuthState = {
  _authToken: string | null;
};

export const authStore = defineStore('auth', {
  state: (): AuthState => ({
    _authToken: null,
  }),

  actions: {
    setAuth(auth: string | null) {
      if (!auth) return;
      this._authToken = auth.split('|')[0];
    },
  },

  getters: {
    authToken(state): string | null {
      return state._authToken;
    },
  },
})(piniaInstance);

type AuthStore = typeof authStore;
export type { AuthStore };
