using Phrasefable.StardewMods.StarUnit.Framework.Definitions;

namespace Phrasefable.StardewMods.StarUnit.Internal.TestListers
{
    internal interface ILister
    {
        public void List(ITraversable node);
    }
}