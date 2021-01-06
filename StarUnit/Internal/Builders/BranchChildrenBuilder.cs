using System;
using System.Collections.Generic;
using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Model;

namespace Phrasefable.StardewMods.StarUnit.Internal.Builders
{
    internal class BranchChildrenBuilder<TChildren> : IBuilder<IEnumerable<TChildren>> where TChildren : ITraversable
    {
        private readonly IList<TChildren> _children = new List<TChildren>();
        private ICollection<string> Keys { get; } = new HashSet<string>();

        public void AddChild(TChildren child)
        {
            if (this.Keys.Contains(child.Key))
            {
                throw new ArgumentException($"May not add child with duplicate key `{child.Key}`.", nameof(child));
            }

            this._children.Add(child);
            this.Keys.Add(child.Key);
        }

        public IEnumerable<TChildren> Build()
        {
            return this._children;
        }
    }
}
