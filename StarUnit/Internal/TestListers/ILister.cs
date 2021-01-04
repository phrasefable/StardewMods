using Phrasefable.StardewMods.StarUnit.Framework.Model;

namespace Phrasefable.StardewMods.StarUnit.Internal.TestListers
{
    public interface ILister
    {
        public void List(ITraversable node);
    }
}