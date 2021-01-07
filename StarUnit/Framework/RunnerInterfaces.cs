using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Framework
{
    public interface IRunner
    {
        public ITraversableResult Run(ITraversable node);
        public ITraversableResult Skip(ITraversable node);
    }


    public interface ICompositeRunner : IRunner
    {
        public void Add(IComponentRunner runner);
    }


    public interface IComponentRunner : IRunner
    {
        public bool MayHandle(ITraversable node);
    }
}