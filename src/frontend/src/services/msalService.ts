import {
  InteractionRequiredAuthError,
  LogLevel,
  PublicClientApplication,
} from '@azure/msal-browser';
export const msalConfig = {
  auth: {
    clientId:
      import.meta.env.VITE_AZURE_FRONTEND_CLIENT_ID ??
      'AZURE_FRONTEND_CLIENT_ID', // This is the ONLY mandatory field that you need to supply.
    authority: import.meta.env.VITE_AZURE_AUTHORITY ?? 'AZURE_AUTHORITY', // Replace the placeholder with your tenant info
    redirectUri: window.location.origin + '/login', // Points to window.location.origin. You must register this URI on Microsoft Entra admin center/App Registration.
    postLogoutRedirectUri: '/login', // Indicates the page to navigate after logout.
    navigateToLoginRequestUrl: false, // If "true", will navigate back to the original request location before processing the auth code response.
  },
  cache: {
    cacheLocation: 'localStorage', // Configures cache location. "sessionStorage" is more secure, but "localStorage" gives you SSO between tabs.
    storeAuthStateInCookie: false,
  },
  system: {
    loggerOptions: {
      loggerCallback: (
        level: LogLevel,
        message: string,
        containsPii: boolean,
      ) => {
        if (containsPii) {
          return;
        }
        switch (level) {
          case LogLevel.Error:
            console.error(message);
            return;
          case LogLevel.Info:
            console.info(message);
            return;
          case LogLevel.Verbose:
            console.debug(message);
            return;
          case LogLevel.Warning:
            console.warn(message);
            return;
          default:
            return;
        }
      },
    },
  },
};

export const TokenRequest = {
  scopes: [import.meta.env.VITE_AZURE_SCOPE ?? 'AZURE_SCOPE'],
};

export const msalInstance = new PublicClientApplication(msalConfig);

export const msalService = {
  async login() {
    await msalInstance.loginRedirect();
  },

  async logout() {
    const currentAccount = msalInstance.getActiveAccount();
    if (currentAccount) {
      await msalInstance.logoutRedirect({
        account: currentAccount,
        postLogoutRedirectUri: '/login',
      });
    }
  },

  getActiveUser() {
    return msalInstance.getActiveAccount();
  },

  async getAccessToken() {
    const account = msalInstance.getActiveAccount();
    if (!account) {
      return null;
    }

    const request = {
      ...TokenRequest,
      account: account,
    };

    try {
      const response = await msalInstance.acquireTokenSilent(request);
      return response.accessToken;
    } catch (error) {
      if (error instanceof InteractionRequiredAuthError) {
        console.warn(
          'Silent token acquisition failed. Acquiring token using redirect.',
        );
        return null;
      } else {
        return null;
      }
    }
  },
  async getAccessTokenSilent() {
    const account = msalInstance.getActiveAccount();
    if (!account) {
      return null;
    }

    const request = {
      ...TokenRequest,
      account: account,
    };

    try {
      const response = await msalInstance.acquireTokenSilent(request);
      return response.accessToken;
    } catch (error) {
      return null;
    }
  },
};
