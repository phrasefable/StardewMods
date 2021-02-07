using System;
using JetBrains.Annotations;

namespace Phrasefable.StardewMods.StarUnit.Framework.Definitions
{
    public interface IAction
    {
        [NotNull] public Action Action { get; }
        public Delay Delay { get; }
    }
}