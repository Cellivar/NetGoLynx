using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NetGoLynx.Models
{
    /// <summary>
    /// User information stored in the database.
    /// </summary>
    public class Account : IAccount
    {
        /// <summary>
        /// The ID of this email account
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// The name of this account, usually an email.
        /// </summary>
        /// <remarks>
        /// Email collisions are considered to be safe as the email information is coming from
        /// an external source. Group name collisions are not safe as the group names may
        /// not be be from a trusted source. Group names are expected to be prefixed as a result
        /// with {SourceName}_{GroupName}.
        /// </remarks>
        [Required(ErrorMessage = "Owner name must be supplied.")]
        [DisplayName("Owner Name")]
        public string Name { get; set; }

        /// <summary>
        /// The list of redirects this account owns.
        /// </summary>
        public List<Redirect> Redirects { get; set; }

        /// <summary>
        /// Gets the access level for this account.
        /// </summary>
        public AccessType Access { get; set; } = AccessType.Default;

        /// <summary>
        /// Gets a value indicating whether this account may view a redirect.
        /// </summary>
        /// <param name="redirect">The redirect to determine authorization to see.</param>
        /// <returns>True if the user may view a redirect, otherwise false.</returns>
        public bool MayView(IRedirect redirect)
        {
            if (Access.HasFlag(AccessType.Admin))
            {
                return true;
            }

            return redirect?.AccountId == AccountId;
        }
    }
}
