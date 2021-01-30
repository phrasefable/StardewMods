using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.Runners
{
    internal class GroupingRunner : BranchRunner<ITraversableGrouping>
    {
        public GroupingRunner(ITraversableRunner childRunner) : base(childRunner) { }


        protected override ITraversableResult _Run(ITraversableGrouping grouping)
        {
            return this.HandleChildren(
                grouping,
                this.ChildRunner.Run,
                Status.Pass
            );
        }


        protected override ITraversableResult _Run(ITraversableGrouping grouping, IExecutionContext context)
        {
            return this.HandleChildren(
                grouping,
                child => this.ChildRunner.Run(child, context),
                Status.Pass
            );
        }
    }
}
