using System.Collections.Generic;

namespace NetGoLynx.Models
{
    /// <summary>
    /// An account that may view some redirects.
    /// </summary>
    public interface IAccount
    {
        /// <summary>
        /// Gets the account's ID
        /// </summary>
        int AccountId { get; }

        /// <summary>
        /// Gets the account's unique name, usually an email.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a list of redirects this account owns.
        /// </summary>
        List<Redirect> Redirects { get; }

        /// <summary>
        /// Gets a value indicating whether this account may view a redirect.
        /// </summary>
        /// <param name="redirect">The redirect to determine authorization to see.</param>
        /// <returns>True if the user may view the redirect, otherwise false.</returns>
        bool MayView(IRedirect redirect);

        /// <summary>
        /// Gets the access type this account has.
        /// </summary>
        AccessType Access { get; }
    }
}
