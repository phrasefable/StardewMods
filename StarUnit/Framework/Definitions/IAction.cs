using System;

namespace Phrasefable.StardewMods.StarUnit.Framework.Definitions
{
    public interface IAction
    {
        public Action Action { get; }
        public Delay Delay { get; }
    }
}