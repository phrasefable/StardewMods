using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.Filterers.Wrappers
{
    internal class TraversableGroupingWrapperFactory : IBranchWrapperFactory<ITraversableGrouping>
    {
        public ITraversableGrouping Wrap(ITraversableGrouping grouping, IEnumerable<ITraversable> children)
        {
            return new TraversableGroupingWrapper(grouping, children);
        }


        private class TraversableGroupingWrapper : ITraversableGrouping
        {
            private readonly ITraversableGrouping _grouping;


            public TraversableGroupingWrapper(ITraversableGrouping grouping, IEnumerable<ITraversable> children)
            {
                this._grouping = grouping;
                this.Children = children;
            }


            public string Key => this._grouping.Key;
            public string LongName => this._grouping.LongName;
            public IEnumerable<Func<IResult>> Conditions => this._grouping.Conditions;
            public Delay Delay => this._grouping.Delay;

            public IEnumerable<ITraversable> Children { get; }
        }
    }
}