using System.Collections.Generic;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.Results
{
    internal class BranchResult : TraversableResult, IBranchResult
    {
        IReadOnlyDictionary<Status, int> IBranchResult.DescendantLeafTallies => this.DescendantLeafTallies;
        IEnumerable<ITraversableResult> IBranchResult.Children => this.Children;

        public Dictionary<Status, int> DescendantLeafTallies { get; } = new Dictionary<Status, int>();
        public ICollection<ITraversableResult> Children { get; } = new List<ITraversableResult>();

        public int TotalDescendantLeaves { get; set; }

        public BranchResult(ITraversableBranch branch)
        {
            this.Key = branch.Key;
            this.LongName = branch.LongName;
        }
    }
}
