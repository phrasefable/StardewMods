using System;

namespace Phrasefable.StardewMods.StarUnit.Api.Model
{
    public interface ITest : ITraversable
    {
        public Func<Result> TestMethod { get; set; }
    }
}