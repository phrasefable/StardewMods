namespace Phrasefable.StardewMods.StarUnit.Internal.Filterers
{
    internal class StringNode : IStringNode
    {
        private readonly ICollection<StringNode> _children = new List<StringNode>();


        public StringNode(string key = "")
        {
            this.Key = key;
        }


        public IEnumerable<IStringNode> Children => this._children;

        public string Key { get; }
        public bool AllChildren { get; set; }


        public StringNode GetChildElseAdd(string childKey)
        {
            foreach (StringNode child in this._children)
            {
                if (child.Key == childKey) return child;
            }

            var newChild = new StringNode(childKey);
            this._children.Add(newChild);
            return newChild;
        }
    }
}
