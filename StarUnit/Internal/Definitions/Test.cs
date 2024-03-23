using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.Definitions
{
    internal class Test : Traversable, ITest
    {
        public Func<ITestResult> TestMethod { get; set; }
    }
}
