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
        public string AuthorizationEndpoint => $"{OktaDomain}/oauth2/default/v1/authorize";

        /// <summary>
        /// Gets or sets the OAuth token endpoint URL
        /// </summary>
        public string TokenEndpoint => $"{OktaDomain}/oauth2/default/v1/token";

        /// <summary>
        /// Gets or sets the user information URL
        /// </summary>
        public string UserInformationEndpoint => $"{OktaDomain}/oauth2/default/v1/userinfo";

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
