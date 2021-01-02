using System.Collections.Generic;

namespace Phrasefable.StardewMods.StarUnit.Api.Model
{
    public interface ITraversableBranch<out TChildren> : ITraversable where TChildren : ITraversable
    {
        public IEnumerable<TChildren> Children { get; }
    }
}