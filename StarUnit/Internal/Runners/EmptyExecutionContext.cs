using System;
using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;

namespace Phrasefable.StardewMods.StarUnit.Internal.Runners
{
    internal class EmptyExecutionContext : IExecutionContext
    {
        public void Execute<T>(OnCompleted @return, Action<OnCompleted, T> executable, T node) where T : ITraversable
        {
            executable(@return, node);
        }
    }
}