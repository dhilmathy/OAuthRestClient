using System;
using System.Collections.Generic;

namespace MsalRestClient
{
    public interface IConfigProvider
    {
        /// <summary>
        /// Base server URL
        /// </summary>
        /// <returns></returns>
        Uri GetBaseUrl();

        /// <summary>
        /// Client ID (also named Application ID) of the application as registered in the
        /// application registration portal (https://aka.ms/msal-net-register-app/).
        /// </summary>
        /// <returns></returns>
        string GetClientId();
        /// <summary>
        /// URL where the STS will call back the application with the security token. REQUIRED
        /// </summary>
        /// <returns></returns>
        string GetRedirectUri();
        /// <summary>
        /// Application secret key
        /// </summary>
        /// <returns></returns>
        string GetClientSecret();
        /// <summary>
        /// Scopes requested to access a protected API
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetScopes();
        /// <summary>
        /// Authority of the security token service (STS) from which MSAL.NET will acquire
        /// the tokens. Usual authorities are: https://login.microsoftonline.com/tenant/,
        /// where tenant is the tenant ID of the Azure AD tenant or a domain associated with
        /// this Azure AD tenant, in order to sign-in user of a specific organization only
        /// https://login.microsoftonline.com/common/ to signing users with any work and
        /// school accounts or Microsoft personal account https://login.microsoftonline.com/organizations/
        /// to signing users with any work and school accounts https://login.microsoftonline.com/consumers/
        /// to signing users with only personal Microsoft account (live) Note that this setting
        /// needs to be consistent with what is declared in the application registration
        /// portal
        /// </summary>
        /// <returns></returns>
        string GetAuthority();
    }
}
