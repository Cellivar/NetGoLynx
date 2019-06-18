using System.Collections.Generic;

namespace NetGoLynx.Models
{
    public class IndexModel
    {
        public IndexModel(
            string suggestedLinkName = "",
            IEnumerable<Redirect> redirects = null,
            int id = -1)
        {
            LinkName = suggestedLinkName;
            Redirects = redirects;
            HighlightRedirectId = id;
        }

        public string LinkName { get; }

        public IEnumerable<Redirect> Redirects { get; }

        public int HighlightRedirectId { get; private set; }
    }
}
