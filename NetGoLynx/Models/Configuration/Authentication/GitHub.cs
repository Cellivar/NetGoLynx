namespace NetGoLynx.Models.Configuration.Authentication
{
    /// <summary>
    /// Describes a configuration object for GitHub
    /// </summary>
    /// <remarks>
    /// To get this information, create a new GitHub OAuth application.
    /// See: https://developer.github.com/apps/building-oauth-apps/creating-an-oauth-app/
    /// </remarks>
    public class GitHub
    {
        /// <summary>
        /// Gets a value indicating whether this configuration element is enabled. Defaults to false.
        /// </summary>
        /// <remarks>
        /// If this configuration section is missing, this value will indicate that the other values
        /// in this configuration are not valid and should be discarded.
        /// </remarks>
        public bool Enabled { get; set; } = false;

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
        public string AuthorizationEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the OAuth token endpoint URL
        /// </summary>
        public string TokenEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the user information URL
        /// </summary>
        public string UserInformationEndpoint { get; set; }
    }
}
