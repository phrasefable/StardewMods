using System.Collections.Generic;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;

namespace Phrasefable.StardewMods.StarUnit.Internal.Filterers.Wrappers
{
    internal interface IBranchWrapperFactory<T> where T : ITraversableBranch
    {
        /// <summary>
        ///     Creates wrapper of branch with all properties identical except for Children.
        /// </summary>
        public T Wrap(T branch, IEnumerable<ITraversable> children);
    }
}