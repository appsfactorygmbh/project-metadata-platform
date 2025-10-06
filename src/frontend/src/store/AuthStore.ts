import { type Pinia, defineStore } from 'pinia';
import { piniaInstance } from './piniaInstance';

type AuthState = {
  _authToken: string | null;
  _authMethod: string | null;
};

export const useAuthStore = (pinia: Pinia = piniaInstance) => {
  return defineStore('auth', {
    state: (): AuthState => ({
      _authToken: null,
      _authMethod: null,
    }),

    actions: {
      setAuth(auth: string | null, method: string | null) {
        if (!auth) return;
        this._authToken = auth.split('|')[0];
        this._authMethod = method;
      },
    },

    getters: {
      authToken(state): string | null {
        return state._authToken;
      },
      authMethod(state): string | null {
        return state._authMethod;
      },
    },
  })(pinia);
};

export const authStore = useAuthStore(piniaInstance);

type AuthStore = ReturnType<typeof useAuthStore>;
export type { AuthStore };
