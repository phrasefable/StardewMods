using Phrasefable.StardewMods.StarUnit.Framework.Definitions;

namespace Phrasefable.StardewMods.StarUnit.Internal.Filterers
{
    internal class TestFilterer : IComponentFilterer
    {
        public bool MayHandle(ITraversable node)
        {
            return node is ITest;
        }


        public ITraversable Filter(ITraversable node, IStringNode filter)
        {
            return node;
        }
    }
}
