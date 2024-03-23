using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.Runners
{
    internal abstract class TraversableRunner<T> : IComponentRunner where T : ITraversable
    {
        public bool MayHandle(ITraversable node)
        {
            return node is T;
        }


        public void Run(OnCompleted @return, ITraversable node, IExecutionContext context)
        {
            this.TryRunNodeIfAble(@return, (T) node, context);
        }


        public void Skip(OnCompleted @return, ITraversable node)
        {
            this.Skip(@return, node, Status.Skipped, null);
        }


        public void Skip(OnCompleted @return, ITraversable node, Status status, string message)
        {
            this.Skip(@return, (T) node, status, message);
        }


        // ========== Abstract Methods (more specific traversable) =====================================================


        protected abstract void Run(OnCompleted @return, T grouping, IExecutionContext context);
        protected abstract void Skip(OnCompleted @return, T grouping, Status status, string message);


        // ========== Traversable Handling =============================================================================


        private void TryRunNodeIfAble(OnCompleted @return, T traversable, IExecutionContext context)
        {
            if (this.CheckConditions(@return, traversable))
            {
                this.TryRunNode(@return, traversable, context);
            }
        }


        private void TryRunNode(OnCompleted @return, T traversable, IExecutionContext context)
        {
            try
            {
                this.Run(@return, traversable, context);
            }
            catch (Exception e)
            {
                this.Skip(@return, traversable, Status.Error, e.ToString());
            }
        }


        private bool CheckConditions(OnCompleted @return, ITraversable node)
        {
            int i = 0;

            foreach (Func<IResult> condition in node.Conditions)
            {
                IResult testResult = TryRunCondition(condition, out string errorMessage);

                if (testResult == null)
                {
                    this.Skip(@return, node, Status.Error, $"Error evaluating condition {i}: {errorMessage}");
                    return false;
                }

                if (testResult.Status != Status.Pass)
                {
                    this.Skip(@return, node, Status.Fail, $"Condition {i} failed: {testResult.Message}");
                    return false;
                }

                i++;
            }

            return true;
        }


        private static IResult TryRunCondition(Func<IResult> condition, out string errorMessage)
        {
            try
            {
                errorMessage = null;
                return condition.Invoke();
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return null;
            }
        }
    }
}
