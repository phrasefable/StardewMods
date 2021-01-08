using System;
using System.Collections.Generic;
using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Internal.Definitions;

namespace Phrasefable.StardewMods.StarUnit.Internal.Builders
{
    internal class TraversableBranchBuilder : IIdentifiableBuilder
    {
        private readonly IList<ITraversable> _children = new List<ITraversable>();
        private readonly IdentifiableBuilder _identifiableBuilder = new IdentifiableBuilder();
        private ICollection<string> Keys { get; } = new HashSet<string>();


        public string Key
        {
            set => this._identifiableBuilder.Key = value;
        }


        public string LongName
        {
            set => this._identifiableBuilder.LongName = value;
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
            this._identifiableBuilder.Build(branch);

            foreach (ITraversable traversable in this._children)
            {
                branch.Children.Add(traversable);
            }
        }
    }
}