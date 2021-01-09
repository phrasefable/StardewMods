using System;
using System.Linq;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;
using Phrasefable.StardewMods.StarUnit.Internal.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.Runners
{
    internal abstract class BranchRunner<T> : ComponentRunner<T> where T : ITraversableBranch
    {
        protected readonly ITraversableRunner ChildRunner;


        protected BranchRunner(ITraversableRunner childRunner)
        {
            this.ChildRunner = childRunner;
        }


        protected override ITraversableResult _Skip(T branch)
        {
            return this.HandleChildren(
                branch,
                Status.Skipped,
                this.ChildRunner.Skip
            );
        }


        protected IBranchResult HandleChildren(
            ITraversableBranch branch,
            Status status,
            Func<ITraversable, ITraversableResult> childConsumer)
        {
            var result = new SelfAggregatingBranchResult(branch) {Status = status};

            foreach (ITraversableResult childResult in branch.Children.Select(childConsumer))
            {
                result.AddChild(childResult);
            }

            return result;
        }
    }
}
