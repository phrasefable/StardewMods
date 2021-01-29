using System.Collections.Generic;

namespace Phrasefable.StardewMods.StarUnit.Internal.Filterers
{
    internal interface IStringNode
    {
        public string Key { get; }
        public bool AllChildren { get; }
        public IEnumerable<IStringNode> Children { get; }
    }
}