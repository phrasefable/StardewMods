using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;
using Phrasefable.StardewMods.StarUnit.Internal.Results;

namespace Phrasefable.StardewMods.StarUnit.Internal.Runners
{
    internal class TestRunner : TraversableRunner<ITest>
    {
        protected override ITraversableResult _Run(ITest test)
        {
            var result = (TestResult) test.TestMethod.Invoke();
            result.Key = test.Key;
            result.LongName = test.LongName;
            return result;
        }


        protected override ITraversableResult _Skip(ITest test)
        {
            return new TestResult(test) {Status = Status.Skipped};
        }
    }
}
