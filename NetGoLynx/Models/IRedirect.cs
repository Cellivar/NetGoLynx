namespace NetGoLynx.Models
{
    /// <summary>
    /// Information for a redirection
    /// </summary>
    public interface IRedirect
    {
        /// <summary>
        /// The unique ID of this redirect
        /// </summary>
        int RedirectId { get; }

        /// <summary>
        /// The target URL to redirect to
        /// </summary>
        string Target { get; }

        /// <summary>
        /// The name of the redirect
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A description of this redirect
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets or sets the AccountID of the owner account.
        /// </summary>
        int AccountId { get; }
    }
}
