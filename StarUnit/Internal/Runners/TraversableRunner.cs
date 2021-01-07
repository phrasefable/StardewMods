using System;
using JetBrains.Annotations;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;
using Phrasefable.StardewMods.StarUnit.Internal.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.Runners
{
    public abstract class TraversableRunner<T> : IComponentRunner where T : ITraversable
    {
        public bool MayHandle(ITraversable node)
        {
            return node is T;
        }

        public ITraversableResult Run(ITraversable node)
        {
            return this.CheckConditions((T) node) ?? this.TryRunNode((T) node);
        }

        public ITraversableResult Skip(ITraversable node)
        {
            return this._Skip((T) node);
        }

        protected abstract ITraversableResult _Skip(T test);
        protected abstract ITraversableResult _Run(T test);

        private ITraversableResult TryRunNode(T node)
        {
            try
            {
                return this._Run(node);
            }
            catch (Exception e)
            {
                return this.SkipAndOverrideResult(node, Status.Error, e.ToString());
            }
        }

        [CanBeNull]
        private ITraversableResult CheckConditions(T node)
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


        private ITraversableResult SkipAndOverrideResult(T node, Status status, string message)
        {
            var result = (TraversableResult) this.Skip(node);
            result.Status = status;
            result.Message = message;
            return result;
        }
    }
}
