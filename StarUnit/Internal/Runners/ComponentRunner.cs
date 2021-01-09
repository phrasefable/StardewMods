using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.Runners
{
    internal abstract class ComponentRunner<T> : IComponentRunner where T : ITraversable
    {
        public ITraversableResult Run(ITraversable node)
        {
            return this._Run((T) node);
        }


        public ITraversableResult Skip(ITraversable node)
        {
            return this._Skip((T) node);
        }


        public ITraversableResult Run(ITraversable node, IExecutionContext context)
        {
            return this._Run((T) node, context);
        }


        public bool MayHandle(ITraversable node)
        {
            return node is T;
        }


        protected abstract ITraversableResult _Run(T node);
        protected abstract ITraversableResult _Run(T node, IExecutionContext context);
        protected abstract ITraversableResult _Skip(T node);
    }
}
