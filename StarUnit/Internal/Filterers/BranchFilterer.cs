using System.Linq;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Internal.Filterers.Wrappers;

namespace Phrasefable.StardewMods.StarUnit.Internal.Filterers
{
    internal class BranchFilterer<T> : IComponentFilterer where T : ITraversableBranch
    {
        private readonly IFilterer _childFilterer;
        private readonly IBranchWrapperFactory<T> _wrapperFactory;


        public BranchFilterer(IFilterer childFilterer, IBranchWrapperFactory<T> wrapperFactory)
        {
            this._childFilterer = childFilterer;
            this._wrapperFactory = wrapperFactory;
        }


        public bool MayHandle(ITraversable node)
        {
            return node is T;
        }


        public ITraversable Filter(ITraversable node, IStringNode filter)
        {
            return this._Filter((T) node, filter);
        }


        private ITraversable _Filter(T branch, IStringNode filter)
        {
            IStringNode[] childFilters = filter.Children.ToArray();
            return this._wrapperFactory.Wrap(
                branch,
                branch.Children
                    .Select(child => this._childFilterer.Filter(child, childFilters))
                    .Where(filteredChild => !(filteredChild is null))
            );
        }
    }
}
