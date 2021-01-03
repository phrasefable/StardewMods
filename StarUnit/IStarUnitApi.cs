using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Model;

namespace Phrasefable.StardewMods.StarUnit
{
    public interface IStarUnitApi
    {
        public IBuilderFactory BuilderFactory { get; set; }
        public void Register(string modId, params ITraversable[] testNodes);
    }
}