using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NetGoLynx.Models.RedirectModels
{
    /// <summary>
    /// Model for listing all redirects
    /// </summary>
    public class ListAllModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListAllModel"/> class.
        /// </summary>
        /// <param name="redirects">The list of redirects to display, with accounts populated.</param>
        public ListAllModel(IEnumerable<IRedirect> redirects = null)
        {
            Redirects = redirects.Select(r => new RedirectWithOwnerName
            {
                Name = r.Name,
                Description = r.Description,
                Target = r.Target,
                OwnerName = r.Account.Name,
                RedirectId = r.RedirectId
            });
        }

        /// <summary>
        /// Gets the list of redirects with their owner names.
        /// </summary>
        public IEnumerable<RedirectWithOwnerName> Redirects { get; }

        /// <summary>
        /// Redirect with the owner name cached for easier serialization to JSON.
        /// </summary>
        public class RedirectWithOwnerName : Redirect
        {
            /// <summary>
            /// Gets or sets the owner name for this redirect.
            /// </summary>
            [DisplayName("Owner Name")]
            public string OwnerName { get; set; }
        }
    }
}
