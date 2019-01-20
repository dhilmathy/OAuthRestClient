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
        private readonly PublicClientApplication publicClientApp;
        private readonly IEnumerable<string> scopes;
        
        public AuthHelper(IConfigProvider configProvider)
        {
            clientId = configProvider.GetClientId();
            scopes = configProvider.GetScopes();

            publicClientApp = new PublicClientApplication(clientId, configProvider.GetAuthority(), TokenCacheHelper.GetUserCache());
        }

        public async Task<string> GetTokenForCurrentAccountAsync()
        {
            try
            {
                AuthenticationResult authResult = null;

                var accounts = await publicClientApp.GetAccountsAsync().ConfigureAwait(false);

                try
                {
                    if (accounts.Any())
                        authResult = await publicClientApp.AcquireTokenSilentAsync(scopes, accounts.FirstOrDefault()).ConfigureAwait(false);
                    else
                        authResult = await publicClientApp.AcquireTokenByIntegratedWindowsAuthAsync(scopes).ConfigureAwait(false);
                }
                catch (Exception) // AcquireTokenByIntegratedWindowsAuth will not be possible for managed users
                {
                    try
                    {
                        authResult = await publicClientApp.AcquireTokenAsync(scopes).ConfigureAwait(false);
                    }
                    catch (Exception exception)
                    {
                        throw exception;
                    }
                }

                return authResult.AccessToken;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SignOut()
        {
            var accounts = publicClientApp.GetAccountsAsync().Result;

            if (accounts.Any())
                publicClientApp.RemoveAsync(accounts.First());
        }
    }
}
