using System;

namespace NetGoLynx.Models.Configuration.Authentication
{
    /// <summary>
    /// Describes a configuration object for Okta.
    /// </summary>
    /// <remarks>
    /// To get this information, create a new Okta web app in your okta
    /// </remarks>
    public class Okta
    {
        /// <summary>
        /// Gets a value indicating whether this configuration element is enabled. Defaults to false.
        /// </summary>
        /// <remarks>
        /// If this configuration section is missing, this value will indicate that the other values
        /// in this configuration are not valid and should be discarded.
        /// </remarks>
        public bool Enabled => !string.IsNullOrEmpty(ClientId);

        /// <summary>
        /// The OAuth Client ID
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the OAuth Client Secret
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the OAuth auth endpoint URL
        /// </summary>
        /// <remarks>
        /// No /default/ between oauth2 and v1 here because that only applies to
        /// custom auth servers: https://developer.okta.com/docs/reference/api/oidc/
        /// </remarks>
        public string AuthorizationEndpoint => $"{OktaDomain}/oauth2/v1/authorize";

        /// <summary>
        /// Gets or sets the OAuth token endpoint URL
        /// </summary>
        /// <remarks>
        /// No /default/ between oauth2 and v1 here because that only applies to
        /// custom auth servers: https://developer.okta.com/docs/reference/api/oidc/
        /// </remarks>
        public string TokenEndpoint => $"{OktaDomain}/oauth2/v1/token";

        /// <summary>
        /// Gets or sets the user information URL
        /// </summary>
        /// <remarks>
        /// No /default/ between oauth2 and v1 here because that only applies to
        /// custom auth servers: https://developer.okta.com/docs/reference/api/oidc/
        /// </remarks>
        public string UserInformationEndpoint => $"{OktaDomain}/oauth2/v1/userinfo";

        /// <summary>
        /// Gets or sets the okta domain.
        /// </summary>
        public Uri OktaDomain { get; set; }

        /// <summary>
        /// Gets the authentication scheme for Google OAuth.
        /// </summary>
        public static string AuthenticationScheme => "Okta";
    }
}
