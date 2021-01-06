using System.Collections.Generic;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Model;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.Results
{
    internal class SuiteResult : TraversableResult, ISuiteResult
    {
        IReadOnlyDictionary<Status, int> ISuiteResult.DescendantLeafTallies => this.DescendantLeafTallies;
        IEnumerable<ITraversableResult> ISuiteResult.Children => this.Children;

        public Dictionary<Status, int> DescendantLeafTallies { get; } = new Dictionary<Status, int>();
        public IList<ITraversableResult> Children { get; } = new List<ITraversableResult>();

        public int TotalDescendantLeaves { get; set; }

        public SuiteResult(ITestSuite suite)
        {
            this.Key = suite.Key;
            this.LongName = suite.LongName;
        }
    }
}
