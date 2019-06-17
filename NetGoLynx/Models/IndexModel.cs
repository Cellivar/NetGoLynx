namespace NetGoLynx.Models
{
    public class IndexModel
    {
        public IndexModel(Operation op, string suggestedLinkName)
        {
            Op = op;
            LinkName = suggestedLinkName;
        }

        public Operation Op { get; set; }
        public string LinkName { get; private set; }

        public enum Operation
        {
            List,
            Add,
        }
    }
}
