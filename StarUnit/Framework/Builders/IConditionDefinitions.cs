using System;

namespace Phrasefable.StardewMods.StarUnit.Framework.Builders
{
    public interface IConditionDefinitions
    {
        public Func<IResult> WorldReady { get; }
    }
}
