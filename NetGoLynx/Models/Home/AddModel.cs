using System.ComponentModel.DataAnnotations;
using NetGoLynx.Data;

namespace NetGoLynx.Models.Home
{
    /// <summary>
    /// Model for adding a new redirect
    /// </summary>
    public class AddModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddModel"/> class.
        /// </summary>
        public AddModel() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddModel"/> class.
        /// </summary>
        /// <param name="linkName">The name of the link to create.</param>
        public AddModel(string linkName)
        {
            LinkName = linkName;
        }

        /// <summary>
        /// The name of the link, which will be used for go/ lookup.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Link name is required")]
        [RegularExpressionNegate(@"^.*[{}|\\^~\[\]`;/?:@#=%&<>\s]+.*$", ErrorMessage = @"Unsafe URL characters not allowed: {} | ^ ~ [] ` ; /\ ? : @ # = % & <> and spaces.")]
        [StringLength(255, ErrorMessage = "Length limit of 255")]
        public string LinkName { get; set; }

        /// <summary>
        /// The target URL for this redirect, which will be redirected to.
        /// </summary>
        [Required(ErrorMessage = "Link target is required")]
        [Url(ErrorMessage = "Link must be a valid URL")]
        public string Target { get; set; }

        /// <summary>
        /// The description for this redirect.
        /// </summary>
        [StringLength(1024, ErrorMessage = "Length limit of 1024")]
        public string Description { get; set; }

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
                Name = LinkName,
                Description = Description,
                Target = Target
            };
        }
    }
}
