using Phrasefable.StardewMods.StarUnit.Framework.Model;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.Results
{
    internal class TestResult : TraversableResult, ITestResult
    {
        public TestResult()
        {
        }

        public TestResult(ITest test)
        {
            this.Key = test.Key;
            this.LongName = test.LongName;
        }
    }
}
