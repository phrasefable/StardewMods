using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Framework.Definitions
{
    public interface ITest : ITraversable
    {
        public Func<ITestResult> TestMethod { get; }
    }
}
