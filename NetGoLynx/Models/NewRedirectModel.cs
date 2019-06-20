using System.ComponentModel.DataAnnotations;
using NetGoLynx.Data;

namespace NetGoLynx.Models
{
    public class NewRedirectModel
    {
        public NewRedirectModel() { }

        public NewRedirectModel(string linkName)
        {
            LinkName = linkName;
        }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Link name is required")]
        [RegularExpressionNegate(@"^.*[{}|\\^~\[\]`;/?:@#=%&<>\s]+.*$", ErrorMessage = @"Unsafe URL characters not allowed: {} | ^ ~ [] ` ; /\ ? : @ # = % & <> and spaces.")]
        [StringLength(255, ErrorMessage = "Length limit of 255")]
        public string LinkName { get; set; }

        [Required(ErrorMessage = "Link target is required")]
        [Url(ErrorMessage = "Link must be a valid URL")]
        public string Target { get; set; }

        [StringLength(1024, ErrorMessage = "Length limit of 1024")]
        public string Description { get; set; }

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
