namespace NetGoLynx.Models.RedirectModels
{
    /// <summary>
    /// Model for adding a new redirect
    /// </summary>
    public class RedirectMetadata : Redirect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectMetadata"/> class.
        /// </summary>
        public RedirectMetadata() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectMetadata"/> class.
        /// </summary>
        /// <param name="name">The name of the link.</param>
        /// <param name="id">The ID of the link</param>
        public RedirectMetadata(string name, int id = 0)
        {
            Name = name;
            RedirectId = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectMetadata"/> class.
        /// </summary>
        /// <param name="redirect">The redirect to copy</param>
        public RedirectMetadata(Redirect redirect)
        {
            RedirectId = redirect.RedirectId;
            Name = redirect.Name;
            Target = redirect.Target;
            Description = redirect.Description;
        }

        /// <summary>
        /// An error message to display.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets a redirect from this new redirect model.
        /// </summary>
        /// <returns></returns>
        public Redirect ToRedirect()
        {
            return new Redirect()
            {
                Name = Name,
                Description = Description,
                Target = Target
            };
        }
    }
}
