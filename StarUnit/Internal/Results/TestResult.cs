using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.Results
{
    internal class TestResult : TraversableResult, ITestResult
    {
        public TestResult() { }

        public TestResult(ITest test)
        {
            this.Key = test.Key;
            this.LongName = test.LongName;
        }

        public TestResult(ITest test, IResult baseResult) : this(test)
        {
            this.Status = baseResult.Status;
            this.Message = baseResult.Message;
        }
    }
}
