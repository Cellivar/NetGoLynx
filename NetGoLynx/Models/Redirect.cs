using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NetGoLynx.Data;

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
        [Required(ErrorMessage = "Link target is required")]
        [Url(ErrorMessage = "Link must be a valid URL")]
        public string Target { get; set; }

        /// <summary>
        /// The name of the redirect
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Link name is required")]
        [RegularExpressionNegate(@"^.*[{}|\\^~\[\]`;/?:@#=%&<>\s]+.*$", ErrorMessage = @"Unsafe URL characters not allowed: {} | ^ ~ [] ` ; /\ ? : @ # = % & <> and spaces.")]
        [StringLength(255, ErrorMessage = "Length limit of 255")]
        public string Name { get; set; }

        /// <summary>
        /// A description of this redirect
        /// </summary>
        [StringLength(1024, ErrorMessage = "Length limit of 1024")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the AccountID of the owner account.
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the account that owns this redirect.
        /// </summary>
        public Account Account { get; set; }
    }
}
