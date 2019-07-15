using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NetGoLynx.Models
{
    /// <summary>
    /// User information stored in the database.
    /// </summary>
    public class Account
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
    }
}
