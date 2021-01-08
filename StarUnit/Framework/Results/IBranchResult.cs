using System.Collections.Generic;

namespace Phrasefable.StardewMods.StarUnit.Framework.Results
{
    public interface IBranchResult : ITraversableResult
    {
        public IReadOnlyDictionary<Status, int> DescendantLeafTallies { get; }
        public int TotalDescendantLeaves { get; }
        public IEnumerable<ITraversableResult> Children { get; }
    }
}