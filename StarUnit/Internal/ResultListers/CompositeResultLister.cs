using System.Collections.Generic;
using System.Linq;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.ResultListers
{
    internal class CompositeResultLister<TContext>
        : ICompositeResultLister<TContext>, IContextualResultLister<TContext>
        where TContext : new()
    {
        private readonly ICollection<IComponentResultLister<TContext>> _componentListers;


        public CompositeResultLister()
        {
            this._componentListers = new List<IComponentResultLister<TContext>>();
        }


        public void Add(IComponentResultLister<TContext> lister)
        {
            this._componentListers.Add(lister);
        }


        public void List(IEnumerable<ITraversableResult> results)
        {
            this.List(results.ToArray());
        }


        public void List(params ITraversableResult[] results)
        {
            var context = new TContext();

            foreach (ITraversableResult result in results)
            {
                this.PreProcess(result, context);
            }

            foreach (ITraversableResult result in results)
            {
                this.List(result, context);
            }
        }


        public void List(ITraversableResult result, in TContext context)
        {
            this.GetLister(result).List(result, context);
        }


        public void PreProcess(ITraversableResult result, in TContext context)
        {
            this.GetLister(result).PreProcess(result, context);
        }


        private IComponentResultLister<TContext> GetLister(ITraversableResult result)
        {
            return this._componentListers.First(lister => lister.MayHandle(result));
        }
    }
}