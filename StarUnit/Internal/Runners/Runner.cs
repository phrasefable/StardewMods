using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;

namespace Phrasefable.StardewMods.StarUnit.Internal.Runners
{
    internal class Runner : ICompositeRunner, IRunnerDelegator, ITreeRunner
    {
        private readonly ICompositeRunner _runners = new CompositeRunner();
        private readonly IDelayedExecutor _delayer;


        public Runner(IDelayedExecutor delayer)
        {
            this._delayer = delayer;
        }


        // Tree runner
        public void Run(OnCompleted @return, ITraversable node)
        {
            this.Run(@return, node, new EmptyExecutionContext());
        }


        // Non-traversable ticking
        public void Run(Action @return, Action action, Delay delay)
        {
            void TheDelayedAction()
            {
                action();
                @return();
            }

            this._delayer
                .After(delay)
                .Invoke(TheDelayedAction);
        }


        public void Run(OnCompleted @return, ITraversable node, IExecutionContext context)
        {
            this._delayer.After(node.Delay).Invoke(() => this._runners.Run(@return, node, context));
        }


        public void Skip(OnCompleted @return, ITraversable node)
        {
            this._runners.Skip(@return, node);
        }


        public void Skip(OnCompleted @return, ITraversable node, Status status, string message)
        {
            this._runners.Skip(@return, node, status, message);
        }


        public void Add(IComponentRunner component)
        {
            this._runners.Add(component);
        }
    }
}