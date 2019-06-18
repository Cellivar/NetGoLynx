using System;

namespace NetGoLynx.Models
{
    public class NewRedirectModel
    {
        public NewRedirectModel() { }

        public NewRedirectModel(string linkName)
        {
            LinkName = linkName;
        }

        public string LinkName { get; set; }

        public Uri Target { get; set; }

        public string ErrorMessage { get; set; }
    }
}
