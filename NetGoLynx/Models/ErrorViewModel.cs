namespace NetGoLynx.Models
{
    /// <summary>
    /// Model for displaying errors
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// The request information.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Whether there is a request ID to display.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
