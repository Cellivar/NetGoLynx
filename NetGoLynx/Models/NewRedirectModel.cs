using System;
using System.ComponentModel.DataAnnotations;

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
        [RegularExpression("^[A-Za-z0-9-$_.!*()+]*$", ErrorMessage = "No spaces, allowed characters: ! $ * ( ) - _ +")]
        [StringLength(255, ErrorMessage = "Length limit of 255")]
        public string LinkName { get; set; }

        [Required(ErrorMessage = "Link target is required")]
        [RegularExpression("^https?://", ErrorMessage = "Link must start with http:// or https://")]
        public Uri Target { get; set; }

        public string ErrorMessage { get; set; }
    }
}
