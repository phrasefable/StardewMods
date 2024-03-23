using System.Collections.Generic;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;

namespace Phrasefable.StardewMods.StarUnit.Internal.Filterers
{
    internal interface IFilterer
    {
        public ITraversable Filter(ITraversable node, IEnumerable<IStringNode> possibleFilterNodes);
    }


    internal interface ICompositeFilterer : IFilterer
    {
        public void Add(IComponentFilterer component);
    }


    internal interface IComponentFilterer
    {
        public bool MayHandle(ITraversable node);

        /// <returns>Returns null to indicate that node doesn't match filter</returns>
        public ITraversable Filter(ITraversable node, IStringNode filter);
    }
}
