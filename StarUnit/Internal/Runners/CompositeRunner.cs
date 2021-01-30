using System.Collections.Generic;
using System.Linq;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.Runners
{
    internal class CompositeRunner : ICompositeRunner
    {
        private readonly ICollection<IComponentRunner> _runners = new List<IComponentRunner>();


        public void Add(IComponentRunner runner)
        {
            this._runners.Add(runner);
        }


        private IComponentRunner RunnerFor(ITraversable node)
        {
            return this._runners.First(runner => runner.MayHandle(node));
        }


        public ITraversableResult Run(ITraversable node)
        {
            return this.RunnerFor(node).Run(node);
        }


        public ITraversableResult Run(ITraversable node, IExecutionContext context)
        {
            return this.RunnerFor(node).Run(node, context);
        }


        public ITraversableResult Skip(ITraversable node)
        {
            return this.RunnerFor(node).Skip(node);
        }


        public ITraversableResult Skip(ITraversable node, Status status, string message)
        {
            return this.RunnerFor(node).Skip(node, status, message);
        }
    }
}