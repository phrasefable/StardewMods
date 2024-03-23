using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Framework
{
    public interface IResultLister
    {
        public void List(params ITraversableResult[] results);
        public void List(IEnumerable<ITraversableResult> results);
    }


    public interface IContextualResultLister<TContext>
    {
        public void List(ITraversableResult result, in TContext context);
        public void PreProcess(ITraversableResult result, in TContext context);
    }


    public interface ICompositeResultLister<TContext> : IResultLister
    {
        public void Add(IComponentResultLister<TContext> lister);
    }


    public interface IComponentResultLister<TContext> : IContextualResultLister<TContext>
    {
        public bool MayHandle(ITraversableResult result);
    }
}
