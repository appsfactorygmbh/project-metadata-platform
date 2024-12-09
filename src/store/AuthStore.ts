import { type Pinia, defineStore } from 'pinia';
import { piniaInstance } from './piniaInstance';

type AuthState = {
  _authToken: string | null;
};

export const useAuthStore = (pinia: Pinia = piniaInstance) => {
  return defineStore('auth', {
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
  })(pinia);
};

export const authStore = useAuthStore(piniaInstance);

type AuthStore = ReturnType<typeof useAuthStore>;
export type { AuthStore };
