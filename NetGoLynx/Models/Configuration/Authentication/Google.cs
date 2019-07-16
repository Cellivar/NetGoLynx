using Microsoft.AspNetCore.Authentication.Google;

namespace NetGoLynx.Models.Configuration.Authentication
{
    /// <summary>
    /// Represents a configuration for Google's OAuth endpoint.
    /// </summary>
    /// <remarks>
    /// To get this information, you must generate it using the google setup instructions
    /// available here: https://developers.google.com/identity/sign-in/web/sign-in#before_you_begin
    /// </remarks>
    public class Google
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
        /// Gets or sets the Google OAuth client ID.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the Google OAuth client secret.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets the authentication scheme for Google OAuth.
        /// </summary>
        public static string AuthenticationScheme => GoogleDefaults.AuthenticationScheme;
    }
}
