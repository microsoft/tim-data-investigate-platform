const importConfig = window.appConfig || {};

export default Object.freeze({
  auth: {
    clientId: import.meta.env.VITE_AUTH_CLIENT_ID,
    authority: `https://login.microsoftonline.com/${import.meta.env.VITE_AUTH_TENANT_ID}`,
  },
  redirectUri: import.meta.env.VITE_AUTH_REDIRECT,
  apiEndpoint: import.meta.env.VITE_API_ENDPOINT,
  agGridLicenseKey: import.meta.env.VITE_AGGRID_LICENSE_KEY,
  wikiUri: import.meta.env.VITE_HELP_WIKI_URI || 'https://github.com/microsoft/tim-data-investigate-platform/wiki',
  issueUri: import.meta.env.VITE_HELP_ISSUE_URI || 'https://github.com/microsoft/tim-data-investigate-platform/issues/new?template=issue_template.md',
  nodeEnv: import.meta.env.NODE_ENV || 'production',
  ...importConfig,
});
