using System;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Internal.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.Runners
{
    internal abstract class BranchRunner<T> : TraversableRunner<T> where T : ITraversableBranch
    {
        protected readonly IRunnerDelegator Delegator;


        protected BranchRunner(IRunnerDelegator delegator)
        {
            this.Delegator = delegator;
        }


        protected override void Run(OnCompleted @return, T branch, IExecutionContext childContext)
        {
            HandleChildren(
                @return,
                branch,
                (returnChild, child) => this.Delegator.Run(returnChild, child, childContext),
                Status.Pass
            );
        }


        protected override void Skip(OnCompleted @return, T branch, Status status, string message)
        {
            HandleChildren(@return, branch, this.Delegator.Skip, status, message);
        }


        private static void HandleChildren(
            OnCompleted @return,
            ITraversableBranch branch,
            Action<OnCompleted, ITraversable> childConsumer,
            Status status,
            string message = null
        )
        {
            var result = new SelfAggregatingBranchResult(branch) { Status = status, Message = message };

            branch.Children.ForEach(() => @return(result), childConsumer, result.AddChild);
        }
    }
}
