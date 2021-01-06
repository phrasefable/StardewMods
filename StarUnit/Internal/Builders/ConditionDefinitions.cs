using System;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Results;
using Phrasefable.StardewMods.StarUnit.Internal.Results;
using StardewModdingAPI;

namespace Phrasefable.StardewMods.StarUnit.Internal.Builders
{
    internal class ConditionDefinitions : IConditionDefinitions
    {
        public Func<IResult> WorldReady { get; } = () => Context.IsWorldReady
            ? new Result {Status = Status.Pass}
            : new Result {Status = Status.Fail, Message = "World not ready."};
    }
}
