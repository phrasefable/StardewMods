using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;

namespace Phrasefable.StardewMods.StarUnit
{
    public interface IStarUnitApi
    {
        public ITestDefinitionFactory TestDefinitionFactory { get; set; }
        public void Register(string modId, params ITraversable[] testNodes);
    }
}