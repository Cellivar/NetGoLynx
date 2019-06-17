using System.Collections.Generic;

namespace NetGoLynx.Models
{
    public class IndexModel
    {
        public IndexModel(
            string suggestedLinkName = "",
            IEnumerable<Redirect> redirects = null,
            int id = 0)
        {
            LinkName = suggestedLinkName;
            Redirects = redirects;
        }

        public string LinkName { get; }

        public IEnumerable<Redirect> Redirects { get; }
    }
}
