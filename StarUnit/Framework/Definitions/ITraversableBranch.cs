using System.Collections.Generic;

namespace Phrasefable.StardewMods.StarUnit.Framework.Definitions
{
    public interface ITraversableBranch : ITraversable
    {
        public IEnumerable<ITraversable> Children { get; }
    }
}