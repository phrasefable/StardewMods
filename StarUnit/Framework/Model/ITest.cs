using System;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Framework.Model
{
    public interface ITest : ITraversable
    {
        public Func<ITestResult> TestMethod { get; set; }
    }
}
