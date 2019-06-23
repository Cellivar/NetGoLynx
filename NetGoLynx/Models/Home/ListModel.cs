using System.Collections.Generic;

namespace NetGoLynx.Models.Home
{
    /// <summary>
    /// Model for listing redirects
    /// </summary>
    public class ListModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListModel"/> class.
        /// </summary>
        /// <param name="redirects">The list of redirects to display</param>
        /// <param name="id">The redirect ID to highlight</param>
        /// <param name="name">The redirect name to highlight</param>
        public ListModel(
            IEnumerable<Redirect> redirects = null,
            int id = -1,
            string name = "")
        {
            Redirects = redirects;
            HighlightRedirectId = id;
            HighlightRedirectName = name;
        }

        /// <summary>
        /// The list of redirects to display
        /// </summary>
        public IEnumerable<Redirect> Redirects { get; }

        /// <summary>
        /// The redirect name to highlight
        /// </summary>
        public string HighlightRedirectName { get; set; }

        /// <summary>
        /// The redirect ID to highlight
        /// </summary>
        public int HighlightRedirectId { get; private set; }
    }
}
