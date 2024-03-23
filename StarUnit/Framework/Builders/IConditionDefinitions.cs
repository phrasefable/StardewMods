using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Framework.Builders
{
    public interface IConditionDefinitions
    {
        public Func<IResult> WorldReady { get; }
    }
}
