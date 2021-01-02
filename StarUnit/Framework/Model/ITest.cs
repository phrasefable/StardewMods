using System;

namespace Phrasefable.StardewMods.StarUnit.Framework.Model
{
    public interface ITest : ITraversable
    {
        public Func<Result> TestMethod { get; set; }
    }
}