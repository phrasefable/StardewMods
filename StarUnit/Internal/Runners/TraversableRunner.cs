using System;
using JetBrains.Annotations;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;
using Phrasefable.StardewMods.StarUnit.Internal.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.Runners
{
    internal class TraversableRunner : ICompositeRunner
    {
        private readonly ICompositeRunner _runners = new CompositeRunner();


        public ITraversableResult Run(ITraversable node)
        {
            return this.TryRunNodeIfAble(node, () => this._runners.Run(node));
        }


        public ITraversableResult Run(ITraversable node, IExecutionContext context)
        {
            return this.TryRunNodeIfAble(node, () => this._runners.Run(node, context));
        }


        public ITraversableResult Skip(ITraversable node)
        {
            return this._runners.Skip(node);
        }


        public void Add(IComponentRunner component)
        {
            this._runners.Add(component);
        }


        private ITraversableResult TryRunNodeIfAble(ITraversable node, Func<ITraversableResult> runner)
        {
            return this.CheckConditions(node) ?? this.TryRunNode(node, runner);
        }


        private ITraversableResult TryRunNode(ITraversable node, Func<ITraversableResult> runner)
        {
            try
            {
                return runner.Invoke();
            }
            catch (Exception e)
            {
                return this.SkipAndOverrideResult(node, Status.Error, e.ToString());
            }
        }


        [CanBeNull]
        private ITraversableResult CheckConditions(ITraversable node)
        {
            var i = 0;
            foreach (Func<IResult> condition in node.Conditions)
            {
                IResult testResult;
                try
                {
                    testResult = condition.Invoke();
                }
                catch (Exception e)
                {
                    return this.SkipAndOverrideResult(
                        node,
                        Status.Error,
                        $"Error evaluating condition {i}: {e.Message}"
                    );
                }

                if (testResult.Status != Status.Pass)
                {
                    return this.SkipAndOverrideResult(node, Status.Fail, $"Condition {i} failed: {testResult.Message}");
                }

                i++;
            }

            return null;
        }


        private ITraversableResult SkipAndOverrideResult(ITraversable node, Status status, string message)
        {
            var result = (TraversableResult) this.Skip(node);
            result.Status = status;
            result.Message = message;
            return result;
        }
    }
}
