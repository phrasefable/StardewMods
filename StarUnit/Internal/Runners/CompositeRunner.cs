using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;

namespace Phrasefable.StardewMods.StarUnit.Internal.Runners
{
    internal class CompositeRunner : ICompositeRunner
    {
        private readonly ICollection<IComponentRunner> _runners = new List<IComponentRunner>();


        public void Add(IComponentRunner runner)
        {
            this._runners.Add(runner);
        }


        public void Run(OnCompleted @return, ITraversable node, IExecutionContext context)
        {
            this.RunnerFor(node).Run(@return, node, context);
        }


        public void Skip(OnCompleted @return, ITraversable node)
        {
            this.RunnerFor(node).Skip(@return, node);
        }


        public void Skip(OnCompleted @return, ITraversable node, Status status, string message)
        {
            this.RunnerFor(node).Skip(@return, node, status, message);
        }


        private IComponentRunner RunnerFor(ITraversable node)
        {
            return this._runners.First(runner => runner.MayHandle(node));
        }
    }
}