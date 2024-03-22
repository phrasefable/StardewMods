using System.Collections.Generic;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.Results
{
    internal class SelfAggregatingBranchResult : TraversableResult, IBranchResult
    {
        private readonly Dictionary<Status, int> _descendantLeafTallies = new();
        public IReadOnlyDictionary<Status, int> DescendantLeafTallies => this._descendantLeafTallies;

        private readonly ICollection<ITraversableResult> _children = new List<ITraversableResult>();
        public IEnumerable<ITraversableResult> Children => this._children;

        public int TotalDescendantLeaves { get; private set; }


        public SelfAggregatingBranchResult(ITraversableBranch branch)
        {
            this.Key = branch.Key;
            this.LongName = branch.LongName;
        }


        public void AddChild(ITraversableResult result)
        {
            this._children.Add(result);
            if (result is IBranchResult suiteResult)
            {
                this._descendantLeafTallies.AddToValues(suiteResult.DescendantLeafTallies);
                this.TotalDescendantLeaves += suiteResult.TotalDescendantLeaves;
            }
            else
            {
                this._descendantLeafTallies.AddToValue(result.Status, 1);
                this.TotalDescendantLeaves += 1;
            }
        }
    }
}
