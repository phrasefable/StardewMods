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
            return this._Skip(branch, Status.Skipped, null);
        }


        protected override ITraversableResult _Skip(T branch, Status status, string message)
        {
            return this.HandleChildren(
                branch,
                this.ChildRunner.Skip,
                status,
                message
            );
        }


        protected IBranchResult HandleChildren(
            ITraversableBranch branch,
            Func<ITraversable, ITraversableResult> childConsumer,
            Status status,
            string message = null)
        {
            var result = new SelfAggregatingBranchResult(branch) {Status = status, Message = message};

            foreach (ITraversableResult childResult in branch.Children.Select(childConsumer))
            {
                result.AddChild(childResult);
            }

            return result;
        }
    }
}
