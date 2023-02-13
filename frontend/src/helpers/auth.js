import { PublicClientApplication, LogLevel } from '@azure/msal-browser';
import runtimeConfig from '@/helpers/runtimeConfig';

const msalConfig = {
  auth: {
    clientId: runtimeConfig.auth.clientId,
    authority: runtimeConfig.auth.authority,
  },
  cache: {
    cacheLocation: 'localStorage', // This configures where your cache will be stored
    storeAuthStateInCookie: false, // Set this to "true" if you are having issues on IE11 or Edge
  },
  system: {
    loggerOptions: {
      loggerCallback: (level, message, containsPii) => {
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
            console.log(message);
        }
      },
    },
  },
};

class AuthHandler {
  // Most of this code is taken from: https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/samples/msal-browser-samples/VanillaJSTestApp2.0/app/default/auth.js

  constructor() {
    this.interactivePromise = null;
    this.clientApplication = new PublicClientApplication(msalConfig);
  }

  getUserId() {
    const activeAccount = this.clientApplication.getActiveAccount();
    if (activeAccount !== null) {
      return activeAccount.username;
    }
    return null;
  }

  handleResponse(response) {
    if (response !== null) {
      this.clientApplication.setActiveAccount(response.account);
    } else {
      const currentAccounts = this.clientApplication.getAllAccounts();

      if (currentAccounts === null) {
        console.warn('No accounts detected');
        return;
      }

      if (currentAccounts.length > 1) {
        // Add choose account code here
        console.warn(
          'Multiple accounts detected, need to add choose account code.',
        );
      } else if (currentAccounts.length === 1) {
        const activeAccount = currentAccounts[0];
        this.clientApplication.setActiveAccount(activeAccount);
      }
    }
  }

  async login() {
    if (!this.clientApplication.getActiveAccount()) {
      const authResult = await this.clientApplication.loginPopup({
        scopes: [`${runtimeConfig.auth.clientId}/.default`],
        redirectUri: runtimeConfig.redirectUri,
        prompt: 'select_account',
      });
      this.handleResponse(authResult);
    }
  }

  async logout() {
    const activeAccount = this.clientApplication.getActiveAccount();
    if (activeAccount) {
      await this.clientApplication.logoutRedirect({
        account: activeAccount,
        onRedirectNavigate: () => false,
      });
    }
  }

  async getTokenSilent(request) {
    try {
      console.debug(`Trying to get silent token... ${JSON.stringify(request)}`);
      return await this.clientApplication.acquireTokenSilent(request);
    } catch (error) {
      console.info(
        'Silent token acquisition failed, acquiring token using pop up',
      );
      return this.getTokenInteractive(request);
    }
  }

  async getTokenInteractive(request) {
    try {
      return await this.clientApplication.acquireTokenPopup(request);
    } catch (error) {
      console.error(error);
      return null;
    }
  }

  async getToken(request) {
    let authResponse;
    let resolver = () => {};

    // Singleton promise to ensure only one login prompt
    if (this.interactivePromise === null) {
      console.debug('Creating new login prompt promise...');
      this.interactivePromise = new Promise((resolve) => {
        resolver = resolve;
      });
    } else {
      console.debug('Waiting for login prompt promise to complete...');
      await this.interactivePromise;
    }

    const activeAccount = this.clientApplication.getActiveAccount();
    if (activeAccount) {
      authResponse = await this.getTokenSilent(request);
    } else {
      authResponse = await this.getTokenInteractive(request);
    }
    await this.handleResponse(authResponse);

    resolver();
    this.interactivePromise = null;

    if (authResponse !== null) {
      return authResponse.accessToken || null;
    }
    return null;
  }

  async getKustoToken() {
    return this.getToken({
      scopes: ['https://help.kusto.windows.net/.default'],
    });
  }

  async getApiToken() {
    return this.getToken({
      scopes: [`api://${runtimeConfig.auth.clientId}/user_impersonation`],
    });
  }
}

export default new AuthHandler();
