class ApiService {
  apiUrl = import.meta.env.VITE_BACKEND_URL;
  authToken: string | null = null;

  setAuth = (auth: string | null) => {
    if (!auth) return;
    this.authToken = auth.split('|')[0];
  };

  get authHeader(): HeadersInit {
    return this.authToken ? { Authorization: `Bearer ${this.authToken}` } : {};
  }

  async fetch(url: string, init: RequestInit = {}): Promise<Response> {
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
