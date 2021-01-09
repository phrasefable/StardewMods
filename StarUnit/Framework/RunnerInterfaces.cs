using System;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Framework
{
    public interface IRunner
    {
        public ITraversableResult Run(ITraversable node);
        public ITraversableResult Skip(ITraversable node);
    }


    public interface IExecutionContext
    {
        public ITraversableResult Execute(ITraversable traversable, Func<ITraversable, ITraversableResult> executable);
    }


    public interface ITraversableRunner : IRunner
    {
        public ITraversableResult Run(ITraversable node, IExecutionContext context);
    }


    public interface IComponentRunner : ITraversableRunner
    {
        public bool MayHandle(ITraversable node);
    }


    public interface ICompositeRunner : ITraversableRunner
    {
        public void Add(IComponentRunner component);
    }
}
