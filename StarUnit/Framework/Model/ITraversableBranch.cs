using System.Collections.Generic;

namespace Phrasefable.StardewMods.StarUnit.Framework.Model
{
    public interface ITraversableBranch<out TChildren> : ITraversable where TChildren : ITraversable
    {
        public IEnumerable<TChildren> Children { get; }
    }
}