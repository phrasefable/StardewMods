using System.Collections.Generic;

namespace Phrasefable.StardewMods.StarUnit.Framework.Results
{
    public interface ISuiteResult : ITraversableResult
    {
        public IReadOnlyDictionary<Status, int> DescendantLeafTallies { get; }
        public int TotalDescendantLeaves { get; }
        public IEnumerable<ITraversableResult> Children { get; }
    }
}
