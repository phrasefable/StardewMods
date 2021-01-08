using System.Collections.Generic;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;

namespace Phrasefable.StardewMods.StarUnit.Internal.Definitions
{
    internal class TraversableBranch : Traversable, ITraversableBranch
    {
        IEnumerable<ITraversable> ITraversableBranch.Children => this.Children;
        public ICollection<ITraversable> Children { get; } = new List<ITraversable>();
    }
}