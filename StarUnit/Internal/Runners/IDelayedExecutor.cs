using System;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;

namespace Phrasefable.StardewMods.StarUnit.Internal.Runners
{
    internal interface IDelayedExecutor
    {
        public Action<Action> After(Delay delay);
    }
}
