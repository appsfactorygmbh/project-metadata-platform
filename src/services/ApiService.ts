class ApiService {
  apiUrl = import.meta.env.VITE_BACKEND_URL;
  authToken: string | null = null;

  setAuth = (auth: string | null) => {
    console.log('setAuth', auth);
    if (!auth) return;
    this.authToken = auth;
  };

  get authHeader(): HeadersInit {
    return this.authToken ? { Authorization: `Bearer ${this.authToken}` } : {};
  }

  async fetch(url: string, init: RequestInit = {}): Promise<Response> {
    console.log('authHeader', this.authHeader);
    return fetch(this.apiUrl + url, {
      ...init,
      headers: {
        ...init.headers,
        ...this.authHeader,
      },
    });
  }
}

export { ApiService };
