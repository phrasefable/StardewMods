using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;
using Phrasefable.StardewMods.StarUnit.Internal.Definitions;

namespace Phrasefable.StardewMods.StarUnit.Internal.Builders
{
    internal class TraversableBranchBuilder : ITraversableBuilder
    {
        private readonly IList<ITraversable> _children = new List<ITraversable>();
        private readonly TraversableBuilder _traversableBuilder = new();
        private ICollection<string> Keys { get; } = new HashSet<string>();


        public string Key
        {
            set => this._traversableBuilder.Key = value;
        }


        public string LongName
        {
            set => this._traversableBuilder.LongName = value;
        }


        public Delay Delay
        {
            set => this._traversableBuilder.Delay = value;
        }


        public void AddCondition(Func<IResult> condition)
        {
            this._traversableBuilder.AddCondition(condition);
        }


        public void AddChild(ITraversable child)
        {
            if (this.Keys.Contains(child.Key))
            {
                throw new ArgumentException($"May not add child with duplicate key `{child.Key}`.", nameof(child));
            }

            this._children.Add(child);
            this.Keys.Add(child.Key);
        }


        public void Build(TraversableBranch branch)
        {
            this._traversableBuilder.Build(branch);

            foreach (ITraversable traversable in this._children)
            {
                branch.Children.Add(traversable);
            }
        }
    }
}