using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MsalRestClient
{
    internal class AuthHelper
    {
        private readonly string clientId;
        private readonly ClientApplicationBase clientApplication;
        private readonly IEnumerable<string> scopes;
        private readonly bool isUserMode;

        public AuthHelper(IConfigProvider configProvider)
        {
            clientId = configProvider.GetClientId();
            scopes = configProvider.GetScopes();

            if (string.IsNullOrWhiteSpace(configProvider.GetClientSecret()))
            {
                clientApplication = new PublicClientApplication(clientId, configProvider.GetAuthority(), TokenCacheHelper.GetUserCache());
                isUserMode = true;
            }
            else
            {
                var clientCredential = new ClientCredential(configProvider.GetClientSecret());
                clientApplication = new ConfidentialClientApplication(clientId, configProvider.GetAuthority(), configProvider.GetRedirectUri(), clientCredential, null, TokenCacheHelper.GetUserCache());
                isUserMode = false;
            }
        }

        public async Task<string> GetTokenAsync()
        {
            if (isUserMode)
                return await GetTokenForCurrentAccountAsync();
            return await GetTokenForClientAsync();
        }

        private async Task<string> GetTokenForCurrentAccountAsync()
        {
            try
            {
                AuthenticationResult authenticationResult = null;
                var publicClientApplication = clientApplication as PublicClientApplication;
                var accounts = await clientApplication.GetAccountsAsync().ConfigureAwait(false);

                try
                {
                    if (accounts.Any())
                        authenticationResult = await publicClientApplication.AcquireTokenSilentAsync(scopes, accounts.FirstOrDefault()).ConfigureAwait(false);
                    else
                        authenticationResult = await publicClientApplication.AcquireTokenByIntegratedWindowsAuthAsync(scopes).ConfigureAwait(false);
                }
                catch (Exception) // AcquireTokenByIntegratedWindowsAuth will not be possible for managed users
                {
                    try
                    {
                        authenticationResult = await publicClientApplication.AcquireTokenAsync(scopes).ConfigureAwait(false);
                    }
                    catch (Exception exception)
                    {
                        throw exception;
                    }
                }

                return authenticationResult.AccessToken;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<string> GetTokenForClientAsync()
        {
            AuthenticationResult authenticationResult = null;
            var confidentialClientApplication = clientApplication as ConfidentialClientApplication;
            try
            {
                authenticationResult = await confidentialClientApplication.AcquireTokenForClientAsync(scopes).ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(authenticationResult?.AccessToken))
                    authenticationResult = await confidentialClientApplication.AcquireTokenForClientAsync(scopes, true).ConfigureAwait(false);
                return authenticationResult.AccessToken;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SignOut()
        {
            var accounts = clientApplication.GetAccountsAsync().Result;

            if (accounts.Any())
                clientApplication.RemoveAsync(accounts.First());
        }
    }
}
