using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;

namespace Phrasefable.StardewMods.StarUnit.Internal.Runners
{
    internal class GroupingRunner : BranchRunner<ITraversableGrouping>
    {
        public GroupingRunner(IRunnerDelegator delegator) : base(delegator) { }
    }
}