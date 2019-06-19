using System.ComponentModel;

namespace NetGoLynx.Models
{
    /// <summary>
    /// Information for a redirection
    /// </summary>
    public class Redirect
    {
        /// <summary>
        /// The unique ID of this redirect
        /// </summary>
        [DisplayName("ID")]
        public int RedirectId { get; set; }

        /// <summary>
        /// The target URL to redirect to
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// The name of the redirect
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A description of this redirect
        /// </summary>
        public string Description { get; set; }
    }
}
